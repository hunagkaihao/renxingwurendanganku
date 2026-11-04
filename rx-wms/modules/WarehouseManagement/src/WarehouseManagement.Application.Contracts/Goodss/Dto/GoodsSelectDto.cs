using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Goodss.Dto
{
    public class GoodsSelectDto
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string Value { get; set; }

    }
}
