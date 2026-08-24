using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using WarehouseManagement.ArchiveBoxs;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.Cells;
using WarehouseManagement.CheckHiss;
using WarehouseManagement.CheckHiss.Aggregates;
using WarehouseManagement.CheckHiss.Dto;
using WarehouseManagement.Checks.Aggregates;
using WarehouseManagement.Checks.Dto;
using WarehouseManagement.Goodss;
using WarehouseManagement.StockTasks;
using WarehouseManagement.StockTasks.Aggregates;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.TaskHiss;
using WarehouseManagement.TaskHiss.Aggregates;
using WarehouseManagement.TaskHiss.Dto;
using WarehouseManagement.WcsTasks;
using WarehouseManagement.WcsTasks.Dto;
using Check = WarehouseManagement.Checks.Aggregates.Check;

namespace WarehouseManagement.Checks
{
    //[Authorize(WarehouseManagementPermissions.CheckManagement.Default)]
    public class CheckAppService : WarehouseManagementAppService,
         ICheckAppService //implement the ICheckAppService
    {
        //private readonly IRepository<Check, Guid> _checkRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly ICheckRepository _checkRepository;
        private readonly ICheckDetailRepository _checkDetailRepository;
        private readonly IGoodsRepository _goodsRepository;
        private readonly CheckManager _checkManagement;
        private readonly CellManager _cellManager;
        private readonly StockTaskManager _stockTaskManager;
        private readonly ArchiveBoxManager _archiveBoxManager;
        private readonly WcsApiManager _wcsApiManager;
        private readonly CheckDetailManager _checkDetailManager;
        private readonly CheckHisManager _checkHisManager;
        private readonly CheckDetailHisManager _checkDetailHisManager;
        private readonly ICurrentUser _currentUser;
        private readonly TaskHisManager _taskHisManager;
        public CheckAppService(ICheckRepository checkRepository, CheckManager checkManagement
            , IGoodsRepository goodsRepository, ICheckDetailRepository checkDetailRepository
            , ICurrentUser currentUser, CellManager cellManager, StockTaskManager stockTaskManager,
            ArchiveBoxManager archiveBoxManager,CheckDetailHisManager checkDetailHisManager
            ,WcsApiManager wcsApiManager ,CheckDetailManager checkDetailManager,CheckHisManager checkHisManager
            ,TaskHisManager taskHisManager)
        {
            _checkRepository = checkRepository;
            _checkManagement = checkManagement;
            _goodsRepository = goodsRepository;
            _checkDetailRepository = checkDetailRepository;
            _currentUser = currentUser;
            _cellManager = cellManager;
            _stockTaskManager = stockTaskManager;
            _archiveBoxManager = archiveBoxManager;
            _wcsApiManager = wcsApiManager;
            _checkDetailManager = checkDetailManager;
            _checkHisManager = checkHisManager;
            _checkDetailHisManager = checkDetailHisManager;
            _taskHisManager = taskHisManager;
        }
        //[Authorize(WarehouseManagementPermissions.CheckManagement.Create)]
        //创建盘点计划
        [AllowAnonymous]
        public async Task<CheckDto> CreateCheckByAreaAsync(CreateCheckDto input)
        {
            var entity = new Check(input.AreaCode)
            {
                CheckCode = "YK" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                CheckStatus = CheckStatus.Waiting,
                CheckType = CheckType.AreaCodeAuto,
                AreaCode = input.AreaCode,
                AccuracyFlag = 1
            };

            var check = await _checkManagement.CreateByAreaCodeAsync(entity);
            return  base.ObjectMapper.Map<Check, CheckDto>(check);
        }

        //执行盘点计划
        [AllowAnonymous]
        public async Task<bool> SetAsExecutingAsync(IdIntInput input)
        {
            bool bResult = true;
            //?判断是否存在出入库任务
            if (await _stockTaskManager.ExistInOutManage())
            {
                throw new UserFriendlyException("盘点计划下达过程中不允许有档案盒的出入库任务!");
            }
            try
            {
                Check mCheckMain = await _checkRepository.FindByIdAsync(input.Id);
                //20220422 避免同一盘点计划，多次下达。
                if (mCheckMain.CheckStatus != CheckStatus.Waiting)
                {
                    throw new UserFriendlyException("计划不能重复下达!");
                }
                if (mCheckMain.CheckType == CheckType.AnnualCheck)
                {
                    //创建年度盘点明细

                }
                else if (mCheckMain.CheckType == CheckType.AreaCodeAuto)
                {

                    //获取区域所在的库位列表
                    List<Cell> cells = await _cellManager.GetCellsByAreaCode(mCheckMain.AreaCode);
                    //创建盘点清单
                    //List<CheckDetail> checkDetails = new List<CheckDetail>();
                    Cell endCell = await _cellManager.GetByCodeAsync("12002");
                    CheckOrderCreateDto checkOrderCreate = new()
                    {
                        Priority = 1,
                        Orders = new(),
                    };
                    
                    if (cells.Count > 0)
                    {
                        //foreach (int cId in cellids)
                        for (int i = 0; i < cells.Count; i++)
                        {
                                //下达盘点任务
                                var stock = await ManageCreateCheckByCell(cells[i].Id, mCheckMain.Id, cells[i].CellCode);
                                OrderDto order = new();
                                order.OrderCode = stock.Id.ToString();
                                order.CellCode = stock.EndCellCode;
                                checkOrderCreate.Orders.Add(order);
                        }
                    }
                    var req = await _wcsApiManager.CheckOrderCreate(checkOrderCreate);
                    mCheckMain.BatchNo = req.QueryCode;
                    mCheckMain.BeginTime = DateTime.Now.ToString();
                    mCheckMain.CheckStatus = CheckStatus.Executing;
                    await _checkManagement.UpdateAsync(mCheckMain, true);

                    
                }
                return bResult;
            }
            catch (Exception ex)
            {
                var exceptionClassName = ex.GetType().ToString();
                //如果是自定义异常，则使用原抛出
                if (!exceptionClassName.Contains("UserFriendlyException"))
                {
                    throw new UserFriendlyException("下达任务失败!" + ex.ToString());
                }
                else
                {
                    throw;
                }
            }
        }

        //创建盘点任务
        public async Task<StockTask> ManageCreateCheckByCell(int cellId, int checkId, string cellCode)
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

                stockTask.PlanId = checkId;
                stockTask.PlanTypeCode = "Check";
                stockTask.EndCellId = cellId;
                stockTask.ManageTypeCode = ManageType.HpAnnualCheckDown;
                stockTask.ManageStatus = ManageStatus.Executing;
                stockTask.StartCellId = cellId;
                stockTask.StartCellCode = cellCode;
                stockTask.EndCellCode = cellCode;

                //增加操作者ID
            }
            catch
            {
                throw new UserFriendlyException("ManageCreateCheckByCell异常");
            }
            if (await _stockTaskManager.ValidateStockManageExist(stockTask.ArchiveBoxRfid))
            {
                throw new UserFriendlyException("档案盒已存在任务");
            }
            var stock = base.ObjectMapper.Map<StockTaskDto, StockTask>(stockTask);
            var st = await _stockTaskManager.CreateCheckAsync(stock);

            //创捷盘点明细
            await CreateCheckList(checkId, st);

            //锁定库位
            if (stockTask.StartCellId != 0 )
            {
                await _cellManager.SetSelectedAsync(stockTask.StartCellId);
            }

            //自动执行盘点任务
            //await CheckDownLoadAsync(st.Id , cellCode);

            //添加工作单元、事务处理 
            await CurrentUnitOfWork.SaveChangesAsync();
            return stock;
        }
        //创建盘点明细
        public async Task CreateCheckList(int checkId , StockTask stock)
        {
            CheckDetailDto checkDetail = new()
            {
                CheckId = checkId,
                ManageId = stock.Id,
                CompleteFlag = 0,
            };
            if(stock != null)
            {
                if (stock.ArchiveBoxRfid != "")
                {
                    var archiveBox = await _archiveBoxManager.GetArchiveBoxByRfidCode(stock.ArchiveBoxRfid);
                    if(archiveBox == null)
                    {
                        throw new UserFriendlyException("档案盒标签不存在！");
                    }
                    //List<ArchiveBoxDetail> archiveBoxDetails = await _archiveBoxManager.GetArchiveBoxByRfidCode(stock.ArchiveBoxRfid);
                    if(archiveBox.CellId > 0)
                    {
                        var cell = await _cellManager.GetByIdAsync(archiveBox.CellId);
                        if(cell != null)
                        {
                            checkDetail.CellName = cell.CellName;
                        }
                    }
                    else
                    {
                        //var cell = await _cellManager.GetByIdAsync((int)stock.EndCellId);
                        checkDetail.CellName = stock.EndCellCode;
                    }
                    checkDetail.StockBarcode = stock.ArchiveBoxRfid;
                    checkDetail.Account = 1;
                    checkDetail.GoodsId = 0;
                    checkDetail.BoxBarcode = "";
                }
                else
                {
                    //记录无库存的盘点记录的库位
                    checkDetail.CellName = (await _cellManager.GetByIdAsync((int)stock.EndCellId)).CellName;
                    //20220331 无库存的盘点数量修改为0
                    checkDetail.Account = 0;
                    checkDetail.GoodsId = 0;
                    checkDetail.BoxBarcode = "";
                }
                await _checkDetailManager.CreateCheckDetailAsync(checkDetail);
            }
            else
            {
                throw new UserFriendlyException("盘点任务异常！");
            }
        }

        //盘点任务执行
        [UnitOfWork]
        public async Task CheckDownLoadAsync(int stockId , string cellCode)
        {
            var stock= await _stockTaskManager.FindByIdAsync(stockId);
            if (stock != null)
            {
                if(stock.ManageStatus == ManageStatus.WaitingExecute)
                {
                    //创建WCS任务
                    var res = await DownloadToWcsAsync(stockId, cellCode);
                    //保存结果查询码
                    stock.ManageRemark = res.QueryCode;
                    //更新库位状态到执行中
                    stock.ManageStatus = ManageStatus.Executing;
                    stock.ManageBeginTime = DateTime.Now.ToString();
                    stock.ManageLaneWay = "盘点任务1号取货位";
                    await _stockTaskManager.UpdateAsync(stock);

                    //更新库位状态
                    await _cellManager.SetSelectedAsync((int)stock.StartCellId);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                throw new UserFriendlyException("任务不存在");
            }

        }
        //下达至WCS
        [UnitOfWork]
        public async Task<ResultWcsTaskDto> DownloadToWcsAsync(int orderCode, string cellCode)
        {

            try
            {
                CheckOrderCreateDto checkOrderCreate = new()
                {
                    Priority = 1,
                    Orders = new(),
                };
                OrderDto order = new()
                {
                    OrderCode = orderCode.ToString(),
                    CellCode = cellCode
                };
                checkOrderCreate.Orders.Add(order);
                
                return await _wcsApiManager.CheckOrderCreate(checkOrderCreate);
            }
            catch
            {
                throw new UserFriendlyException("创建WCS任务出错.");
            }
        }

        


        //完成盘点任务
        public async Task<bool> CompleteOne(int stockId, string rfid, int flag, string remark)
        {
            bool bResult = true;
            try
            {
                List<CheckDetail> checkDetail = await _checkDetailManager.GetCheckDetailByStockId(stockId);
                if(checkDetail.Count == 0)
                {
                    throw new UserFriendlyException("CHECK_LIST中未找到索引");
                }
                else
                {
                    Check check = await _checkManagement.GetCheck(checkDetail[0].CheckId);
                    if(check == null)
                    {
                        throw new UserFriendlyException("CHECK_MAIN中未找到索引");
                    }
                    //check.CheckStatus = CheckStatus.Complete;
                    // 判断历史记录中有无该年度盘点单号 没有则新建
                    CheckHis checkHis = (await _checkHisManager.GetHisAsync(check.CheckCode)).FirstOrDefault();
                    int checkHisId = 0;
                    if(checkHis == null)
                    {
                        checkHisId = await CreateCheckHis(check);
                    }
                    else
                    {
                        checkHisId = checkHis.Id;
                    }
                    foreach(CheckDetail ck in checkDetail)
                    {
                        //盘点子表加入历史表
                        bResult = await CreateCheckDetailHis(checkHisId, ck, flag, remark);
                        if (!bResult)
                        {
                            return bResult;
                        }
                        //删除盘点子表
                        await _checkDetailRepository.DeleteAsync(ck.Id);
                    }
                    //判断是否全部盘点完毕
                    List<CheckDetail> checkDetails = await _checkDetailManager.GetCheckDetail(check.Id);
                    if(checkDetails.Count == 0)
                    {
                        CheckHis ckhis =await _checkHisManager.GetHisByIdAsync(checkHisId);
                        ckhis.FinishTime = DateTime.Now.ToString();
                        ckhis.CheckStatus = CheckStatus.Complete.ToString();
                        await _checkHisManager.UpdateAsync(ckhis);
                        //删除对应的年度任务盘点
                        await _checkManagement.DeleteAsync(ckhis.CheckCode);
                    }

                }
                return bResult;
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.ToString());
            }
        }

        //创建盘点任务历史记录
        public async Task<int> CreateCheckHis(Check check)
        {
            try
            {
                CheckHisDto checkHisDto = new();
                checkHisDto.CheckCode = check.CheckCode;
                checkHisDto.CheckStatus = check.CheckStatus.ToString();
                checkHisDto.CheckType = check.CheckType.ToString();
                checkHisDto.CreateTime = check.CreateTime;
                checkHisDto.BeginTime = check.BeginTime;
                checkHisDto.FinishTime = check.FinishTime;
                checkHisDto.GoodsCode = check.GoodsCode;
                checkHisDto.BatchNo = check.BatchNo;
                checkHisDto.AreaCode = check.AreaCode;
                checkHisDto.Supplier = check.Supplier;
                var entity = base.ObjectMapper.Map<CheckHisDto, CheckHis>(checkHisDto);
                var checkHis = await _checkHisManager.CreateAsync(entity);
                return checkHis.Id;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.ToString());
            }
        }

        //创建盘点明细历史记录
        public async Task<bool> CreateCheckDetailHis(int checkHisId, CheckDetail checkDetail, int flag, string remark)
        {
            bool bResult = true;
            try
            {
                CheckDetailHisDto checkDetailHisDto = new()
                {
                    CheckId = checkHisId,
                    ManageId = checkDetail.ManageId,
                    Remark = checkDetail.Remark,
                    StockBarcode = checkDetail.StockBarcode,
                    CellName = checkDetail.CellName,
                    GoodsId = checkDetail.GoodsId,
                    Supplier = checkDetail.Supplier,
                    Account = checkDetail.Account,
                    RealAmount_1 = checkDetail.RealAmount_1,
                    RealAmount_2 = checkDetail.RealAmount_2,
                    ProfitLossAmount = checkDetail.ProfitLossAmount,
                    Checker = checkDetail.Checker,
                    BeginTime = checkDetail.BeginTime,
                    FinishTime = DateTime.Now.ToString(),
                    BoxBarcode = checkDetail.BoxBarcode
                };
                checkDetailHisDto.Remark = remark;
                checkDetailHisDto.Checker = "system";
                //盘点一致
                if (flag == 2)
                {
                    checkDetailHisDto.RealAmount_1 = checkDetail.Account;
                    checkDetailHisDto.ProfitLossAmount = 0;
                }
                //盘亏
                else if (flag == 3)
                {
                    checkDetailHisDto.RealAmount_1 = 0;
                    checkDetailHisDto.ProfitLossAmount = 1;
                }
                //空库位 盘盈
                else if (flag == 4)
                {
                    checkDetailHisDto.RealAmount_1 = 1;
                    checkDetailHisDto.ProfitLossAmount = -1;
                }
                else
                {
                    checkDetailHisDto.Remark = "任务被强制完成，反馈信息不规范";
                }
                var entity = base.ObjectMapper.Map<CheckDetailHisDto, CheckDetailHis>(checkDetailHisDto);
                var checkdetailHis = await _checkDetailHisManager.CreateAsync(entity);
                
                return bResult;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.ToString());
            }
        }

        public async Task<PagedResultDto<CheckDto>> GetPagingListAsync(PagingCheckListInput input)
        {

            var result = new PagedResultDto<CheckDto>();
            var totalCount = await _checkRepository.GetPagingCountAsync(input.Filter);
            result.TotalCount = totalCount;
            if (totalCount <= 0) return result;

            var entities = await _checkRepository.GetPagingListAsync(input.Filter, input.PageSize,
                input.SkipCount, false);
            result.Items = ObjectMapper.Map<List<Check>, List<CheckDto>>(entities);

            return result;
        }

        public async Task<PagedResultDto<CheckDetailDto>> GetPagingDetailListAsync(
PagingCheckDetailInput input)
        {
            //Get the IQueryable<Book> from the repository
            var queryable = await _checkDetailRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from checkDetail in queryable
                        //join goods in await _goodsRepository.GetQueryableAsync() on checkDetail.GoodsId equals goods.Id
                        //join taskHis in await _checkDetailRepository.GetQueryableAsync() on taskHisDetail.CheckId equals taskHis.Id
                        where checkDetail.CheckId == input.CheckId & checkDetail.StockBarcode.Contains(input.StockBarcode.IsNullOrEmpty() ? "" : input.StockBarcode.Trim())
                        select new { checkDetail};

            //Paging
            query = query
                //.OrderBy(NormalizeSorting(input.Sorting))
                .OrderBy(f => f.checkDetail.Id)
                .Skip(input.SkipCount)
                .Take(1000);
            //.Take(input.MaxResultCount);

            //Execute the query and get a list
            var queryResult = await AsyncExecuter.ToListAsync(query);

            //Convert the query result to a list of BookDto objects
            var checkDetailDtos = queryResult.Select(x =>
            {
                var checkDetailDtos = ObjectMapper.Map<CheckDetail, CheckDetailDto>(x.checkDetail);
                //checkDetailDtos.StockBarcode = x.taskHis.StockBarcode;
                //checkDetailDtos.GoodsCode = x.goods.GoodsCode;
                //checkDetailDtos.GoodsName = x.goods.GoodsName;
                return checkDetailDtos;
            }).Take(input.PageSize).ToList();

            //Get the total count with another query
            //var totalCount = await _taskHisDetailRepository.GetCountAsync();
            var totalCount = queryResult.Count();

            return new PagedResultDto<CheckDetailDto>(
                totalCount,
                checkDetailDtos
            );
        }

        [UnitOfWork]
        public async Task<bool> CheckComplete(string checkCode)
        {
            CheckHis checkHis = (await _checkHisManager.GetHisAsync(checkCode)).FirstOrDefault();
            if(checkHis.CheckStatus == CheckStatus.Finish.ToString())
            {
                throw new UserFriendlyException("盘点计划已结束，不需要重复完成");
            }
            //获取盘点任务
            List<StockTask> stockTasks = await _stockTaskManager.GetCheckList();
            if (stockTasks.Count > 0)
            {
                throw new UserFriendlyException("盘点任务还未执行完毕，不能完成盘点计划");
            }
            List<CheckDetailHis> checkDetailHis = await _checkDetailHisManager.GetList(0, checkHis.Id);
            if (checkDetailHis.Count > 0)
            {
                throw new UserFriendlyException("盘点记录还未审核完毕，不能完成盘点计划");
            }
            checkHis.CheckStatus = CheckStatus.Finish.ToString();
            await _checkHisManager.UpdateAsync(checkHis);
            //完成后删除盘点任务
            //await _checkManagement.DeleteByCheckCodeAsync(checkCode);
            return true;
        }
        //盘点历史结果盘亏确认
        public async Task<bool> InventoryLossConfirm(IdIntInput input)
        {
            CheckDetailHis checkDetailHis = await _checkDetailHisManager.GetById(input.Id);
            if (checkDetailHis.VerifyFlag == 1)
            {
                throw new UserFriendlyException("该条盘点记录已确认，不需要重复确认");
            }
            ArchiveBox archiveBox = await _archiveBoxManager.GetArchiveBoxByBoxName(checkDetailHis.StockBarcode);
            if(archiveBox == null)
            {
                throw new UserFriendlyException("档案盒不存在，无法进行盘亏处理");
            }
            if (archiveBox.Id > 0)
            {
                throw new UserFriendlyException("需先处理错误库位库存，才能进行盘亏确认");
            }

            checkDetailHis.VerifyUser = _currentUser.UserName;
            checkDetailHis.VerifyAmount = 0;
            checkDetailHis.VerifyFinishTime = DateTime.Now.ToString();
            checkDetailHis.VerifyFlag = 1;
            await _checkDetailHisManager.UpdateAsync(checkDetailHis);
            return true;
        }
        //盘点历史结果账实一致确认
        public async Task<bool> InventoryConfirm(IdIntInput input)
        {
            CheckDetailHis checkDetailHis = await _checkDetailHisManager.GetById(input.Id);
            if(checkDetailHis.VerifyFlag == 1)
            {
                throw new UserFriendlyException("该条盘点记录已确认，不需要重复确认");
            }
            checkDetailHis.VerifyUser = _currentUser.UserName;
            checkDetailHis.VerifyAmount = 1;
            checkDetailHis.VerifyFinishTime = DateTime.Now.ToString();
            checkDetailHis.VerifyFlag = 1;
            await _checkDetailHisManager.UpdateAsync(checkDetailHis);
            return true;
        }
        //盘盈入库
        [UnitOfWork]
        public async Task CreateSurplusIn(string ArBoxRfid, string cellName)
        {
            //step1 获取是否存在库位终
            var archiveBox = await _archiveBoxManager.GetArchiveBoxByRfidCode(ArBoxRfid);
            if(archiveBox == null)
            {
                throw new UserFriendlyException("档案盒不存在，无法执行盘盈入库：");
            }
            if (archiveBox.CellId != 0)
            {
                var cellnameE = await _cellManager.GetByIdAsync(archiveBox.CellId);
                if (cellnameE.CellName != cellName)
                {
                    throw new UserFriendlyException("无法执行操作，档案盒已存在于库位：" + cellnameE);
                }
            }


            //step2 入库库存处理
            //库位解锁以及库位状态的变更   盘点时不变更库位状态
            var currentCell = await _cellManager.GetByNameAsync(cellName);
            //设置库位状态
            var startCell = await _cellManager.SetAsStockInAsync(currentCell.Id);

            //step3 库存处理
            //更新档案盒的库位
            await _archiveBoxManager.UpdateStockCellAsync(ArBoxRfid, currentCell.Id);
            //List<StorageList> storageLists = _storageListRepository.GetAllList(a => a.StorageId == stg.Id);
            //foreach (var storageList in storageLists)
            //{
            //    GoodsMain goodsMain = _goodsMainRepository.Get(storageList.GoodsId);
            //    goodsMain.GoodsFlag = Enums.Achive_STATUS.StockIn.ToString();
            //    _goodsMainRepository.Update(goodsMain);
            //}

            //step4 创建历史记录
            StockTaskDto taskhis = new StockTaskDto()
            {
                StartCellCode = null,
                EndCellCode = cellName,
                ManageTypeCode = ManageType.SurplusIn,
                ManageStatus = ManageStatus.Complete,
                //ManageOperator = _currentUser.UserName,
                //ManageCreateTime = DateTime.Now.ToString(),//增加创建时间
                //ManageBeginTime = DateTime.Now.ToString(),
                //ManageEndTime = DateTime.Now.ToString(),
                ArchiveBoxRfid = ArBoxRfid,
            };
            var entiety = base.ObjectMapper.Map<StockTaskDto, StockTask>(taskhis);
            var stockhis =await _taskHisManager.CreateAsync(entiety, entiety.Details);

            await CurrentUnitOfWork.SaveChangesAsync();

        }
        //盘亏出库
        [UnitOfWork]
        public async Task CreateLossOut(string ArBoxRfid, string cellName)
        {
            //step1 获取是否存在库位终
            var stg = await _archiveBoxManager.GetArchiveBoxByRfidCode(ArBoxRfid);
            if (stg.CellId != 0)
            {
                var cellnameE =await _cellManager.GetByIdAsync(stg.CellId);
                if (cellnameE.CellName != cellName)
                {
                    throw new UserFriendlyException("无法执行操作，档案盒已存在于库位：" + cellnameE);
                }
            }


            //step2 入库库存处理
            //库位解锁以及库位状态的变更   盘点时不变更库位状态
            var currentCell = await _cellManager.GetByNameAsync(cellName);
            //设置库位状态
            var startCell = await _cellManager.SetAsStockOutAsync(currentCell.Id);
            

            //step3 库存处理
            //更新档案盒的库位
            await _archiveBoxManager.UpdateStockOutCellAsync(ArBoxRfid);

            //List<StorageList> storageLists = _storageListRepository.GetAllList(a => a.StorageId == stg.Id);
            //foreach (var storageList in storageLists)
            //{
            //    GoodsMain goodsMain = _goodsMainRepository.Get(storageList.GoodsId);
            //    goodsMain.GoodsFlag = Enums.Achive_STATUS.StockOut.ToString();
            //    _goodsMainRepository.Update(goodsMain);
            //}

            //step4 创建历史记录
            StockTaskDto taskhis = new StockTaskDto()
            {
                StartCellCode = null,
                EndCellCode = null,
                ManageTypeCode = ManageType.SurplusIn,
                ManageStatus = ManageStatus.Complete,
                //ManageOperator = _currentUser.UserName,
                //CreationTime = DateTime.Now.ToString(),//增加创建时间
                //ManageBeginTime = DateTime.Now.ToString(),
                //ManageEndTime = DateTime.Now.ToString(),
                ArchiveBoxRfid = ArBoxRfid,
            };
            var entity = base.ObjectMapper.Map<StockTaskDto, StockTask>(taskhis);
            var stockhis = await _taskHisManager.CreateAsync(entity, null);
  

            await CurrentUnitOfWork.SaveChangesAsync();
            

        }

        [AllowAnonymous]
        public virtual async Task DeleteAsync(IdIntInput input)
        {
            await _checkManagement.DeleteByIdAsync(input.Id);
            //await _stockTaskRepository.DeleteAsync(input.Id);
        }
        public async Task UpdateRealAmountAsync(UpdateCheckDetailDto input)
        {
            await _checkManagement.UpdateRealAmountAsync(input.Id,input.RealAmount_1, _currentUser.UserName);
        }
    }
}
