using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace WarehouseManagement.Goodss.Aggregates
{
    public class GoodsClass : Entity<int>
    {
        public int GoodsClassParentId { get; set; }
        public GoodsType GoodsType { get; set; }
        public int GoodsTypeId { get; set; }
        public string GoodsClassCode { get; set; }
        public string GoodsClassName { get; set; }
        public string GoodsClassRemark { get; set; }
        public int GoodsClassOrder { get; set; }
        public string GoodsClassFlag { get; set; }

    }
}
