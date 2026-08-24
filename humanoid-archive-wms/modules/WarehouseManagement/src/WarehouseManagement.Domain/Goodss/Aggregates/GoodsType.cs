using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace WarehouseManagement.Goodss.Aggregates
{
    public class GoodsType : Entity<int>
    {
        public string GoodsTypeCode { get; set; }
        public string GoodsTypeName { get; set; }
        public string GoodsTypeRemark { get; set; }
        public int GoodsTypeOrder { get; set; }
        public string GoodsTypeFlag { get; set; }

    }
}
