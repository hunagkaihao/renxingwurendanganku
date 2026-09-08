using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Material.Dto
{
    public class CreateMaterialDto
    {
        public int Id { get; set; }
        public string MaterialName { get; set; }
        public string MaterialRfid { get; set; }
        public string MaterialCode { get; set; }
        public string GoodsRemark { get; set; }
        public string GoodsConstProperty1 { get; set; }
        public string GoodsConstProperty2 { get; set; }
        public string GoodsConstProperty3 { get; set; }
        public string GoodsConstProperty4 { get; set; }
        public string GoodsConstProperty5 { get; set; }
        public string GoodsConstProperty6 { get; set; }
        public string GoodsConstProperty7 { get; set; }
        public string GoodsConstProperty8 { get; set; }
        /// <summary>
        /// 开始号
        /// </summary>
        public int StartNo { get; set; }
        /// <summary>
        /// 结束号
        /// </summary>
        public int EndNo { get; set; }
        //RFIDId
        public String RfidId { get; set; }
        /// <summary>
        /// 物料盒标签
        /// </summary>
        public string MaterialBoxRfid { get; set; }
        public string GoodsAJCode { get; set; }
        public string GoodsNo { get; set; }
        public string ClassCode { get; set; }
        public string Director { get; set; }
        public string ChenWendate { get; set; }
        public string Pages { get; set; }
        public string RetentionPeriod { get; set; }
        public string MaterialInDept { get; set; }
        public string MaterialInDate { get; set; }
        public string StorageRemark { get; set; }
        public string KuaiJiZhuTi { get; set; }
        public string Year { get; set; }
        public string ClassType { get; set; }
        /// <summary>
        /// 密级
        /// </summary>
        public string SecretLevel { get; set; }
    }
}
