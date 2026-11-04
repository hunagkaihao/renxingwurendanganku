using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarehouseManagement.Goodss.Dto
{
    public class PagingGoodsListInput : PagingBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }
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
    }
}
