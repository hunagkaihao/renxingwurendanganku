using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Plans.Dto
{
    public class CreatePlanDetailInput
    {
        public int PlanId { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseCode { get; set; }
        /// <summary>
        /// 物料ID
        /// </summary>
        public int GoodsId { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string GoodsCode { get; set; }
    }
}
