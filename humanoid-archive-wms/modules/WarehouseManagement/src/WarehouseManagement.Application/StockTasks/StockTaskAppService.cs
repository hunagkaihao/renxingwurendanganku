using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Serilog;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Uow;
using WarehouseManagement.MaterialBoxs;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.Material;
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
        private readonly MaterialBoxManager _materialBoxManager;
        private readonly IStockTaskDetailRepository _stockTaskDetailRepository;
        private readonly IGoodsRepository _goodsRepository;
        private readonly ICellRepository _cellRepository;
        private readonly IMaterialBoxRepository _materialBoxRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IDataFilter _dataFilter;

        public StockTaskAppService( IStockTaskRepository stockTaskRepository, StockTaskManager stockTaskManagement, 
                                    PlanManager planManager,IStockTaskDetailRepository stockTaskDetailRepository, 
                                    IGoodsRepository goodsRepository, ICellRepository cellRepository,
                                    IMaterialBoxRepository materialBoxRepository, CellManager cellManager, 
                                    IPlanRepository planRepository,WcsApiManager wcsApiManager, 
                                    MaterialBoxManager materialBoxManager,IMaterialRepository materialRepository, 
                                    UnitOfWorkManager unitOfWorkManager, IDataFilter dataFilter)
        {
            _stockTaskRepository = stockTaskRepository;
            _stockTaskManagement = stockTaskManagement;
            _planManager = planManager;
            _stockTaskDetailRepository = stockTaskDetailRepository;
            _goodsRepository = goodsRepository;
            _cellRepository = cellRepository;
            _materialBoxRepository = materialBoxRepository;
            _cellManager = cellManager;
            _planRepository = planRepository;
            _wcsApiManager = wcsApiManager;
            _materialBoxManager = materialBoxManager;
            _materialRepository = materialRepository;
            _dataFilter = dataFilter;
        }
        
        /// <summary>
        /// 获取页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<StockTaskDto>> GetPagingListAsync(PagingStockTaskListInput input)
        {
            using var disableSoftDeleteFilter = _dataFilter.Disable<ISoftDelete>();

            // 获取任务表
            var queryable = await _stockTaskRepository.GetQueryableAsync();

            // 筛选满足条件项
            var query = from stockTask in queryable
                        where stockTask.CreationTime >= input.StartCreationTime & 
                              stockTask.CreationTime <= input.EndCreationTime & 
                              stockTask.MaterialBoxBarcode.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim()) &
                              (!input.HideCompletedTasks ||
                               (stockTask.TaskStatus != TaskStatus.Cancel &
                                stockTask.TaskStatus != TaskStatus.Complete &
                                stockTask.TaskStatus != TaskStatus.ExceptionComplete)) &
                              (input.TaskStatus == "All" ? 1 == 1 : stockTask.TaskStatus == Enum.Parse<TaskStatus>(input.TaskStatus))
                              select new { stockTask };

            // 降序排序
            query = query .OrderByDescending(f => f.stockTask.Id)
                          .Skip(input.SkipCount)
                          .Take(input.PageSize);

            // 执行查询获取列表
            var queryResult = await AsyncExecuter.ToListAsync(query);

            // 转换查询结构为列表对象
            var stockTaskDtos = queryResult.Select(x =>
            {
                var stockTaskDtos = ObjectMapper.Map<StockTask, StockTaskDto>(x.stockTask);
                return stockTaskDtos;
            }).ToList();
            
            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<StockTaskDto>(totalCount, stockTaskDtos);
        }

        public async Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListAsync(PagingStockTaskDetailInput input)
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
                stockTaskDetailDtos.StockBarcode = x.stockTask.MaterialBoxBarcode;
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

        public async Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListByMaterialIdAsync(PagingStockTaskDetailInput input)
        {
            //Get the IQueryable<Book> from the repository
            var queryable = await _stockTaskDetailRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from stockTaskDetail in queryable
                        join Material in await _materialRepository.GetQueryableAsync() on input.MaterialId equals Material.Id
                        where stockTaskDetail.GoodsId == input.MaterialId
                        select new { stockTaskDetail, Material };

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
                stockTaskDetailDtos.GoodsCode = x.Material.MaterialCode;
                stockTaskDetailDtos.GoodsName = x.Material.MaterialName;
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
        
        /// <summary>
        ///  创建物料入库预约任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [UnitOfWork]
        public async Task<StockTaskDto> CreateWCSIn(CreateStockTaskDto input)
        {
            if (!DateTime.TryParseExact(
                    input.MaterialCreateTime,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var materialCreateTime))
            {
                throw new UserFriendlyException("创建时间格式必须为 yyyy-MM-dd HH:mm:ss");
            }

            // 每次预约均按传入物料创建容器记录；不查询或校验既有容器状态。
            var materialBoxObj = new MaterialBox(input.MaterialName, input.MaterialCode)
            {
                MaterialBoxBarcode = input.MaterialCode,
                CellModel = input.MaterialType,
                MaterialUnit = input.MaterialUnit,
                RetentionPeriod = input.ValidityDays.ToString(CultureInfo.InvariantCulture),
                MaterialPeople = input.CreatorUserCode,
                MaterialInDate = materialCreateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                CreationTime = materialCreateTime
            };
            materialBoxObj = await _materialBoxRepository.InsertAsync(materialBoxObj, true);

            // 设置任务类型
            input.TaskTypeCode = TaskType.NPFullStockIn.ToString();
            
            // 创建入库任务
            var stockTask = await _stockTaskManagement.CreateWCSIn(input.TaskTypeCode, materialBoxObj);
            
            // 放回结果给前端
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
        }
        /// <summary>
        /// 一体机扫描物料条码分配库位下发入库任务
        /// </summary>
        /// <param name="materialBoxBarcode">物料条码</param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [UnitOfWork]
        public async Task<bool> ScanAndDispatchToWCS(string materialBoxBarcode)
        {
            OpenDoorDto openDoorDto = new();
            // 查找对应物料容器
            var box = await _materialBoxRepository.FindByRfidCodeAsync(materialBoxBarcode);
            if (box == null)
            {
                throw new UserFriendlyException("物料容器不存在!!!");
            }

            if (string.IsNullOrWhiteSpace(box.CellModel))
            {
                throw new UserFriendlyException("物料容器未设置物料类型，无法分配库位!");
            }
            
            // 根据物料码查找对应物料任务
            var stockTask = await _stockTaskRepository.FindByBarcodeAsync(materialBoxBarcode);
            if (stockTask == null)
            {
                throw new UserFriendlyException("预约入库任务不存在,请重新预约入库任务.");
            }
            // 分配物料库位并下发任务给RCS
            await WCSSetCell(stockTask.Id);
            
            // 开柜门
            openDoorDto.OrderCode = stockTask.Id.ToString();
            await _wcsApiManager.OpenDoorForOrder(openDoorDto);

            return true;
        }
        
        /// <summary>
        /// 下达物料任务分配库位
        /// </summary>
        /// <param name="StockTaskId"></param>
        /// <returns></returns>
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
            OpenDoorDto openDoorDto = new();
            //找到档案盒
            var box = await _materialBoxRepository.FindByRfidCodeAsync(rfid);
            if (box == null)
            {
                throw new UserFriendlyException("档案盒不存在!!");
            }
            // 一体机流程使用已扫描到的容器创建任务，不走外部预约接口的物料建档参数校验。
            var stockTask = await _stockTaskManagement.CreateWCSIn(
                TaskType.NPFullStockIn.ToString(),
                box);
            var stock = base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
            if (stock == null)
            {
                throw new UserFriendlyException(message: "创建任务失败");
            }
            //分配库位并下发任务给WCS
            await WCSSetCell(stock.Id);
            
            //开柜门
            openDoorDto.OrderCode = stock.Id.ToString();
            await _wcsApiManager.OpenDoorForOrder(openDoorDto);

            return true;
        }
        
        //一体机扫码档案盒rfid下达wcs任务打开柜门
        [UnitOfWork]
        public async Task<StockTaskDto> ClientOutCell(ClientOutCellInput input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(message: "请求参数不能为空!");
            }

            var materialCode = input.MaterialCode;
            var cellCode = input.CellCode;

            if (string.IsNullOrWhiteSpace(materialCode) && string.IsNullOrWhiteSpace(cellCode))
            {
                throw new UserFriendlyException(message: "物料码和库位不能同时为空!");
            }

            Cell cell = null;
            if (!string.IsNullOrWhiteSpace(cellCode))
            {
                cell = await _cellRepository.FindByCodeAsync(cellCode.Trim());
                if (cell == null)
                {
                    throw new UserFriendlyException(message: "库位不存在!");
                }

                if (string.IsNullOrWhiteSpace(cell.MaterialCode))
                {
                    throw new UserFriendlyException(message: "库位无货，无法创建出库任务!");
                }

                if (!string.IsNullOrWhiteSpace(materialCode) &&
                    !string.Equals(cell.MaterialCode, materialCode.Trim(), StringComparison.Ordinal))
                {
                    throw new UserFriendlyException(message: "输入库位中的物料码与输入物料码不一致，出库任务下发失败!");
                }

                materialCode = cell.MaterialCode;
            }

            CreateStockTaskDto stockTaskDto = new();
            var box = await _materialBoxRepository.FindByMaterialBoxcodeAsync(materialCode.Trim());
            if (box == null)
            {
                throw new UserFriendlyException(message: "物料不存在!");
            }

            if (cell != null && box.CellId != cell.Id)
            {
                throw new UserFriendlyException(message: "物料容器所在库位与输入库位不一致，出库任务下发失败!");
            }

            stockTaskDto.MaterialBoxId = box.Id;
            //创建任务
            var stock = await CreateWCSOut(stockTaskDto);
            if (stock == null)
            {
                throw new UserFriendlyException(message: "创建任务失败");
            }
            //分配库位
            await WCSSetCell(stock.Id);

            var updatedStockTask = await _stockTaskManagement.FindByIdAsync(stock.Id);
            if (updatedStockTask == null)
            {
                throw new UserFriendlyException(message: "任务不存在");
            }

            return base.ObjectMapper.Map<StockTask, StockTaskDto>(updatedStockTask);
        }
        
        /// <summary>
        /// 创建物料出库任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<StockTaskDto> CreateWCSOut(CreateStockTaskDto input)
        {
            MaterialBox materialBoxObj;
            if (input.MaterialBoxId != 0)
            {
                materialBoxObj = await _materialBoxRepository.FindByIdAsync(input.MaterialBoxId);
            }
            else
            {
                materialBoxObj = await _materialBoxRepository.FindByMaterialBoxcodeAsync(input.MaterialCode);
            }
            input.TaskTypeCode = TaskType.NPSortStockOut.ToString();

            var stockTask = await _stockTaskManagement.CreateWCSOut(input.TaskTypeCode, materialBoxObj);
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
                    mainObj.TaskTypeCode = TaskType.HPBatchStockIn;
                    mainObj.TaskStatus = TaskStatus.Executing;
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
            var manageMainlist =await _stockTaskRepository.GetListAsync(a => a.TaskTypeCode == TaskType.NPFullStockIn || a.TaskTypeCode == TaskType.NPSortStockOut);
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
            var box = await _materialBoxRepository.GetListAsync(x => x.CellId > 5);
            if (box.Count == 0)
            {
                throw new UserFriendlyException(message: "档案盒不存在!");
            }
            else
            {
                stockTaskDto.MaterialBoxId = box[0].Id;
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
            
            if (mge.TaskTypeCode == TaskType.HpAnnualCheckDown)
            {
                //_storageManager.UnLockCell(mge.StartCellId);
                //_storageManager.UnLockCell(mge.EndCellId);
                //设置库位状态
                var endCell = await _cellManager.SetAsStockOutAsync((int)mge.EndCellId);
                var startCell = await _cellManager.SetAsStockOutAsync((int)mge.StartCellId);
                //更新料箱的库位状态
                var box = await _materialBoxManager.UpdateStockCellAsync(mge.MaterialBoxBarcode, endCell.Id);
            }
            else if (mge.TaskTypeCode == TaskType.NpFullStockOut)
            {
                //出库时 库存处理
                var endCell = await _cellManager.SetAsStockOutAsync((int)mge.EndCellId);
                var startCell = await _cellManager.SetAsStockOutAsync((int)mge.StartCellId);
                //CompleteHandleCellOut(mge.StartCellId, mge.EndCellId);
                var box = await _materialBoxManager.UpdateStockOutCellAsync(mge.MaterialBoxBarcode);
            }
            else if (mge.TaskTypeCode == TaskType.HPBatchStockIn)
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
            mge.TaskEndTime = DateTime.Now.ToString();
            mge.TaskStatus = TaskStatus.Complete;
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

        /// <summary>
        /// 接收 WCS 主动推送的任务生命周期状态。
        /// </summary>
        public async Task<ResultWcsTaskDto> WcsSetStockTaskStatus(WcsCallBackRequest input)
        {
            return await _stockTaskManagement.WcsCallBack(input);
        }
        

    }
}
