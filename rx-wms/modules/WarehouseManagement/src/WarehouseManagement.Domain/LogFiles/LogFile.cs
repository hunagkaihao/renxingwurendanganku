using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.LogFiles
{
    public class LogFile
    { 
        /// <summary>
        /// 日志文件记录表
        /// </summary>
        private LogFile()
        {
        }
        public LogFile(string logFileName, string createTime, string modifyTime, string logFileUrl)
        {
            LogFileName = logFileName; 
            CreateTime = createTime;
            ModifyTime = modifyTime;
            LogFileUrl = logFileUrl;
        }

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
