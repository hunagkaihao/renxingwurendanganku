using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Lion.AbpPro.Extension.Customs.Dtos;
using WarehouseManagement.StockTasks.Dto;
using WarehouseManagement.WcsTasks.Dto;

namespace WarehouseManagement.StockTasks
{
    public interface IStockTaskAppService : IApplicationService
    {

        Task<PagedResultDto<StockTaskDto>> GetPagingListAsync(PagingStockTaskListInput input);

        Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListAsync(PagingStockTaskDetailInput input);

        Task<PagedResultDto<StockTaskDetailDto>> GetPagingDetailListByArchiveIdAsync(PagingStockTaskDetailInput input);

        /// <summary>
        /// 更新书籍
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<StockTaskDto> UpdateAsync(UpdateStockTaskDto input);

        /// <summary>
        /// 删除书籍
        /// </summary>
        Task DeleteAsync(IdIntInput input);

        //档案借阅出库
        Task<bool> PickOutTask(List<PickOutDto> input);

        Task<StockTaskDto> SetAsCancelAsync(IdIntInput input);



        /// <summary>
        /// 创建档案入库任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<StockTaskDto> CreateWCSIn(CreateStockTaskDto input);
        /// <summary>
        /// 档案入库分配库位
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Boolean> WCSSetCell(int input);
        Task<StockTaskDto> OpenDoorAndWCSInExcute(int input);
        /// <summary>
        /// 创建Wcs出库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<StockTaskDto> CreateWCSOut(CreateStockTaskDto input);

        /// <summary>
        /// 批量入库任务
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        Task<bool> BatBoxInByArea(string areaCode);

        //Task<bool> BatInSetExecute();


        /// <summary>
        /// 一体机根据rfid下达任务
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        Task<bool> TaskAssignUseRfid(string rfid);
        /// <summary>
        /// 一体机出库
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns></returns>
        Task<bool> ClientOutCell(string rfid);
        /// <summary>
        /// 获取出入库任务
        /// </summary>
        /// <returns></returns>
        Task<List<StockTaskDto>> GetInOutTask();

        /// <summary>
        /// 一体机任务自动下达
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task TaskAssign(int stockId);
        /// <summary>
        /// 创建疲劳任务
        /// </summary>
        /// <returns></returns>
        Task CreateBatTest();
        //手动完成任务

        //指定库位下达

        //批量取消任务
        //Task ManyDeleteTask(int stockId);

        //通过任务ID开门
        Task ControlDoorOpen (int stockId);

        //强制完成任务
        Task ForceComplete(int stockId);

        /// <summary>
        /// 接收 WCS 主动推送的任务生命周期状态。
        /// </summary>
        Task<ResultWcsTaskDto> WcsSetStockTaskStatus(WcsCallBackRequest input);




    }
}
