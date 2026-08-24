using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Serilog;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.Archives;
using WarehouseManagement.Cells;
using WarehouseManagement.Goodss;
using WarehouseManagement.Plans;
using WarehouseManagement.Plans.Aggregates;
using WarehouseManagement.Plans.Dto;
using WarehouseManagement.RfidCodes.Aggregates;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.TaskHiss.Aggregates;
using WarehouseManagement.WcsTasks;
using WarehouseManagement.WcsTasks.Dto;

namespace WarehouseManagement.StockTasks
{
    //[Authorize(WarehouseManagementPermissions.StockTaskManagement.Default)]
    public class StockTaskAppService : WarehouseManagementAppService,
         IStockTaskAppService //implement the IStockTaskAppService
    {
        //private readonly IRepository<StockTask, Guid> _stockTaskRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly IStockTaskRepository _stockTaskRepository;
        private readonly StockTaskManager _stockTaskManagement;
        private readonly PlanManager _planManager;
        private readonly CellManager _cellManager;
        private readonly WcsApiManager _wcsApiManager;
        private readonly ArchiveBoxManager _archiveBoxManager;
        private readonly IStockTaskDetailRepository _stockTaskDetailRepository;
        private readonly IGoodsRepository _goodsRepository;
        private readonly ICellRepository _cellRepository;
        private readonly IArchiveBoxRepository _archiveBoxRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IArchiveRepository _archiveRepository;

        public StockTaskAppService(IStockTaskRepository stockTaskRepository, StockTaskManager stockTaskManagement, 
            PlanManager planManager,
        IStockTaskDetailRepository stockTaskDetailRepository, IGoodsRepository goodsRepository, ICellRepository cellRepository,
            IArchiveBoxRepository archiveBoxRepository, CellManager cellManager, IPlanRepository planRepository,
            WcsApiManager wcsApiManager, ArchiveBoxManager archiveBoxManager,IArchiveRepository archiveRepository, UnitOfWorkManager unitOfWorkManager)
        {
            _stockTaskRepository = stockTaskRepository;
            _stockTaskManagement = stockTaskManagement;
            _planManager = planManager;
            _stockTaskDetailRepository = stockTaskDetailRepository;
            _goodsRepository = goodsRepository;
            _cellRepository = cellRepository;
            _archiveBoxRepository = archiveBoxRepository;
            _cellManager = cellManager;
            _planRepository = planRepository;
            _wcsApiManager = wcsApiManager;
            _archiveBoxManager = archiveBoxManager;
            _archiveRepository = archiveRepository;
        }


       


        public async Task<PagedResultDto<StockTaskDto>> GetPagingListAsync(PagingStockTaskListInput input)
        {
            //Get the IQueryable<Book> from the repository
            var queryable = await _stockTaskRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from stockTask in queryable
                        where stockTask.CreationTime >= input.StartCreationTime & stockTask.CreationTime <= input.EndCreationTime
                        & stockTask.ArchiveBoxRfid.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim())
                        //& stockTask.ManageStatus.ToString().Contains(input.ManageStatus=="All"?"":input.ManageStatus)
                        & (input.ManageStatus == "All" ? 1 == 1 : stockTask.ManageStatus == Enum.Parse<ManageStatus>(input.ManageStatus))
                        select new { stockTask };

            //Paging
            query = query
                //.OrderBy(NormalizeSorting(input.Sorting))
                .OrderByDescending(f => f.stockTask.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var stockTaskDtos = queryResult.Select(x =>
            {
                var stockTaskDtos = ObjectMapper.Map<StockTask, StockTaskDto>(x.stockTask);
                //stockTaskDtos.StartCellCode = x.scell?.CellCode;
                //stockTaskDtos.EndCellCode = x.ecell?.CellCode;
                return stockTaskDtos;
            }).ToList();

            //Get the total count with another query
            //var totalCount = await _stockTaskDetailRepository.GetCountAsync();
            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<StockTaskDto>(
                totalCount,
                stockTaskDtos
            );
        }

        public async Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListAsync(
    PagingStockTaskDetailInput input)
        {
            //Get the IQueryable<Book> from the repository
            var queryable = await _stockTaskDetailRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from stockTaskDetail in queryable
                        join goods in await _goodsRepository.GetQueryableAsync() on stockTaskDetail.GoodsId equals goods.Id
                        join stockTask in await _stockTaskRepository.GetQueryableAsync() on stockTaskDetail.StockTaskId equals stockTask.Id
                        where stockTaskDetail.StockTaskId == input.StockTaskId
                        select new { stockTaskDetail, goods, stockTask };

            //Paging
            query = query
                //.OrderBy(NormalizeSorting(input.Sorting))
                .OrderBy(f => f.stockTaskDetail.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var stockTaskDetailDtos = queryResult.Select(x =>
            {
                var stockTaskDetailDtos = ObjectMapper.Map<StockTaskDetail, StockTaskDetailDto>(x.stockTaskDetail);
                stockTaskDetailDtos.StockBarcode = x.stockTask.ArchiveBoxRfid;
                stockTaskDetailDtos.GoodsCode = x.goods.GoodsCode;
                stockTaskDetailDtos.GoodsName = x.goods.GoodsName;
                stockTaskDetailDtos.GoodsSpec = x.goods.GoodsSpec;
                stockTaskDetailDtos.GoodsBand = x.goods.GoodsConstProperty1;
                stockTaskDetailDtos.GoodsUnits = x.goods.GoodsUnits;
                stockTaskDetailDtos.Quantity = x.stockTaskDetail.ManageListQuantity;

                return stockTaskDetailDtos;
            }).ToList();

            //Get the total count with another query
            //var totalCount = await _stockTaskDetailRepository.GetCountAsync();
            var totalCount = queryResult.Count();

            return new PagedResultDto<StockTaskDetailDto>(
                totalCount,
                stockTaskDetailDtos
            );
        }

        public async Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListByArchiveIdAsync(
    PagingStockTaskDetailInput input)
        {
            //Get the IQueryable<Book> from the repository
            var queryable = await _stockTaskDetailRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from stockTaskDetail in queryable
                        join archive in await _archiveRepository.GetQueryableAsync() on input.ArchiveId equals archive.Id
                        where stockTaskDetail.GoodsId == input.ArchiveId
                        select new { stockTaskDetail, archive };

            //Paging
            query = query
                //.OrderBy(NormalizeSorting(input.Sorting))
                .OrderBy(f => f.stockTaskDetail.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var stockTaskDetailDtos = queryResult.Select(x =>
            {
                var stockTaskDetailDtos = ObjectMapper.Map<StockTaskDetail, StockTaskDetailDto>(x.stockTaskDetail);
                stockTaskDetailDtos.GoodsCode = x.archive.ArchivesCode;
                stockTaskDetailDtos.GoodsName = x.archive.ArchivesName;
                stockTaskDetailDtos.GoodsSpec = x.stockTaskDetail.Borrower;

                return stockTaskDetailDtos;
            }).ToList();

            //Get the total count with another query
            //var totalCount = await _stockTaskDetailRepository.GetCountAsync();
            var totalCount = queryResult.Count();

            return new PagedResultDto<StockTaskDetailDto>(
                totalCount,
                stockTaskDetailDtos
            );
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[Authorize(WarehouseManagementPermissions.StockTaskManagement.Update)]
        public virtual async Task<StockTaskDto> UpdateAsync(UpdateStockTaskDto input)
        {
            var stockTask = await _stockTaskManagement.UpdateAsync(input.Id, input.ManageTypeCode, input.StockBarcode, input.StartCellId, input.EndCellId);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        //[Authorize(WarehouseManagementPermissions.StockTaskManagement.Delete)]
        public virtual async Task DeleteAsync(IdIntInput input)
        {
            await _stockTaskManagement.DeleteAsync(input.Id);
            //await _stockTaskRepository.DeleteAsync(input.Id);
        }
        
        public async Task<StockTaskDto> SetAsCancelAsync(IdIntInput input)
        {
            var stockTask = await _stockTaskManagement.SetAsCancelAsync(input.Id);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
        }

        public async Task<bool> PickOutTask(List<PickOutDto> input)
        {
            if (input.Count == 0)
                return false;
            if (input[0].Userid == 0)
                return false;
            List<PickOutDto> lists = input.OrderBy(a => a.ArchiveBoxId).ToList();
            int stgId = 0;
            List<StockTaskDetail> stockTaskDetails = new ();
            foreach (PickOutDto pickOut in lists)
            {
                if (stgId != pickOut.ArchiveBoxId & stgId != 0)
                {

                    await _stockTaskManagement.ManageCreateOut(stgId, stockTaskDetails, input[0].Userid);
                    stockTaskDetails.Clear();
                }
                StockTaskDetail stockTaskDetail = new (
                   pickOut.ArchiveId
                );
                stockTaskDetails.Add(stockTaskDetail);
                stgId = pickOut.ArchiveBoxId;
            }
            await _stockTaskManagement.ManageCreateOut(stgId, stockTaskDetails, input[0].Userid);
            return true;
        }


        //创建档案入库任务
        public async Task<StockTaskDto> CreateWCSIn(CreateStockTaskDto input)
        {
            ArchiveBox archiveBoxObj;
            if (input.ArchiveBoxId != 0)
            {
                archiveBoxObj = await _archiveBoxRepository.FindByIdAsync(input.ArchiveBoxId);
            }
            else
            {
                archiveBoxObj = await _archiveBoxRepository.FindByArchiveBoxcodeAsync(input.ArchiveCode);
            }
            if (archiveBoxObj.CellModel == null)
            {
                throw new UserFriendlyException("档案盒未设置尺寸");
            }
            if (archiveBoxObj.ArchiveBoxRfid == null)
            {
                throw new UserFriendlyException("档案盒未绑定标签");
            }
            input.ManageTypeCode = ManageType.NPFullStockIn.ToString();
            // var stockTask = await _stockTaskManagement.CreateStockInAsync(input.ManageTypeCode, storageBoxObj, storageBoxObj.Details, input.StartCellCode, input.EndCellId);
            var stockTask = await _stockTaskManagement.CreateWCSIn(input.ManageTypeCode, archiveBoxObj);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
        }
        //下达档案任务分配库位
        public async Task<Boolean> WCSSetCell(int StockTaskId)
        {
            var stockTask = await _stockTaskManagement.WCSSetCell(StockTaskId);
            return stockTask;
        }
        //扫码打开柜门,创建任务
        public async Task<StockTaskDto> OpenDoorAndWCSInExcute(int input)
        {

            var stockTask = await _stockTaskManagement.StockDownloadIn(input);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
        }
        //一体机扫码档案盒rfid下达wcs任务打开柜门
        [UnitOfWork]
        public async Task<bool> TaskAssignUseRfid(string rfid)
        {
            CreateStockTaskDto stockTaskDto = new();
            OpenDoorDto openDoorDto = new();
            //找到档案盒
            var box = await _archiveBoxRepository.FindByRfidCodeAsync(rfid);
            if (box == null)
            {
                throw new UserFriendlyException("档案盒不存在!!");
            }
            else
            {
                stockTaskDto.ArchiveBoxId = box.Id;
            }

            //创建任务
            var stock = await CreateWCSIn(stockTaskDto);
            if (stock == null)
            {
                throw new UserFriendlyException(message: "创建任务失败");
            }
            //分配库位
            await WCSSetCell(stock.Id);


            //开柜门
            openDoorDto.OrderCode = stock.Id.ToString();
            await _wcsApiManager.OpenDoorForOrder(openDoorDto);

            return true;
        }
        //一体机扫码档案盒rfid下达wcs任务打开柜门
        [UnitOfWork]
        public async Task<bool> ClientOutCell(string rfid)
        {

            CreateStockTaskDto stockTaskDto = new();
            //找到档案盒
            var box = await _archiveBoxRepository.FindByRfidCodeAsync(rfid);
            if (box == null)
            {
                throw new UserFriendlyException(message: "档案盒不存在!");
            }
            else
            {
                stockTaskDto.ArchiveBoxId = box.Id;
            }
            //创建任务
            var stock = await CreateWCSOut(stockTaskDto);
            if (stock == null)
            {
                throw new UserFriendlyException(message: "创建任务失败");
            }
            //分配库位
            await WCSSetCell(stock.Id);


            return true;

        }
        //创建档案出库任务
        public async Task<StockTaskDto> CreateWCSOut(CreateStockTaskDto input)
        {
            ArchiveBox archiveBoxObj;
            if (input.ArchiveBoxId != 0)
            {
                archiveBoxObj = await _archiveBoxRepository.FindByIdAsync(input.ArchiveBoxId);
            }
            else
            {
                archiveBoxObj = await _archiveBoxRepository.FindByArchiveBoxcodeAsync(input.ArchiveCode);
            }
            input.ManageTypeCode = ManageType.NPSortStockOut.ToString();
            // var stockTask = await _stockTaskManagement.CreateStockInAsync(input.ManageTypeCode, storageBoxObj, storageBoxObj.Details, input.StartCellCode, input.EndCellId);
            var stockTask = await _stockTaskManagement.CreateWCSOut(input.ManageTypeCode, archiveBoxObj);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
        }
        public async Task<bool> BatBoxInByArea(string areaCode)
        {
            List<int> cellIds = await _cellManager.GetCellidsByAreaCode(areaCode);
            //增加了对CELL进行排序
            List<int> newcellIds = await _cellManager.OrderCellidsByIds(cellIds);
            return await ManageCreateBatIn(newcellIds);
        }

        [UnitOfWork]
        public async Task<bool> ManageCreateBatIn(List<int> cellIds)
        {
            //step1 该是否存在任务
            var stockCount = await _stockTaskRepository.GetListAsync();
            if (stockCount.Count > 0)
            {
                throw new UserFriendlyException("存在出入库任务，请先执行完其它任务。");
            }

            //step2创建计划
            PlanDto planMain = new PlanDto();
            DateTime.Now.Ticks.ToString();
            planMain.PlanCode = "批量入库" + DateTime.Now.Ticks.ToString();
            planMain.PlanExecuteType = PlanExecuteType.Automatic;
            planMain.PlanStatus = PlanStatus.Waiting;
            planMain.PlanTypeCode = PlanTypeInout.In.ToString();
            var entity = base.ObjectMapper.Map<PlanDto, Plan>(planMain);
            var plan =await _planRepository.InsertAsync(entity);
            //step3 检查库位是否存在货物 、创建入库任务
            CheckOrderCreateDto checkOrderCreate = new()
            {
                Priority = 1,
                Orders = new(),
            };
            foreach (int cId in cellIds)
            {
                Cell cell = await _cellManager.GetByIdAsync(cId);
                if (cell is null)
                {
                    throw new UserFriendlyException(cell.CellCode + "库位数据错误，请校核。");
                }
                else if (cell.CellStatus == CellStatus.Full)
                {
                    throw new UserFriendlyException(cell.CellCode + "库位错误，已存在档案，请校核。");
                }
                else
                {
                    StockTaskDto mainObj = new StockTaskDto();
                    mainObj.StartCellId = cId;
                    mainObj.EndCellId = cId;
                    mainObj.EndCellCode = cell.CellCode;
                    mainObj.PlanId = plan.Id;
                    mainObj.ManageTypeCode = ManageType.HPBatchStockIn;
                    mainObj.ManageStatus = ManageStatus.Executing;
                    var st = base.ObjectMapper.Map<StockTaskDto, StockTask>(mainObj);
                    var stock = await _stockTaskManagement.CreateCheckAsync(st);

                    //锁库位
                    await _cellManager.SetSelectedAsync(cId);

                    OrderDto order = new();
                    order.OrderCode = stock.Id.ToString();
                    order.CellCode = stock.EndCellCode;
                    checkOrderCreate.Orders.Add(order);
                }

            }
            var req = await _wcsApiManager.CheckOrderCreate(checkOrderCreate);
            plan.HdDefineStr1 = req.QueryCode;
            await _planRepository.UpdateAsync(plan);

            //添加工作单元、事务处理
            await CurrentUnitOfWork.SaveChangesAsync();
            //20240122记录日志
            Log.Debug("用户创建了批量入库计划，ID:" + plan.ToString() + "  方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }

        [UnitOfWork]
        public async Task<List<StockTaskDto>> GetInOutTask()
        {
            var manageMainlist =await _stockTaskRepository.GetListAsync(a => a.ManageTypeCode == ManageType.NPFullStockIn || a.ManageTypeCode == ManageType.NPSortStockOut);
            List<StockTaskDto> listResultDtos = new();
            foreach (var item in manageMainlist)
            {
                var stockTaskDtos = ObjectMapper.Map<StockTask, StockTaskDto>(item);

                listResultDtos.Add(stockTaskDtos);
            }

            return listResultDtos;
        }

        //一体机打开柜门
        public async Task ControlDoorOpen(int stockId)
        {
            StockTask s  = await _stockTaskManagement.FindByIdAsync(stockId);
            if(s != null)
            {
                //s.StartCellCode
                OpenDoorDto openDoorDto = new (){
                    OrderCode = stockId.ToString(),
                };
                //通知Wcs打开柜门
                await _wcsApiManager.OpenDoorForOrder(openDoorDto);
            }
            else
            {
                throw new UserFriendlyException("任务不存在");
            }
        }
        //一体机任务自动分配
        public async Task TaskAssign(int stockId)
        {
            await WCSSetCell(stockId);
        }
        //疲劳测试
        public async Task CreateBatTest()
        {
            CreateStockTaskDto stockTaskDto = new();
            //找到档案盒
            var box = await _archiveBoxRepository.GetListAsync(x => x.CellId > 5);
            if (box.Count == 0)
            {
                throw new UserFriendlyException(message: "档案盒不存在!");
            }
            else
            {
                stockTaskDto.ArchiveBoxId = box[0].Id;
            }
            //创建任务
            var stock = await CreateWCSOut(stockTaskDto);
            if (stock == null)
            {
                throw new UserFriendlyException(message: "创建任务失败");
            }
            //分配库位
            await WCSSetCell(stock.Id);
        }
        //任务异常强制完成
        public async Task ForceComplete(int stockId)
        {
            await WCSSetCell(stockId);
            var mge = await _stockTaskManagement.FindByIdAsync(stockId);
            
            if (mge.ManageTypeCode == ManageType.HpAnnualCheckDown)
            {
                //_storageManager.UnLockCell(mge.StartCellId);
                //_storageManager.UnLockCell(mge.EndCellId);
                //设置库位状态
                var endCell = await _cellManager.SetAsStockOutAsync((int)mge.EndCellId);
                var startCell = await _cellManager.SetAsStockOutAsync((int)mge.StartCellId);
                //更新料箱的库位状态
                var box = await _archiveBoxManager.UpdateStockCellAsync(mge.ArchiveBoxRfid, endCell.Id);
            }
            else if (mge.ManageTypeCode == ManageType.NpFullStockOut)
            {
                //出库时 库存处理
                var endCell = await _cellManager.SetAsStockOutAsync((int)mge.EndCellId);
                var startCell = await _cellManager.SetAsStockOutAsync((int)mge.StartCellId);
                //CompleteHandleCellOut(mge.StartCellId, mge.EndCellId);
                var box = await _archiveBoxManager.UpdateStockOutCellAsync(mge.ArchiveBoxRfid);
            }
            else if (mge.ManageTypeCode == ManageType.HPBatchStockIn)
            {
                //设置库位状态
                var endCell = await _cellManager.SetAsStockOutAsync((int)mge.EndCellId);
                var startCell = await _cellManager.SetAsStockOutAsync((int)mge.StartCellId);
                //批量上架时将异常的库位未NOHAVING   识别异常的需要取出档案
                // CompleteHandleCellOut(mge.StartCellId, mge.EndCellId);
            }
            else
            {
                //库位解锁以及库位状态的变更   盘点时不变更库位状态
                //设置库位状态
                var endCell = await _cellManager.SetAsStockOutAsync((int)mge.EndCellId);
                var startCell = await _cellManager.SetAsStockOutAsync((int)mge.StartCellId);
                //CompleteHandleCell(mge.StartCellId, mge.EndCellId);
            }

            List<int> ids = new();
            ids.Add(stockId);
            //wcs强制完成任务
            await WcsForceComplete(ids);


            //更新任务状态
            mge.ManageEndTime = DateTime.Now.ToString();
            mge.ManageStatus = ManageStatus.Complete;
            await _stockTaskRepository.UpdateAsync(mge);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        //Wcs强制完成
        public async Task WcsForceComplete(List<int> ids)
        {
            //暂停执行
            await _wcsApiManager.Pause();

            //强制完成
            if(ids.Count != 0)
            {
                foreach (int id in ids)
                {
                    await _wcsApiManager.CancelOrder(id);
                }
            }

            //恢复执行
            await _wcsApiManager.Restart();
        }









    }
}
