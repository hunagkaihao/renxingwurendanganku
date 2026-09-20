using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Authorization;
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
using WarehouseManagement.Permissions;
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
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public StockTaskAppService( IStockTaskRepository stockTaskRepository, StockTaskManager stockTaskManagement, 
                                    PlanManager planManager,IStockTaskDetailRepository stockTaskDetailRepository, 
                                    IGoodsRepository goodsRepository, ICellRepository cellRepository,
                                    IMaterialBoxRepository materialBoxRepository, CellManager cellManager, 
                                    IPlanRepository planRepository,WcsApiManager wcsApiManager, 
                                    MaterialBoxManager materialBoxManager,IMaterialRepository materialRepository, 
                                    IUnitOfWorkManager unitOfWorkManager, IDataFilter dataFilter)
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
            _unitOfWorkManager = unitOfWorkManager;
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

            var queryable = await _stockTaskRepository.GetQueryableAsync();
            var query = queryable.AsQueryable();

            if (input.PlanId.HasValue)
            {
                query = query.Where(stockTask => stockTask.PlanId == input.PlanId.Value);
            }

            if (input.StartCreationTime != default)
            {
                query = query.Where(stockTask => stockTask.CreationTime >= input.StartCreationTime);
            }

            if (input.EndCreationTime != default)
            {
                query = query.Where(stockTask => stockTask.CreationTime <= input.EndCreationTime);
            }

            if (!input.Filter.IsNullOrEmpty())
            {
                var filter = input.Filter.Trim();
                query = query.Where(stockTask => stockTask.MaterialBoxBarcode.Contains(filter));
            }

            if (input.HideCompletedTasks)
            {
                query = query.Where(stockTask =>
                    stockTask.TaskStatus != TaskStatus.Cancel &&
                    stockTask.TaskStatus != TaskStatus.Complete &&
                    stockTask.TaskStatus != TaskStatus.ExceptionComplete);
            }

            TaskStatus? taskStatusFilter = null;
            var taskStatusText = input.TaskStatus;
            if (!string.IsNullOrWhiteSpace(taskStatusText) &&
                !string.Equals(taskStatusText, "All", StringComparison.OrdinalIgnoreCase) &&
                Enum.TryParse<TaskStatus>(taskStatusText, out var parsedTaskStatus))
            {
                taskStatusFilter = parsedTaskStatus;
            }

            if (taskStatusFilter.HasValue)
            {
                var taskStatus = taskStatusFilter.Value;
                query = query.Where(stockTask => stockTask.TaskStatus == taskStatus);
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            var pagedQuery = query.OrderByDescending(stockTask => stockTask.Id)
                          .Skip(input.SkipCount)
                          .Take(input.PageSize);

            var queryResult = await AsyncExecuter.ToListAsync(pagedQuery);

            var stockTaskDtos = queryResult
                .Select(stockTask => ObjectMapper.Map<StockTask, StockTaskDto>(stockTask))
                .ToList();

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
        public async Task<StockTaskDto> CreateWCSIn(CreateStockInTaskDto input)
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

            var materialCode = input.MaterialCode.Trim();
            var material = await _materialRepository.FindByMaterialCodeAsync(materialCode);
            if (material == null)
            {
                throw new UserFriendlyException("基础物料信息无此物料");
            }

            Cell targetCell = null;
            if (input.EndCellId > 0)
            {
                targetCell = await _cellRepository.FindByIdAsync(input.EndCellId);
                if (targetCell == null)
                {
                    throw new UserFriendlyException("目标库位不存在");
                }

                targetCell.EnsureCanStockIn(materialCode);
                if (!string.Equals(targetCell.CellModel?.Trim(), material.MaterialType?.Trim(), StringComparison.Ordinal))
                {
                    throw new UserFriendlyException("目标库位规格与物料类型不一致");
                }
            }

            // 每次预约按基础物料信息创建容器记录，物料属性不接受客户端传入值。
            var materialBoxObj = new MaterialBox(material.MaterialName, materialCode)
            {
                MaterialBoxBarcode = materialCode,
                CellModel = material.MaterialType,
                MaterialUnit = material.MaterialUnit,
                RetentionPeriod = (material.ValidityDays ?? 0).ToString(CultureInfo.InvariantCulture),
                MaterialPeople = input.CreatorUserCode,
                MaterialInDate = materialCreateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                CreationTime = materialCreateTime
            };
            materialBoxObj = await _materialBoxRepository.InsertAsync(materialBoxObj, true);

            // 设置任务类型
            // 创建入库任务
            var stockTask = await _stockTaskManagement.CreateWCSIn(TaskType.NPFullStockIn.ToString(), materialBoxObj);
            if (targetCell != null)
            {
                stockTask.EndCellId = targetCell.Id;
                stockTask.EndCellCode = targetCell.CellCode;
                stockTask = await _stockTaskRepository.UpdateAsync(stockTask, true);
            }
            
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
            return await _stockTaskManagement.WCSSetCell(StockTaskId);
        }

        /// <summary>
        /// 下发已到期物料的自动出库任务。没有空闲柜门或系统存在活动任务时保留到下一轮调度。
        /// </summary>
        [UnitOfWork]
        public async Task<int> DispatchExpiredStockOutTasksAsync()
        {
            var activeTasks = await _stockTaskRepository.GetListAsync(task =>
                task.TaskStatus != TaskStatus.Complete &&
                task.TaskStatus != TaskStatus.Cancel &&
                task.TaskStatus != TaskStatus.ExceptionComplete);
            if (activeTasks.Count > 0)
                return 0;

            var materialBoxes = await _materialBoxRepository.GetListAsync(box => box.CellId > 0);
            var now = DateTime.Now;
            var dispatchedCount = 0;

            foreach (var materialBox in materialBoxes
                .OrderBy(box => box.MaterialInDate))
            {
                if (!DateTime.TryParse(materialBox.MaterialInDate, out var materialInTime) ||
                    !int.TryParse(materialBox.RetentionPeriod, out var validityDays) ||
                    validityDays < 0 || materialInTime.AddDays(validityDays) > now)
                {
                    continue;
                }

                var cabinetDoor = await _cellManager.GetEmptyStation(1, materialBox.CellModel);
                if (cabinetDoor == null)
                    continue;

                var stockTask = await _stockTaskManagement.CreateWCSOut(
                    TaskType.NPSortStockOut.ToString(), materialBox);
                stockTask.SetEndCell(cabinetDoor.Id, cabinetDoor.CellCode);
                stockTask.TaskRemark = "物料有效期到期自动出库";
                await _stockTaskRepository.UpdateAsync(stockTask, true);
                await WCSSetCell(stockTask.Id);
                dispatchedCount++;

                // 每轮只下发一个任务，后续任务等待该柜门重新空闲。
                break;
            }

            return dispatchedCount;
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
                    // 同时传入物料码时，以物料当前绑定的实际库位为准继续出库。
                    // 仅传库位码时仍需明确提示库位不存在。
                    if (string.IsNullOrWhiteSpace(materialCode))
                    {
                        throw new UserFriendlyException(message: "库位不存在!");
                    }
                }

                if (cell != null)
                {
                    cell.EnsureCanStockOut();

                    if (!string.IsNullOrWhiteSpace(materialCode) &&
                        !string.Equals(cell.MaterialCode, materialCode.Trim(), StringComparison.Ordinal))
                    {
                        throw new UserFriendlyException(message: "输入库位中的物料码与输入物料码不一致，出库任务下发失败!");
                    }

                    materialCode = cell.MaterialCode;
                }
            }

            CreateStockOutTaskDto stockTaskDto = new();
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

            // CreateWCSOut 已在独立事务中提交任务并完成 WCS 下发。
            // 当前 ClientOutCell 的外层事务在 MySQL 可重复读隔离级别下不可见该提交，
            // 不再重复查询，以免将已下发的任务误判为不存在。
            return stock;
        }
        
        /// <summary>
        /// 创建物料出库任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<StockTaskDto> CreateWCSOut(CreateStockOutTaskDto input)
        {
            if (input == null || (input.MaterialBoxId <= 0 && string.IsNullOrWhiteSpace(input.MaterialCode)))
            {
                throw new UserFriendlyException("物料容器 ID 或物料码不能为空");
            }

            MaterialBox materialBoxObj;
            if (input.MaterialBoxId != 0)
            {
                materialBoxObj = await _materialBoxRepository.FindByIdAsync(input.MaterialBoxId);
            }
            else
            {
                materialBoxObj = await _materialBoxRepository.FindByMaterialBoxcodeAsync(input.MaterialCode.Trim());
            }

            if (materialBoxObj == null)
            {
                throw new UserFriendlyException("物料容器不存在");
            }

            // 先在独立事务中创建并提交普通出库任务，确保随后 WCSSetCell 的独立事务可读取任务。
            // 批量出库使用 HPBatchStockOut，由执行计划时的专用调度流程下发，不经过此接口。
            StockTask stockTask;
            using (var unitOfWork = _unitOfWorkManager.Begin(requiresNew: true))
            {
                stockTask = await _stockTaskManagement.CreateWCSOut(
                    TaskType.NPSortStockOut.ToString(),
                    materialBoxObj);
                await unitOfWork.CompleteAsync();
            }

            await WCSSetCell(stockTask.Id);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(stockTask);
        }

        [Authorize(WarehouseManagementPermissions.StockTaskManagement.Create)]
        [UnitOfWork]
        public async Task<StockTaskDto> BorrowStockOutAsync(BorrowStockOutInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.MaterialCode))
            {
                throw new UserFriendlyException("物料码不能为空");
            }

            if (string.IsNullOrWhiteSpace(input.BorrowPurpose))
            {
                throw new UserFriendlyException("借用用途不能为空");
            }

            if (input.BorrowDurationHours <= 0)
            {
                throw new UserFriendlyException("借用时长必须大于 0 小时");
            }

            var materialCode = input.MaterialCode.Trim();
            var materialBox = await _materialBoxRepository.FindByMaterialBoxcodeAsync(materialCode);
            if (materialBox == null)
            {
                throw new UserFriendlyException("物料不存在");
            }

            var stockTask = await _stockTaskManagement.CreateWCSOut(
                TaskType.HPSortStockOut.ToString(),
                materialBox);
            stockTask.TaskOperator = CurrentUser.UserName;
            stockTask.TaskRemark = $"临时借用；用途：{input.BorrowPurpose.Trim()}；时长：{input.BorrowDurationHours}小时";
            stockTask = await _stockTaskRepository.UpdateAsync(stockTask, true);

            await WCSSetCell(stockTask.Id);

            var assignedTask = await _stockTaskManagement.FindByIdAsync(stockTask.Id);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(assignedTask);
        }

        [Authorize(WarehouseManagementPermissions.StockTaskManagement.Create)]
        [UnitOfWork]
        public async Task<StockTaskDto> ReturnStockInAsync(ReturnStockInInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.MaterialCode))
            {
                throw new UserFriendlyException("物料码不能为空");
            }

            var materialBox = await _materialBoxRepository.FindByMaterialBoxcodeAsync(input.MaterialCode.Trim());
            if (materialBox == null)
            {
                throw new UserFriendlyException("物料不存在");
            }

            if (materialBox.CellId > 0)
            {
                throw new UserFriendlyException("物料已在库位中，无法重复归还入库");
            }

            var stockTask = await _stockTaskManagement.CreateWCSIn(
                TaskType.NPFullStockIn.ToString(),
                materialBox);
            stockTask.TaskOperator = CurrentUser.UserName;
            stockTask.TaskRemark = "临时借用归还入库";
            stockTask = await _stockTaskRepository.UpdateAsync(stockTask, true);

            await WCSSetCell(stockTask.Id);

            var assignedTask = await _stockTaskManagement.FindByIdAsync(stockTask.Id);
            return base.ObjectMapper.Map<StockTask, StockTaskDto>(assignedTask);
        }
        
        public async Task<bool> BatBoxInByArea(string areaCode)
        {
            List<int> cellIds = await _cellManager.GetCellidsByAreaCode(areaCode);
            if (cellIds == null || cellIds.Count == 0)
            {
                throw new UserFriendlyException("所选区域未配置库位。");
            }

            // 按库位顺序创建批量出库任务。
            List<int> newcellIds = await _cellManager.OrderCellidsByIds(cellIds);
            return await ManageCreateBatIn(newcellIds, areaCode);
        }

        [UnitOfWork]
        public async Task<bool> ManageCreateBatIn(List<int> cellIds, string areaCode)
        {
            // 批量任务不能与尚未结束的出入库任务并行，已完成任务不影响新计划创建。
            var activeTasks = await _stockTaskRepository.GetListAsync(task =>
                task.TaskStatus != TaskStatus.Cancel &&
                task.TaskStatus != TaskStatus.Complete &&
                task.TaskStatus != TaskStatus.ExceptionComplete);
            if (activeTasks.Count > 0)
            {
                throw new UserFriendlyException("存在出入库任务，请先执行完其它任务。");
            }

            // 创建批量出库计划。
            PlanDto planMain = new PlanDto();
            DateTime.Now.Ticks.ToString();
            planMain.PlanCode = "批量出库" + DateTime.Now.Ticks.ToString();
            planMain.PlanExecuteType = PlanExecuteType.Automatic;
            planMain.PlanStatus = PlanStatus.Waiting;
            planMain.PlanTypeCode = PlanTypeInout.Out.ToString();
            planMain.AreaCode = areaCode;
            var entity = base.ObjectMapper.Map<PlanDto, Plan>(planMain);
            var plan = await _planRepository.InsertAsync(entity, true);

            var createdCount = 0;
            foreach (int cId in cellIds)
            {
                Cell cell = await _cellManager.GetByIdAsync(cId);
                if (cell is null)
                {
                    throw new UserFriendlyException("库位数据错误，请校核。");
                }

                var materialBox = await _materialBoxRepository.FindByCellIdAsync(cId);
                if (materialBox == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(materialBox.CellModel))
                {
                    throw new UserFriendlyException($"物料 {materialBox.MaterialBoxBarcode} 未设置物料类型，无法创建批量出库任务。");
                }

                var stockTask = new StockTask(
                    TaskType.HPBatchStockOut,
                    plan.Id,
                    PlanTypeInout.Out.ToString(),
                    materialBox.MaterialBoxBarcode,
                    cell.Id,
                    0,
                    cell.CellCode,
                    null)
                {
                    CellModel = materialBox.CellModel.Trim(),
                    TaskRemark = "批量出库"
                };
                await _stockTaskManagement.CreateCheckAsync(stockTask);
                createdCount++;
            }

            if (createdCount == 0)
            {
                throw new UserFriendlyException("所选区域没有可出库的物料。");
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            Log.Debug("用户创建了等待执行的批量出库计划，ID:" + plan.Id + "  方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return true;
        }

        [UnitOfWork]
        public async Task<List<StockTaskDto>> GetInOutTask()
        {
            var manageMainlist =await _stockTaskRepository.GetListAsync(a =>
                a.TaskTypeCode == TaskType.NPFullStockIn ||
                a.TaskTypeCode == TaskType.NPSortStockOut ||
                a.TaskTypeCode == TaskType.HPSortStockOut);
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
            CreateStockOutTaskDto stockTaskDto = new();
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
