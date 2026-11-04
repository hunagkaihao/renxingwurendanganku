-- MySQL: add the material-code column used by the cell inventory display.
-- Safe to run repeatedly against the current database.
SET @column_exists := (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'Cell'
      AND COLUMN_NAME = 'MaterialCode'
);

SET @sql := IF(
    @column_exists = 0,
    'ALTER TABLE `Cell` ADD COLUMN `MaterialCode` varchar(128) NULL',
    'SELECT 1'
);

PREPARE add_cell_material_code FROM @sql;
EXECUTE add_cell_material_code;
DEALLOCATE PREPARE add_cell_material_code;
