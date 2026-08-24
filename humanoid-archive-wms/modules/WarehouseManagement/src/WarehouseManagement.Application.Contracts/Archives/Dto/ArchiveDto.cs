using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.Archives.Dto
{
    public class ArchiveDto : AuditedEntityDto<int>
    {
        public string ArchivesName { get; set; }
        public string ArchivesCode { get; set; }
        public string ArchivesRfid { get; set; }
        public string ArchivesUnits { get; set; }
        public string ArchivesRemark { get; set; }
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
        /// 档案盒标签
        /// </summary>
        public string ArchiveBoxRfid { get; set; }
        public string ArchiveBoxName { get; set; }
        public int ArchiveBoxId { get; set; }
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
