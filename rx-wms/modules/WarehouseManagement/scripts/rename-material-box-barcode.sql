-- MySQL 8：将实物条码数据库列统一命名为 MaterialBarcode。
-- 与包含对应 HasColumnName 映射的后端版本一起部署；执行前备份两张表并停止旧后端。
-- 仅适用于两张表仍存在旧列且不存在新列的数据库；改名保留原列类型和数据。
ALTER TABLE `materialbox` RENAME COLUMN `MaterialBoxBarcode` TO `MaterialBarcode`;
ALTER TABLE `stocktask` RENAME COLUMN `MaterialBoxBarcode` TO `MaterialBarcode`;

-- 检查：结果应只包含新列名。
SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('materialbox', 'stocktask')
  AND COLUMN_NAME IN ('MaterialBoxBarcode', 'MaterialBarcode');

-- 回滚需同时恢复旧后端映射：
-- ALTER TABLE `stocktask` RENAME COLUMN `MaterialBarcode` TO `MaterialBoxBarcode`;
-- ALTER TABLE `materialbox` RENAME COLUMN `MaterialBarcode` TO `MaterialBoxBarcode`;
