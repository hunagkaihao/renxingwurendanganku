using Wcs.Dispatch;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace Wcs.WMS
{
    /// <summary>
    /// WMS交互服务
    /// </summary>
    public interface IWMSService : IApplicationService
    {
        /// <summary>
        /// 反馈任务状态
        /// </summary>
        Task<bool> SendTaskStatus(TaskStatusDto taskStatusDto);
        
        /// <summary>
        /// 获取盘点任务
        /// </summary>
        Task<List<CheckOrder>> GetChkTask(ChkTaskDto chkTaskDto);
        
        /// <summary>
        /// 下发盘点状态
        /// </summary>
        Task<bool> SendChkStatus(ChkStatusDto chkStatusDto);
        
    }
}
