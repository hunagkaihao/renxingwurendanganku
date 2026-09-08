using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Material.Dto
{
    public class MaterialDto : AuditedEntityDto<int>
    {
        /// <summary>
        /// 物料名
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 物料码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料Rfid
        /// </summary>
        public string MaterialRfid { get; set; }
        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnits { get; set; }
        /// <summary>
        /// 物料备注
        /// </summary>
        public string MaterialRemark { get; set; }
        /// <summary>
        /// 物料常量属性1
        /// </summary>
        public string GoodsConstProperty1 { get; set; }
        /// <summary>
        /// 物料常量属性2
        /// </summary>
        public string GoodsConstProperty2 { get; set; }
        /// <summary>
        /// 物料常量属性3
        /// </summary>
        public string GoodsConstProperty3 { get; set; }
        /// <summary>
        /// 物料常量属性4
        /// </summary>
        public string GoodsConstProperty4 { get; set; }
        /// <summary>
        /// 物料常量属性5
        /// </summary>
        public string GoodsConstProperty5 { get; set; }
        /// <summary>
        /// 物料常量属性6
        /// </summary>
        public string GoodsConstProperty6 { get; set; }
        /// <summary>
        /// 物料常量属性7
        /// </summary>
        public string GoodsConstProperty7 { get; set; }
        /// <summary>
        /// 物料常量属性8
        /// </summary>
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
        /// 物料容器标签
        /// </summary>
        public string MaterialBoxRfid { get; set; }
        /// <summary>
        /// 物料容器名
        /// </summary>
        public string MaterialBoxName { get; set; }
        /// <summary>
        /// 物料容器id
        /// </summary>
        public int MaterialBoxId { get; set; }
        public string GoodsAJCode { get; set; }
        public string GoodsNo { get; set; }
        public string ClassCode { get; set; }
        public string Director { get; set; }
        public string ChenWendate { get; set; }
        public string Pages { get; set; }
        public string RetentionPeriod { get; set; }
        public string AchieveInDept { get; set; }
        public string AchieveInDate { get; set; }
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
