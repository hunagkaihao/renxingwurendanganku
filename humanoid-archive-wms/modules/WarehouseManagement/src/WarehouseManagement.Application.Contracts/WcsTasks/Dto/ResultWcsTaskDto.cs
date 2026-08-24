using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;

namespace WarehouseManagement.WcsTasks.Dto
{
    public class ResultWcsTaskDto
    {
        
        /// <summary>
        /// True：成功
        /// false：失败
        /// </summary>
        public Boolean Success { get; set; }
        /// <summary>
        /// 调用结果描述
        /// </summary>
        public string Message { get; set; }
        public string QueryCode { get; set; }

        public ResultWcsTaskDto(bool success, string message)
        {
            Success = success;
            Message = message;
        }

    }
}
