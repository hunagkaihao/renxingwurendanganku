/*
  Normalize existing Cell records to:
    Cell_x = row   (CellCode segment 1)
    Cell_y = column (CellCode segment 2)
    Cell_z = level  (CellCode segment 3)

  EF Core maps Cell to [Cell] with no table prefix or schema.
  Review the preview result and execute only through an authorized database operator.
*/

-- Preview rows that will change. Expected CellCode format: DD-DDD-DD.
SELECT
    Id,
    CellCode,
    Cell_x AS CurrentCell_x,
    Cell_y AS CurrentCell_y,
    Cell_z AS CurrentCell_z,
    TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 3)) AS NewCell_x,
    TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 2)) AS NewCell_y,
    TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 1)) AS NewCell_z
FROM [Cell]
WHERE CellCode LIKE '[0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9]';

BEGIN TRANSACTION;

UPDATE [Cell]
SET
    Cell_x = TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 3)),
    Cell_y = TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 2)),
    Cell_z = TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 1))
WHERE CellCode LIKE '[0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9]'
  AND TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 3)) IS NOT NULL
  AND TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 2)) IS NOT NULL
  AND TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 1)) IS NOT NULL;

-- Validate that no normalized row disagrees with its CellCode.
SELECT Id, CellCode, Cell_x, Cell_y, Cell_z
FROM [Cell]
WHERE CellCode LIKE '[0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9]'
  AND (
      Cell_x <> TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 3))
      OR Cell_y <> TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 2))
      OR Cell_z <> TRY_CONVERT(int, PARSENAME(REPLACE(CellCode, '-', '.'), 1))
  );

-- Change ROLLBACK to COMMIT only after the validation query returns no rows.
ROLLBACK TRANSACTION;
