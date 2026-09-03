using Abp.Domain.Entities;
using Lion.AbpPro.ConfigurationOptions;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.Archives;
using WarehouseManagement.Archives.Aggregates;
using WarehouseManagement.Cells;
using WarehouseManagement.Checks;
using WarehouseManagement.Goodss;
using WarehouseManagement.Plans;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.TaskHiss.Aggregates;
using WarehouseManagement.WcsTasks;
using WarehouseManagement.WcsTasks.Dto;
using static System.Collections.Specialized.BitVector32;
using static WarehouseManagement.Permissions.WarehouseManagementPermissions;

namespace WarehouseManagement.StockTasks
{
    public class StockTaskManager : StockTaskDomainService
    {
        /// <summary>
        /// 将 WMS 业务任务类型转换为发送给 WCS 的粗粒度任务类型。
        /// WCS 当前只接收该值，实际类型仍由起点和终点推导。
        /// </summary>
        private static string ToWcsTaskType(ManageType manageType)
        {
            return manageType switch
            {
                ManageType.NPFullStockIn or
                ManageType.HPFullStockIn or
                ManageType.NPSupllyStockIn or
                ManageType.HPSupplyStockIn or
                ManageType.FullSotckUp or
                ManageType.EmptyStockIn or
                ManageType.HPBatchStockIn or
                ManageType.SurplusIn => "StockIn",

                ManageType.NPSortStockOut or
                ManageType.HPSortStockOut or
                ManageType.FullStockDown or
                ManageType.EmptyStockOut or
                ManageType.SealedGoodsDown or
                ManageType.NpFullStockOut or
                ManageType.LossOut => "StockOut",

                ManageType.HpAnnualCheckDown => "CheckDown",
                _ => manageType.ToString()
            };
        }

        private readonly IStockTaskRepository _stockTaskRepository;
        private readonly IStockTaskDetailRepository _stockTaskDetailRepository;
        private readonly ICellRepository _cellRepository;
        private readonly ICurrentUser _currentUser;
        private readonly GoodsManager _goodsManager;
        private readonly CellManager _cellManager;
        private readonly ArchiveBoxManager _archiveBoxManager;
        private readonly ArchiveManager _archiveManager;
        private readonly FbqOptions _fbqOptions;
        private readonly WcsApiManager _wcsApiManager;
        private readonly CheckManager _checkManager;
        private readonly PlanManager _planManager;
        private readonly IIdentityUserAppService identityUserAppService;
        //private readonly IWcsTaskService _wcsTaskService;

        public string FbqEnable { get; set; }
        //private readonly IDistributedCache<StockTask> _cache;//设置缓存

        //    public StockTaskManager(
        //IStockTaskRepository StockTaskRepository,
        //IDistributedCache<StockTaskDto> cache)
        //    {
        //        _StockTaskRepository = StockTaskRepository;
        //        _cache = cache;
        //    }

        public StockTaskManager(
            IStockTaskRepository stockTaskRepository, ICellRepository cellRepository
            , GoodsManager goodsManager
            , CellManager cellManager
            , ICurrentUser currentUser
            , IStockTaskDetailRepository stockTaskDetailRepository
            , IOptionsSnapshot<FbqOptions> fbqOptions
            , WcsApiManager wcsApiManager
            , ArchiveBoxManager archiveBoxManager
            , CheckManager checkManager
            , PlanManager planManager
            , ArchiveManager archiveManager
            )
        {
            _stockTaskRepository = stockTaskRepository;
            _cellRepository = cellRepository;
            _goodsManager = goodsManager;
            _cellManager = cellManager;
            _currentUser = currentUser;
            _stockTaskDetailRepository = stockTaskDetailRepository;
            FbqEnable = fbqOptions.Value.Enable;
            _wcsApiManager = wcsApiManager;
            _archiveBoxManager = archiveBoxManager;
            _checkManager = checkManager;
            _planManager = planManager;
            _archiveManager = archiveManager;
        }
        //public async Task<StockTask> CreateAsync(StockTask stockTask, List<StockTaskDetail> stockTaskDetails)
        //{
        //    var entity = new StockTask(stockTask, stockTaskDetails);
        //    return await _stockTaskRepository.InsertAsync(entity);
        //}
       
        
       

      

        public async Task DeleteAsync(int stockTaskId)
        {
            var entity = await _stockTaskRepository.FindByIdAsync(stockTaskId);
            if (entity == null)
                throw new UserFriendlyException(message: "物料盒不存在");
            await _stockTaskRepository.DeleteAsync(entity);
        }
        public async Task<StockTask> UpdateAsync(int id, string manageTypeCode, string stockBarcode, int startCellId, int endCellId)
        {
            var entity = await _stockTaskRepository.FindByIdAsync(id);
            if (entity == null)
                throw new UserFriendlyException(message: "物料盒不存在");
            string startCellCode = null;
            if (startCellId != 0)
            {
                var startCell = await _cellRepository.FindAsync(startCellId);
                if (startCell.CellStatus != CellStatus.Nohave || startCell.RunStatus != CellRunStatus.Enable)
                    throw new UserFriendlyException(message: "开始库位状态错误");
                startCellCode = startCell.CellCode;
            }
            //startCell.
            string endCellCode = null;
            if (endCellId != 0)
            {
                var endCell = await _cellRepository.FindAsync(endCellId);
                if (endCell.CellStatus != CellStatus.Nohave || endCell.RunStatus != CellRunStatus.Enable)
                    throw new UserFriendlyException(message: "目标库位状态错误");
                endCellCode = endCell.CellCode;
            }
            entity.Update(manageTypeCode, stockBarcode, startCellId, endCellId, startCellCode, endCellCode);
            return await _stockTaskRepository.UpdateAsync(entity);
        }

        public async Task<StockTask> UpdateAsync(StockTask stockTask)
        {
            return await _stockTaskRepository.UpdateAsync(stockTask);
        }
        
        public async Task<StockTask> SetAsCancelAsync(int stockTaskId)
        {
            var entity = await _stockTaskRepository.FindByIdAsync(stockTaskId);
            if (entity == null)
                throw new UserFriendlyException(message: "任务不存在");
            if(entity.ManageStatus == ManageStatus.WaitingExecute)
            {
                //await _stockTaskRepository.DeleteAsync(stockTaskId);
                //return null;
                entity.SetAsCancel();
                StockTask stockTaskRtn = await _stockTaskRepository.UpdateAsync(entity);
                Log.Warning($"{_currentUser?.UserName}将任务:{stockTaskRtn?.Id.ToString()}的状态设置为已取消。");
                return stockTaskRtn;
            }
            else
            {
                entity.SetAsCancel();
                //await _wcsTaskService.CancelTask();
                //await _wcsApiManager.Pause();
                //await _wcsApiManager.ForceDone(entity.Id);
                //await _wcsApiManager.Restart();
                StockTask stockTaskRtn = await _stockTaskRepository.UpdateAsync(entity);
                Log.Warning($"{_currentUser?.UserName}将任务:{stockTaskRtn?.Id.ToString()}的状态设置为已取消。");
                return stockTaskRtn;
            }  
        }

        public async Task<StockTask> FindByIdAsync(int stockTaskId)
        {
            return await _stockTaskRepository.FindByIdAsync(stockTaskId);
        }
       

        /// <summary>
        /// 是否存在出入库任务
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExistInOutManage()
        {
            if (await _stockTaskRepository.GetPagingCountAsync() > 0)
                return true;
            else
                return false;
        }



        //检查档案盒任务是否重复
        public virtual async Task<bool> ValidateStockManageExist(string boxRfid)
        {
            try
            {
                if (boxRfid == "")
                {
                    return false;
                }
                var mMain = await _stockTaskRepository.GetListAsync(a => a.ArchiveBoxRfid == boxRfid & (a.ManageStatus != ManageStatus.Complete & a.ManageStatus != ManageStatus.Cancel));
                if (mMain.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }


        }

        public async Task<List<StockTaskDetail>> GetTaskDetailsByGoodsCodeAndBatchNoAsync(string goodsCode, string goodsBatchNo)
        {
            //获取物料基础信息
            var goods = await _goodsManager.GetByCodeAsync(goodsCode);
            if (goods == null)
                throw new UserFriendlyException(message: "物料不存在");
            //在任务表中查找物料
            return await _stockTaskDetailRepository.GetListAsync(f => f.GoodsId == goods.Id
            & f.GoodsBatchNo == goodsBatchNo & (f.StorageListStatus != ManageStatus.Complete & f.StorageListStatus != ManageStatus.Cancel));
        }

        public async Task<StockTask> UpdateDetailQuantityAsync(int stockTaskDetailId, int stockTaskId, decimal manageListQuantity)
        {
            var entity = await _stockTaskRepository.FindByIdAsync(stockTaskId, true);
            if (entity == null)
                //throw new DataDictionaryDomainException(message: "数据字典不存在");
                throw new UserFriendlyException(message: "任务不存在");
            var detail = entity.Details.FirstOrDefault(e => e.Id == stockTaskDetailId);
            if (null == detail)
            {
                throw new UserFriendlyException(message: "任务明细不存在");
            }

            detail.UpdatemanageListQuantity(stockTaskId, manageListQuantity);
            return await _stockTaskRepository.UpdateAsync(entity);
        }

        [UnitOfWork]
        public async Task<bool> ManageCreateOut(int archiveBoxId, List<StockTaskDetail> stockTaskDetails, int userid)
        {
            //查询档案盒ID
            StockTaskDto stockTask = new();
            ArchiveBox archiveBox;
            string exMessage = null;
            try
            {
                exMessage = "未查询到档案盒";
                archiveBox = await _archiveBoxManager.GetArchiveBoxById(archiveBoxId);
                if (archiveBox.ArchiveBoxRfid.IsNullOrEmpty())
                {
                    exMessage = "档案盒未绑定标签";
                    throw new UserFriendlyException("档案盒未绑定标签");
                }
                stockTask.ArchiveBoxRfid = archiveBox.ArchiveBoxRfid;

                if (archiveBox.CellId == 0)
                {
                    exMessage = "档案盒不在库位中，无法出库";
                    throw new UserFriendlyException("档案盒不在库位中，无法出库");
                }
                stockTask.ManageTypeCode = ManageType.HPSortStockOut;
                stockTask.ManageStatus = ManageStatus.WaitingExecute;
                stockTask.StartCellId = archiveBox.CellId;
            }
            catch
            {
                throw new UserFriendlyException(exMessage);
            }
            //step1 该档案盒是否存在任务
            if (await ValidateStockManageExist(archiveBox.ArchiveBoxRfid))
            {
                throw new UserFriendlyException("档案盒已存在任务");
            }
            var startCell = await _cellManager.GetByIdAsync(archiveBox.CellId);
            //try
            //{
            //    var user = _userManager.GetUserById(userid);
            //    if (user == null)
            //    {
            //        throw new UserFriendlyException("未查询到用户");
            //    }
            //}
            //catch
            //{
            //    throw new UserFriendlyException("未查询到用户");
            //}

            StockTask s = new(ManageType.HPSortStockOut.ToString(),stockTask.ArchiveBoxRfid, startCell.CellCode ,startCell.Id);
            //会出现部分ID丢失的情况
            //if (s != null)
            //{
                if (archiveBox != null)
                {
                    //查询档案盒所包含的档案文件
                    List<ArchiveBoxDetail> archiveBoxDetails = archiveBox.Details;
                    Boolean flag = false;
                    for (int i = 0; i < archiveBoxDetails.Count; i++)
                    {
                        //商品ID是否存在
                        try
                        {
                            Archive archive = await _archiveManager.GetArchiveById(archiveBoxDetails[i].ArchiveId);
                        }
                        catch
                        {
                            throw new UserFriendlyException("档案文件不存在");
                        }

                        foreach (var stockTaskDetail in stockTaskDetails)
                        {
                            if (stockTaskDetail.GoodsId == archiveBoxDetails[i].ArchiveId)
                            {
                                s.AddDetail(archiveBoxDetails[i].Id, archiveBoxDetails[i].ArchiveId, _currentUser.Name);
                                await _stockTaskRepository.InsertAsync(s,true);
                                flag = true;
                            }
                        }


                    }
                    if (!flag & stockTaskDetails.Count > 0)
                    {
                        throw new UserFriendlyException("档案文件和档案盒不匹配");
                    }
                }
            //}
            //else
            //{
            //    throw new UserFriendlyException("数据同步异常");
            //}
            //step2锁定库位
            if (stockTask.StartCellId != 0)
            {
               // _storageManager.LockCell(mainObj.StartCellId);
            }
            if (stockTask.EndCellId != 0)
            {
                //_storageManager.LockCell(mainObj.EndCellId);
            }

            //20220423记录日志
            Log.Debug("用户:" + "创建了出库任务，任务ID：" + "  方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }
        public async Task<StockTask> CreateWCSIn(string manageTypeCode, ArchiveBox archiveBox)
        {
            //判断档案盒状态
            if (archiveBox.CellId != 0)
            {
                throw new UserFriendlyException(message: "档案盒已在库位");
            }
            //判断档案盒是否存在库位任务
            if (await ValidateStockManageExist(archiveBox.ArchiveBoxRfid))
            {
                throw new UserFriendlyException(message: "档案盒已存在出入库任务");
            }
            StockTask entity = null;
            try
            {
                entity = new StockTask(manageTypeCode, archiveBox.ArchiveBoxRfid);
                entity = await _stockTaskRepository.InsertAsync(entity, true);
                Log.Debug($"Task:{entity.Id} Box:{archiveBox.ArchiveBoxRfid} CreateStockIn Inserted StockTaskData: {JsonConvert.SerializeObject(entity)}");
                //entity.SetAsWaitingExecuted();

                //await SetAsExecutingAsync(entity.Id);
                //Log.Information($"Task:{entity.Id} Box:{storageBox.StorageBoxBarcode} CreateStockIn SetAsWaitingExecuted");
                return await _stockTaskRepository.UpdateAsync(entity, true);
            }
            catch (Exception ex)
            {
                Log.Error($"Box:{archiveBox.ArchiveBoxRfid} CreateStockIn is fail ErrorMsg: {ex.Message}");
                throw new UserFriendlyException(message: "创建入库任务失败");
            }

        }
        //分配库位
        [UnitOfWork]
        public async Task<bool> WCSSetCell(int StockTaskId)
        {
            var stockTask = await _stockTaskRepository.FindByIdAsync(StockTaskId);
            if (stockTask != null)
            {
                var box = await _archiveBoxManager.GetArchiveBoxByRfidCode(stockTask.ArchiveBoxRfid);
                if (stockTask.ManageStatus == ManageStatus.WaitingExecute)
                {
                    Cell startCell ;
                    Cell endCell;
                    if (stockTask.StartCellId == 0 )
                    {
                        //分配入库规格
                        //2024多个仓库在登陆界面获取仓库的Id?
                        startCell = await _cellManager.GetEmptyStation(1, box.CellModel);
                        if (stockTask.ManageTypeCode == ManageType.NpFullStockOut)
                        {
                            startCell = await _cellManager.GetEmptyCell(1, box.CellModel);
                        }
                        if (startCell == null)
                            throw new UserFriendlyException("没有空闲的柜格.");
                        stockTask.StartCellId = startCell.Id;
                        stockTask.StartCellCode = startCell.CellCode;
                    }
                    else
                    { 
                        //档案盒出库
                        startCell = await _cellManager.GetByIdAsync((int)stockTask.StartCellId);
                        stockTask.StartCellCode = startCell.CellCode;
                    }

                    if (stockTask.EndCellId == 0 || stockTask.EndCellId == null)
                    {
                        //优先分配上一个出库库位
                        if(stockTask.ManageTypeCode != ManageType.NPSortStockOut)
                        {
                            StockTask last = (await _stockTaskRepository.GetListAsync(x => x.ArchiveBoxRfid == stockTask.ArchiveBoxRfid & (x.ManageTypeCode == ManageType.NpFullStockOut || x.ManageTypeCode == ManageType.LossOut) & x.ManageStatus == ManageStatus.Complete)).OrderByDescending(o => o.Id).FirstOrDefault();
                            if (last != null)
                            {
                                endCell = await _cellManager.GetByIdAsync(last.StartCellId);
                            }
                            else
                            {
                                //分配入库库位
                                endCell = await _cellManager.GetEmptyCell(1, box.CellModel);
                            }
                        }
                        else
                        {
                            endCell = await _cellManager.GetEmptyStation(1, box.CellModel);
                        }
                        if (endCell == null)
                            throw new UserFriendlyException("没有空闲的柜格或库位.");
                        stockTask.EndCellId = endCell.Id;
                        stockTask.EndCellCode = endCell.CellCode;
                    }

                    stockTask.ManageStatus = ManageStatus.OrderCatched;
                    stockTask.ManageBeginTime = DateTime.Now.ToString();
                    //更新库位状态
                    await _cellManager.SetSelectedAsync((int)stockTask.StartCellId);
                    await _cellManager.SetSelectedAsync((int)stockTask.EndCellId);
                    //记录用户？
                    await _stockTaskRepository.UpdateAsync(stockTask, true);
                    var reqCode = StockTaskId.ToString();
                    //创建WCS任务
                    var result = await _wcsApiManager.StockOrderCreate(
                        reqCode,
                        box.ArchiveBoxRfid,
                        stockTask.StartCellCode,
                        stockTask.EndCellCode,
                        ToWcsTaskType(stockTask.ManageTypeCode),
                        1);
                    //Log.Debug("请求结果：" + result.Success + result.Message);
                    if(result == null)
                    {
                        //throw new UserFriendlyException("WCS服务未启用");
                        return false;
                    }
                    if (result.Success == false)
                    {
                        throw new UserFriendlyException(result.Message);
                    }

                    //记录日志
                    try
                    {
                        Log.Warning("用户" + "  下达了入库任务" + stockTask.Id + "  方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                        return true;
                    }
                    catch (Exception)
                    {
                        Log.Warning("系统后台下达了入库任务" + stockTask.Id + "  方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                        return true;
                    }
                }
                else
                {
                    throw new UserFriendlyException("任务状态为不可下达");
                }

            }
            else
            {
                throw new UserFriendlyException("任务不存在");
            }
        }

        //指定库位分配
        //public async

        //任务下达WCS
        public async Task<StockTask> StockDownloadIn(int StockTaskId)
        {
            return await _stockTaskRepository.FindAsync(StockTaskId);
        }
        //获取未完成任务
        public async Task<List<StockTask>> GetNoCompleteAsync()
        {
            return await _stockTaskRepository.GetListAsync(f =>
                f.ManageStatus != ManageStatus.Cancel &&
                f.ManageStatus != ManageStatus.Complete &&
                f.ManageStatus != ManageStatus.ExceptionComplete &&
                f.ManageStatus != ManageStatus.WaitingExecute);
        }

        public async Task<StockTask> CreateWCSOut(string manageTypeCode, ArchiveBox archiveBox)
        {
            //判断档案盒状态
            if (archiveBox.CellId == 0)
            {
                throw new UserFriendlyException(message: "档案盒不在库位");
            }
            //
            var startCell = await _cellManager.GetByIdAsync(archiveBox.CellId);
            //判断档案盒是否存在库位任务
            if (await ValidateStockManageExist(archiveBox.ArchiveBoxRfid))
            {
                throw new UserFriendlyException(message: "档案盒已存在出入库任务");
            }
            StockTask entity = null;
            try
            {
                entity = new StockTask(manageTypeCode, archiveBox.ArchiveBoxRfid, startCell.CellCode, startCell.Id);
                return await _stockTaskRepository.InsertAsync(entity, true);
                Log.Debug($"Task:{entity.Id} Box:{archiveBox.ArchiveBoxRfid} CreateStockIn Inserted StockTaskData: {JsonConvert.SerializeObject(entity)}");
                //entity.SetAsWaitingExecuted();

                //await SetAsExecutingAsync(entity.Id);
                //Log.Information($"Task:{entity.Id} Box:{storageBox.StorageBoxBarcode} CreateStockIn SetAsWaitingExecuted");
                //return await _stockTaskRepository.UpdateAsync(entity, true);
            }
            catch (Exception ex)
            {
                Log.Error($"Box:{archiveBox.ArchiveBoxRfid} CreateStockIn is fail ErrorMsg: {ex.Message}");
                throw new UserFriendlyException(message: "创建出库任务失败");
            }

        }
        //从WCS返回更新状态
        [UnitOfWork]
        public async Task<StockTask> UpdateStatusAsync(int stockTaskId, WcsTaskStatus status)
        {
            try
            {
                Log.Debug($"Task:{stockTaskId} 更新任务状态为 {status}。");
                var entity = await _stockTaskRepository.FindByIdAsync(stockTaskId);
                if (entity == null)
                    throw new UserFriendlyException(message: "出入库任务不存在或已完成");

                switch (status)
                {
                    case WcsTaskStatus.Unknown:
                        // 未知状态不改变库存和任务状态。
                        Log.Warning($"Task:{stockTaskId} 收到未知的 WCS 任务状态。");
                        return entity;

                    case WcsTaskStatus.Accepted:
                        // WCS 已受理，任务可能正在排队或等待资源。
                        entity.SetManageStatus(ManageStatus.OrderCatched);
                        break;

                    case WcsTaskStatus.Executing:
                        // WCS 已获得资源并开始执行设备动作。
                        entity.SetManageStatus(ManageStatus.Executing);
                        break;

                    case WcsTaskStatus.Completed:
                        // WCS 正常完成，按 WMS 任务类型提交库存变化。
                        if (entity.ManageTypeCode == ManageType.NPSortStockOut)
                        {
                            // 出库完成：释放起终点库位并将档案盒标记为出库。
                            await _cellManager.SetAsStockOutAsync((int)entity.EndCellId);
                            await _cellManager.SetAsStockOutAsync((int)entity.StartCellId);
                            await _archiveBoxManager.UpdateStockOutCellAsync(entity.ArchiveBoxRfid);
                            entity.SetAsCompleted();
                        }
                        else if (entity.ManageTypeCode == ManageType.NPFullStockIn)
                        {
                            // 入库完成：目标库位入库、起点释放并绑定档案盒新库位。
                            var endCell = await _cellManager.SetAsStockInAsync((int)entity.EndCellId);
                            await _cellManager.SetAsStockOutAsync((int)entity.StartCellId);
                            await _archiveBoxManager.UpdateStockCellAsync(entity.ArchiveBoxRfid, endCell.Id);
                            entity.SetAsCompleted();
                        }
                        else
                        {
                            // 尚未定义库存收尾的任务类型只记录告警，不擅自修改库存。
                            Log.Warning($"Task:{stockTaskId} 类型 {entity.ManageTypeCode} 尚未实现完成库存处理。");
                        }
                        break;

                    case WcsTaskStatus.Canceled:
                        // WCS 已取消任务，WMS 标记取消；资源释放逻辑后续按业务补充。
                        entity.SetAsCancel();
                        break;

                    case WcsTaskStatus.ForceCompleted:
                        // WCS 强制结束，实际库存位置需人工核对，不按正常完成提交库存。
                        entity.SetManageStatus(ManageStatus.ExceptionComplete);
                        break;

                    default:
                        Log.Warning($"Task:{stockTaskId} 收到未支持的 WCS 任务状态 {status}。");
                        return entity;
                }

                StockTask stockTaskRtn = await _stockTaskRepository.UpdateAsync(entity, true);
                return stockTaskRtn;
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }


        //创建盘点任务
        public async Task<StockTask> CreateCheckAsync(StockTask stockTask)
        {
            return await _stockTaskRepository.InsertAsync(stockTask, true);
        }
        //获取盘点任务
        public async Task<List<StockTask>>GetCheckList()
        {
            return await _stockTaskRepository.GetListAsync(x => x.ManageTypeCode == ManageType.HpAnnualCheckDown);
        }

        /// <summary>
        /// 标记单库位盘点执行任务完成。
        /// 此方法只结束任务生命周期，不修改档案盒绑定和库位库存状态；
        /// 盘盈、盘亏、错位等差异必须由 WMS 审核后通过独立业务流程处理。
        /// </summary>
        public async Task<StockTask> CompleteCheckTaskAsync(int stockTaskId)
        {
            StockTask entity = await _stockTaskRepository.FindByIdAsync(stockTaskId);
            if (entity == null)
                throw new UserFriendlyException("盘点任务不存在或已完成");

            entity.ManageEndTime = DateTime.Now.ToString();
            entity.SetAsCompleted();
            return await _stockTaskRepository.UpdateAsync(entity, true);
        }

        //WCS回调接口
        public async Task<ResultWcsTaskDto> WcsCallBack(WcsCallBackRequest input)
        {
            if (!int.TryParse(input.OrderCode, out var stockTaskId))
                return new ResultWcsTaskDto(false, "订单号格式不正确");

            var stockTask = await FindByIdAsync(stockTaskId);
            if (stockTask == null)
                return new ResultWcsTaskDto(false, "任务不存在或已完成");

            if (stockTask.ManageTypeCode == ManageType.HpAnnualCheckDown)
            {
                // 盘点任务使用独立的盘点结果确认流程，不在普通库存状态处理器中提交库存。
                return new ResultWcsTaskDto(true, "盘点任务状态已接收");
            }

            await UpdateStatusAsync(stockTaskId, input.Status);
            return new ResultWcsTaskDto(true, "任务状态已更新");

        }

        //计划任务结果处理
        public async Task PlanResults(int stockId, string plateCode)
        {
            var entity = await _stockTaskRepository.FindByIdAsync(stockId);
            if (entity == null)
               throw new UserFriendlyException(message: "出入库任务不存在或已完成");
            //WCS盘点执行完成
            if (plateCode != "empty")
            {
                 //设置库位状态
                 var endCell = await _cellManager.SetAsStockInAsync((int)entity.EndCellId);
                 await _archiveBoxManager.UpdateStockCellAsync(plateCode, endCell.Id);
                 entity.ArchiveBoxRfid = plateCode;

            }
            
            entity.SetAsCompleted();
            await _stockTaskRepository.UpdateAsync(entity, true);

        }

        //执行计划
        public async Task<bool> ExecutePlan(Plan plan)
        {
            //?判断是否存在出入库任务
            if (await ExistInOutManage())
            {
                throw new UserFriendlyException("计划下达过程中不允许有出入库任务!");
            }
            try
            {
                if (plan.PlanStatus != PlanStatus.Waiting)
                {
                    throw new UserFriendlyException("计划不能重复下达!");
                }
                //获取区域所在的库位列表
                List<Cell> cells = await _cellManager.GetCellsByAreaCode(plan.AreaCode);
                CheckOrderCreateDto checkOrderCreate = new()
                {
                    Priority = 1,
                    Orders = new(),
                };
                if (cells.Count > 0)
                {
                    if (plan.PlanTypeCode == "Battest")
                    {
                        for (int i = 0; i < cells.Count; i++)
                        {
                            var s = await CreateBatTest(cells[i].CellCode);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < cells.Count; i++)
                        {
                            //下达任务并执行
                            var stock = await CreatePlanStock(cells[i].Id, plan.Id, plan.PlanTypeCode, cells[i].CellCode);
                            OrderDto order = new();
                            order.OrderCode = stock.Id.ToString();
                            order.CellCode = stock.EndCellCode;
                            checkOrderCreate.Orders.Add(order);
                        }
                        var req = await _wcsApiManager.CheckOrderCreate(checkOrderCreate);
                        plan.HdDefineStr1 = req.QueryCode; 
                    }
                    plan.PlanBeginTime = DateTime.Now.ToString();
                    plan.PlanStatus = PlanStatus.Executing;
                    await _planManager.Update(plan);
                }
                return true;
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException("计划下达过程失败!"+  ex.ToString());
            }
            
        }

        //创建计划任务
        [UnitOfWork]
        public async Task<StockTask> CreatePlanStock(int cellId ,int planId, string planType , string cellCode)
        {
            StockTaskDto stockTask = new();
            try
            {
                var archiveBox = await _archiveBoxManager.GetArchiveBoxByCellId(cellId);
                if (archiveBox != null)
                {
                    stockTask.ArchiveBoxRfid = archiveBox.ArchiveBoxRfid;
                }
                else
                {
                    stockTask.ArchiveBoxRfid = "";
                }

                stockTask.PlanId = planId;
                stockTask.PlanTypeCode = planType;
                stockTask.EndCellId = cellId;
                stockTask.ManageTypeCode = ManageType.HpAnnualCheckDown;
                stockTask.ManageStatus = ManageStatus.OrderCatched;
                stockTask.StartCellId = cellId;
                stockTask.StartCellCode = cellCode;
                stockTask.EndCellCode = cellCode;

                //增加操作者ID
            }
            catch
            {
                throw new UserFriendlyException("ManageCreateCheckByCell异常");
            }
            if (await ValidateStockManageExist(stockTask.ArchiveBoxRfid))
            {
                throw new UserFriendlyException("档案盒已存在任务");
            }
            var stock = base.ObjectMapper.Map<StockTaskDto, StockTask>(stockTask);
            var st = await CreateCheckAsync(stock);

            //创捷计划明细
            //await CreateCheckList(checkId, st);

            //锁定库位
            if (stockTask.StartCellId != 0)
            {
                await _cellManager.SetSelectedAsync(stockTask.StartCellId);
            }

            //自动执行盘点任务
            //await CheckDownLoadAsync(st.Id , cellCode);

            //添加工作单元、事务处理 
            //await CurrentUnitOfWork.SaveChangesAsync();
            return stock;
        }

        //取消执行中的计划任务
        public async Task<bool> CancelExecutingPlan(int planId)
        {
            var stocks = await _stockTaskRepository.GetListAsync(f => f.PlanId == planId);
            if(stocks != null)
            {
                foreach (var stock in stocks)
                {
                    var s = await _stockTaskRepository.FindByIdAsync(stock.Id);
                    s.SetAsCancel();
                    await _stockTaskRepository.UpdateAsync(s);
                }
            }
            return true;
        }
        //创建测试任务
        public async Task<StockTask> CreateBatTest(string endcell)
        {
            //var entity = new StockTask(manageTypeCode, archiveBox.ArchiveBoxRfid);
            //return await _stockTaskRepository.InsertAsync(entity, true);

            return null;
        }

    }
}
