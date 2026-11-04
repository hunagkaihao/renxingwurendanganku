-- MySQL: add the outbound fields used by material management.
-- BorrowFlag: 0 = normal stock-out, 1 = borrowed stock-out.
-- Safe to run repeatedly against the current database.

SET @out_time_exists := (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'MaterialBox'
      AND COLUMN_NAME = 'MaterialOutTime'
);

SET @out_time_sql := IF(
    @out_time_exists = 0,
    'ALTER TABLE `MaterialBox` ADD COLUMN `MaterialOutTime` datetime NULL',
    'SELECT 1'
);

PREPARE add_material_out_time FROM @out_time_sql;
EXECUTE add_material_out_time;
DEALLOCATE PREPARE add_material_out_time;

SET @borrow_flag_exists := (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'MaterialBox'
      AND COLUMN_NAME = 'BorrowFlag'
);

SET @borrow_flag_sql := IF(
    @borrow_flag_exists = 0,
    'ALTER TABLE `MaterialBox` ADD COLUMN `BorrowFlag` int NOT NULL DEFAULT 0',
    'SELECT 1'
);

PREPARE add_material_borrow_flag FROM @borrow_flag_sql;
EXECUTE add_material_borrow_flag;
DEALLOCATE PREPARE add_material_borrow_flag;

-- A prior manual column addition may have allowed NULL or omitted the default.
-- Normalize it so existing material records are ordinary stock-out records.
UPDATE `MaterialBox`
SET `BorrowFlag` = 0
WHERE `BorrowFlag` IS NULL;

ALTER TABLE `MaterialBox`
    MODIFY COLUMN `BorrowFlag` int NOT NULL DEFAULT 0;
