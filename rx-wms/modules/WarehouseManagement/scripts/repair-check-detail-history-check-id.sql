-- 修复旧版本在首次创建盘点历史时写入 CheckId = 0 的明细。
-- 关联链路：CheckDetailHis.TaskId -> StockTask.PlanId -> Check.CheckCode -> CheckHis.Id。
-- 执行前请先运行第一段 SELECT 核对待修复数据。

SELECT
    detailHis.Id AS CheckDetailHisId,
    detailHis.TaskId,
    detailHis.CellName,
    detailHis.CheckId AS InvalidCheckHisId,
    checkHis.Id AS TargetCheckHisId,
    checkHis.CheckCode
FROM checkdetailhis AS detailHis
INNER JOIN stocktask AS stockTask ON stockTask.Id = detailHis.TaskId
INNER JOIN `check` AS checkMain ON checkMain.Id = stockTask.PlanId
INNER JOIN checkhis AS checkHis ON checkHis.CheckCode = checkMain.CheckCode
WHERE detailHis.CheckId = 0
  AND stockTask.PlanTypeCode = 'Check';

UPDATE checkdetailhis AS detailHis
INNER JOIN stocktask AS stockTask ON stockTask.Id = detailHis.TaskId
INNER JOIN `check` AS checkMain ON checkMain.Id = stockTask.PlanId
INNER JOIN checkhis AS checkHis ON checkHis.CheckCode = checkMain.CheckCode
SET detailHis.CheckId = checkHis.Id
WHERE detailHis.CheckId = 0
  AND stockTask.PlanTypeCode = 'Check';
