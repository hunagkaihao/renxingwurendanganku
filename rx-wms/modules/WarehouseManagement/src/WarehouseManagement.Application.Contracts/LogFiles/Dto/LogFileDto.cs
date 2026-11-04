using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.LogFiles.Dto
{
    public class LogFileDto
    {

        /// <summary>
        /// 日志名称
        /// </summary>
        public string LogFileName { get; set; }
        /// <summary>
        /// 日志创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 日志修改时间
        /// </summary>
        public string ModifyTime { get; set; }
        /// <summary>
        /// 日志下载路径
        /// </summary>
        public string LogFileUrl { get; set; }

    }
}
