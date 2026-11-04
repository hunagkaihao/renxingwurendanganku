using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.RfidCodes.Dto
{
    public class PagingRfidListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// 档案名称
        /// </summary>
        //public string ArchiveBoxName { get; set; }

    }
}
