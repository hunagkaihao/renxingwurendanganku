using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarehouseManagement.Goodss.Dto
{
    public class UpdateGoodsDto
    {
        [Required]
        public int Id { get; set; }
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
        /// 物料品牌
        /// </summary>
        public string GoodsConstProperty1 { get; set; }
        /// <summary>
        /// 物料单位
        /// </summary>
        public string GoodsUnits { get; set; }

    }
}
