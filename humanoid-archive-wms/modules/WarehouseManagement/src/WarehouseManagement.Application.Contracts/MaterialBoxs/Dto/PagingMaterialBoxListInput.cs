using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.MaterialBoxs.Dto
{
    public class PagingMaterialBoxListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// 物料盒名称
        /// </summary>
        public string MaterialBoxName { get; set; }

    }
}
