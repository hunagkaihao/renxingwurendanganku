using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WarehouseManagement.Checks.Dto;

namespace WarehouseManagement.Checks
{
    public interface ICheckAppService : IApplicationService
    {

        /// <summary>
        /// 档案库创建区域盘点计划
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CheckDto> CreateCheckByAreaAsync(CreateCheckDto input);
        /// <summary>
        /// 将计划设置为执行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> SetAsExecutingAsync(IdIntInput input);

        Task<PagedResultDto<CheckDto>> GetPagingListAsync(PagingCheckListInput input);

        Task<PagedResultDto<CheckDetailDto>> GetPagingDetailListAsync(PagingCheckDetailInput input);

        Task DeleteAsync(IdIntInput input);
        /// <summary>
        /// 更新实盘数量
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task UpdateRealAmountAsync(UpdateCheckDetailDto input);

        //账实一致确认
        Task<bool> InventoryConfirm(IdIntInput input);
        //盘亏
        Task<bool> InventoryLossConfirm(IdIntInput input);

        //盘盈入库
        Task CreateSurplusIn(string ArBoxRfid, string cellName);
        //盘亏出库
        Task CreateLossOut(string ArBoxRfid, string cellName);
        //盘点完成
        Task<bool> CheckComplete(string checkCode);



    }
}
