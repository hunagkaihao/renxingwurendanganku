-- MySQL 8.0：WMS 普通业务字符串收敛为 varchar(200)，补充精确查询与关联索引。
-- 先选择目标数据库。默认仅预检并输出正向、回退 SQL；执行前在同一连接 SET @wms_apply = 1。
-- 仅处理下列 WMS 业务表中的 TEXT 系列列；保留已有 VARCHAR、ABP 扩展数据和人脸图片。
-- Cell.MaterialCode 按现有实体约束保留 128 字符上限，其余候选字段为 200 字符。
-- 应在停止业务写入后执行并保存输出。DDL 会隐式提交，失败后根据已完成语句回退。
SET @wms_apply = COALESCE(@wms_apply, 0);

DROP PROCEDURE IF EXISTS wms_optimize_storage_indexes_20261102;
DELIMITER $$
CREATE PROCEDURE wms_optimize_storage_indexes_20261102(IN p_apply BOOLEAN)
BEGIN
    DECLARE v_done BOOLEAN DEFAULT FALSE;
    DECLARE v_schema VARCHAR(64);
    DECLARE v_old_mode TEXT;
    DECLARE v_old_group_length BIGINT UNSIGNED;
    DECLARE v_table VARCHAR(64);
    DECLARE v_previous_table VARCHAR(64) DEFAULT '';
    DECLARE v_column VARCHAR(64);
    DECLARE v_type TEXT;
    DECLARE v_charset VARCHAR(64);
    DECLARE v_collation VARCHAR(64);
    DECLARE v_nullable VARCHAR(3);
    DECLARE v_default LONGTEXT;
    DECLARE v_comment TEXT;
    DECLARE v_extra TEXT;
    DECLARE v_attributes LONGTEXT;
    DECLARE v_forward LONGTEXT DEFAULT '';
    DECLARE v_rollback LONGTEXT DEFAULT '';
    DECLARE v_index VARCHAR(64);
    DECLARE v_columns TEXT;
    DECLARE v_exists INT;
    DECLARE v_expected INT;
    DECLARE v_limit INT;
    DECLARE v_message VARCHAR(128);

    DECLARE column_cursor CURSOR FOR
        SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, CHARACTER_SET_NAME,
               COLLATION_NAME, IS_NULLABLE, COLUMN_DEFAULT, COLUMN_COMMENT, EXTRA
        FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND LOWER(TABLE_NAME) IN (
              'cell', 'check', 'checkdetail', 'checkdetailhis', 'checkhis', 'face',
              'goods', 'goodsclass', 'goodstype', 'logicarea', 'material',
              'materialbox', 'materialboxdetail', 'plan', 'planlist', 'plantype',
              'rfid', 'stocktask', 'stocktaskdetail', 'stocktasktype', 'taskhis',
              'taskhisdetail', 'vein', 'warehouse', 'warehousearea')
          AND (DATA_TYPE IN ('tinytext', 'text', 'mediumtext', 'longtext')
              OR (LOWER(TABLE_NAME) = 'cell' AND COLUMN_NAME = 'MaterialCode'
                  AND DATA_TYPE = 'varchar' AND CHARACTER_MAXIMUM_LENGTH <> 128))
          AND COLUMN_NAME NOT IN ('ExtraProperties', 'ConcurrencyStamp')
          AND NOT (LOWER(TABLE_NAME) = 'face' AND COLUMN_NAME = 'ImageDate')
        ORDER BY TABLE_NAME, ORDINAL_POSITION;

    DECLARE index_cursor CURSOR FOR
        SELECT t.TABLE_NAME, i.index_name, i.column_names
        FROM wms_index_targets i
        JOIN information_schema.TABLES t
          ON t.TABLE_SCHEMA = DATABASE() AND LOWER(t.TABLE_NAME) = i.table_name
        ORDER BY i.table_name, i.index_name;

    DECLARE ddl_cursor CURSOR FOR
        SELECT forward_sql FROM wms_ddl_plan ORDER BY sequence_id;

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET v_done = TRUE;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET SESSION sql_mode = v_old_mode;
        SET SESSION group_concat_max_len = v_old_group_length;
        RESIGNAL;
    END;

    SET v_schema = DATABASE();
    SET v_old_mode = @@SESSION.sql_mode;
    SET v_old_group_length = @@SESSION.group_concat_max_len;
    IF v_schema IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Select the target database first';
    END IF;
    -- QUOTE() 生成的转义与本会话模式保持一致；严格模式防止预检后的超长写入被静默截断。
    SET SESSION sql_mode = CONCAT_WS(',',
        NULLIF(REPLACE(v_old_mode, 'NO_BACKSLASH_ESCAPES', ''), ''), 'STRICT_ALL_TABLES');
    SET SESSION group_concat_max_len = 16384;

    DROP TEMPORARY TABLE IF EXISTS wms_text_profile;
    CREATE TEMPORARY TABLE wms_text_profile (
        table_name VARCHAR(64) NOT NULL COMMENT '实际业务表名',
        column_name VARCHAR(64) NOT NULL COMMENT '待收敛的普通文本列',
        max_chars BIGINT NULL COMMENT '存量最大字符数，空表或全空值时为 NULL',
        limit_chars INT NOT NULL COMMENT '目标字符上限，已知约束优先于默认 200'
    );
    DROP TEMPORARY TABLE IF EXISTS wms_ddl_plan;
    CREATE TEMPORARY TABLE wms_ddl_plan (
        sequence_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '正向执行顺序；回退时倒序',
        forward_sql LONGTEXT NOT NULL COMMENT '本次需要执行的 DDL',
        rollback_sql LONGTEXT NOT NULL COMMENT '恢复本次变更前定义的 DDL'
    );
    DROP TEMPORARY TABLE IF EXISTS wms_index_targets;
    CREATE TEMPORARY TABLE wms_index_targets (
        table_name VARCHAR(64) NOT NULL COMMENT '小写业务表名',
        index_name VARCHAR(64) NOT NULL COMMENT '与 EF 映射一致的索引名',
        column_names VARCHAR(256) NOT NULL COMMENT '有序列名，以逗号分隔'
    );
    INSERT INTO wms_index_targets VALUES
        ('cell', 'IX_Cell_CellCode', 'CellCode'),
        ('cell', 'IX_Cell_CellName', 'CellName'),
        ('cell', 'IX_Cell_MaterialCode', 'MaterialCode'),
        ('material', 'IX_Material_MaterialCode', 'MaterialCode'),
        ('material', 'IX_Material_RfidId', 'RfidId'),
        ('materialbox', 'IX_MaterialBox_MaterialBarcode', 'MaterialBarcode'),
        ('materialbox', 'IX_MaterialBox_MaterialBoxName', 'MaterialBoxName'),
        ('materialbox', 'IX_MaterialBox_CellId', 'CellId'),
        ('materialboxdetail', 'IX_MaterialBoxDetail_MaterialId_IsDeleted', 'MaterialId,IsDeleted'),
        ('stocktask', 'IX_StockTask_MaterialBarcode_TaskStatus', 'MaterialBarcode,TaskStatus'),
        ('checkdetailhis', 'IX_CheckDetailHis_CheckId', 'CheckId'),
        ('checkdetailhis', 'IX_CheckDetailHis_TaskId', 'TaskId');

    -- 生成完整计划后才执行任何业务表 DDL；任意超长值或不支持的列属性都会终止。
    OPEN column_cursor;
    column_loop: LOOP
        FETCH column_cursor INTO v_table, v_column, v_type, v_charset,
            v_collation, v_nullable, v_default, v_comment, v_extra;
        IF v_done THEN LEAVE column_loop; END IF;
        SET v_limit = IF(LOWER(v_table) = 'cell' AND v_column = 'MaterialCode', 128, 200);
        IF v_extra <> '' OR CHAR_LENGTH(v_default) > v_limit THEN
            SET v_message = CONCAT('Unsupported attributes/default: ', v_table, '.', v_column);
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_message;
        END IF;
        SET @wms_observed = NULL;
        SET @wms_sql = CONCAT('SELECT MAX(CHAR_LENGTH(`', REPLACE(v_column, '`', '``'),
            '`)) INTO @wms_observed FROM `', REPLACE(v_schema, '`', '``'),
            '`.`', REPLACE(v_table, '`', '``'), '`');
        PREPARE wms_statement FROM @wms_sql;
        EXECUTE wms_statement;
        DEALLOCATE PREPARE wms_statement;
        INSERT INTO wms_text_profile VALUES (v_table, v_column, @wms_observed, v_limit);
        IF @wms_observed > v_limit THEN
            SELECT * FROM wms_text_profile WHERE max_chars > limit_chars;
            SET v_message = CONCAT('More than ', v_limit, ' characters: ', v_table, '.', v_column);
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_message;
        END IF;

        IF v_previous_table <> '' AND v_previous_table <> v_table THEN
            INSERT INTO wms_ddl_plan (forward_sql, rollback_sql)
            VALUES (CONCAT(v_forward, ';'), CONCAT(v_rollback, ';'));
        END IF;
        IF v_previous_table <> v_table THEN
            SET v_forward = CONCAT('ALTER TABLE `', REPLACE(v_schema, '`', '``'),
                '`.`', REPLACE(v_table, '`', '``'), '` ');
            SET v_rollback = v_forward;
        ELSE
            SET v_forward = CONCAT(v_forward, ', ');
            SET v_rollback = CONCAT(v_rollback, ', ');
        END IF;
        SET v_attributes = CONCAT(' CHARACTER SET ', v_charset, ' COLLATE ', v_collation,
            IF(v_nullable = 'YES', ' NULL', ' NOT NULL'),
            IF(v_default IS NOT NULL, CONCAT(' DEFAULT ', QUOTE(v_default)),
                IF(v_nullable = 'YES', ' DEFAULT NULL', '')),
            ' COMMENT ', QUOTE(v_comment));
        SET v_forward = CONCAT(v_forward, 'MODIFY COLUMN `', REPLACE(v_column, '`', '``'),
            '` varchar(', v_limit, ')', v_attributes);
        SET v_rollback = CONCAT(v_rollback, 'MODIFY COLUMN `', REPLACE(v_column, '`', '``'),
            '` ', v_type, v_attributes);
        SET v_previous_table = v_table;
    END LOOP;
    CLOSE column_cursor;
    IF v_previous_table <> '' THEN
        INSERT INTO wms_ddl_plan (forward_sql, rollback_sql)
        VALUES (CONCAT(v_forward, ';'), CONCAT(v_rollback, ';'));
    END IF;

    -- 必须具备所有目标表、目标列；不静默跳过未完成的前置结构迁移。
    SELECT COUNT(*) INTO v_expected FROM wms_index_targets;
    SELECT COUNT(*) INTO v_exists FROM wms_index_targets i JOIN information_schema.TABLES t
        ON t.TABLE_SCHEMA = v_schema AND LOWER(t.TABLE_NAME) = i.table_name;
    IF v_expected <> v_exists THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Required WMS tables are missing';
    END IF;
    SET v_done = FALSE;
    OPEN index_cursor;
    index_loop: LOOP
        FETCH index_cursor INTO v_table, v_index, v_columns;
        IF v_done THEN LEAVE index_loop; END IF;
        SET v_expected = 1 + CHAR_LENGTH(v_columns) - CHAR_LENGTH(REPLACE(v_columns, ',', ''));
        SELECT COUNT(*) INTO v_exists FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = v_schema AND TABLE_NAME = v_table
          AND FIND_IN_SET(COLUMN_NAME, v_columns) > 0;
        IF v_exists <> v_expected THEN
            SET v_message = CONCAT('Required index columns are missing: ', v_table, '.', v_index);
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_message;
        END IF;
        -- 已有完整列索引或以目标列为左前缀的索引时跳过；前缀、不可见索引不算等效。
        SELECT COUNT(*) INTO v_exists FROM (
            SELECT GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX SEPARATOR ',') AS columns_list
            FROM information_schema.STATISTICS
            WHERE TABLE_SCHEMA = v_schema AND TABLE_NAME = v_table
              AND INDEX_TYPE = 'BTREE' AND IS_VISIBLE = 'YES'
            GROUP BY INDEX_NAME
            HAVING SUM(SUB_PART IS NOT NULL OR COLUMN_NAME IS NULL) = 0
        ) existing_indexes
        WHERE columns_list = v_columns OR columns_list LIKE CONCAT(v_columns, ',%');
        IF v_exists = 0 THEN
            SELECT COUNT(*) INTO v_exists FROM information_schema.STATISTICS
            WHERE TABLE_SCHEMA = v_schema AND TABLE_NAME = v_table AND INDEX_NAME = v_index;
            IF v_exists > 0 THEN
                SET v_message = CONCAT('Conflicting index definition: ', v_table, '.', v_index);
                SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_message;
            END IF;
            INSERT INTO wms_ddl_plan (forward_sql, rollback_sql) VALUES (
                CONCAT('ALTER TABLE `', REPLACE(v_schema, '`', '``'), '`.`', v_table,
                    '` ADD INDEX `', v_index, '` (`', REPLACE(v_columns, ',', '`,`'), '`);'),
                CONCAT('ALTER TABLE `', REPLACE(v_schema, '`', '``'), '`.`', v_table,
                    '` DROP INDEX `', v_index, '`;'));
        END IF;
    END LOOP;
    CLOSE index_cursor;

    SELECT * FROM wms_text_profile ORDER BY table_name, column_name;
    SELECT * FROM wms_ddl_plan ORDER BY sequence_id;
    -- 输出重复定义候选。保留唯一约束、外键支撑索引，不能仅按列前缀自动删除。
    SELECT a.TABLE_NAME, a.INDEX_NAME AS duplicate_index, b.INDEX_NAME AS equivalent_index
    FROM (
        SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, INDEX_TYPE, IS_VISIBLE,
               GROUP_CONCAT(CONCAT(COALESCE(COLUMN_NAME, EXPRESSION), ':',
                   COALESCE(SUB_PART, 0), ':', COALESCE(COLLATION, ''))
                   ORDER BY SEQ_IN_INDEX SEPARATOR ',') AS definition
        FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = v_schema
          AND LOWER(TABLE_NAME) IN ('cell', 'check', 'checkdetail', 'checkdetailhis',
              'checkhis', 'face', 'goods', 'goodsclass', 'goodstype', 'logicarea',
              'material', 'materialbox', 'materialboxdetail', 'plan', 'planlist',
              'plantype', 'rfid', 'stocktask', 'stocktaskdetail', 'stocktasktype',
              'taskhis', 'taskhisdetail', 'vein', 'warehouse', 'warehousearea')
        GROUP BY TABLE_NAME, INDEX_NAME, NON_UNIQUE, INDEX_TYPE, IS_VISIBLE
    ) a JOIN (
        SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, INDEX_TYPE, IS_VISIBLE,
               GROUP_CONCAT(CONCAT(COALESCE(COLUMN_NAME, EXPRESSION), ':',
                   COALESCE(SUB_PART, 0), ':', COALESCE(COLLATION, ''))
                   ORDER BY SEQ_IN_INDEX SEPARATOR ',') AS definition
        FROM information_schema.STATISTICS
        WHERE TABLE_SCHEMA = v_schema
        GROUP BY TABLE_NAME, INDEX_NAME, NON_UNIQUE, INDEX_TYPE, IS_VISIBLE
    ) b ON a.TABLE_NAME = b.TABLE_NAME AND a.INDEX_NAME < b.INDEX_NAME
        AND a.NON_UNIQUE = b.NON_UNIQUE AND a.INDEX_TYPE = b.INDEX_TYPE
        AND a.IS_VISIBLE = b.IS_VISIBLE AND a.definition = b.definition;

    IF p_apply THEN
        SET v_done = FALSE;
        OPEN ddl_cursor;
        ddl_loop: LOOP
            FETCH ddl_cursor INTO v_forward;
            IF v_done THEN LEAVE ddl_loop; END IF;
            SET @wms_sql = v_forward;
            PREPARE wms_statement FROM @wms_sql;
            EXECUTE wms_statement;
            DEALLOCATE PREPARE wms_statement;
        END LOOP;
        CLOSE ddl_cursor;
    END IF;
    SELECT IF(p_apply, 'APPLIED', 'PREVIEW') AS result,
           (SELECT COUNT(*) FROM wms_text_profile) AS text_columns,
           (SELECT COUNT(*) FROM wms_ddl_plan) AS ddl_statements;
    SET SESSION sql_mode = v_old_mode;
    SET SESSION group_concat_max_len = v_old_group_length;
END$$
DELIMITER ;
CALL wms_optimize_storage_indexes_20261102(@wms_apply);
DROP PROCEDURE wms_optimize_storage_indexes_20261102;
