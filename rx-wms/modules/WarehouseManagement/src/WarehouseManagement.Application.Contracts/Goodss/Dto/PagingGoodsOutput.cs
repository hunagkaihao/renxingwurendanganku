using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Goodss.Dto
{
    public class PagingGoodsOutput : EntityDto<int>
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 物料规格
        /// </summary>
        public string GoodsSpec { get; set; }
        /// <summary>
        /// 物料单位
        /// </summary>
        public string GoodsUnits { get; set; }

    }
}
