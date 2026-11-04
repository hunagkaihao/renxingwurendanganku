using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.WcsTasks.Dto
{
    public  class OpenDoorForOrderDto
    {

        /// <summary>
        /// True：成功
        /// false：失败
        /// </summary>
        public Boolean success { get; set; }
        /// <summary>
        /// 调用结果描述
        /// </summary>
        public string message { get; set; }
    }
}
