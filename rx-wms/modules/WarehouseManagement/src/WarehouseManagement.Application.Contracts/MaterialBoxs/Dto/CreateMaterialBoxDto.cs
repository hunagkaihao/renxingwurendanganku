using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Domain.Repositories;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.MaterialBoxs.Dto
{
    public class CreateMaterialBoxDto
    {
        /// <summary>
        /// 容器id
        /// </summary>
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// 物料盒名称
        /// </summary>
        public string MaterialBoxName { get; set; }
        /// <summary>
        /// 物料盒RFID
        /// </summary>
        public string MaterialBoxRfid { get; set; }
        /// <summary>
        /// 物料盒编码
        /// </summary>
        public string StockBarcode { get; set; }
        /// <summary>
        /// 物料盒类型
        /// </summary>
        public string CellModel { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public string ClassCode { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 物料保存日期
        /// </summary>
        public string MaterialInDate { get; set; }
        /// <summary>
        /// 物料保存部门
        /// </summary>
        public string MaterialInDept { get; set; }
        /// <summary>
        /// 物料保存负责人
        /// </summary>
        public string MaterialPeople { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string Director { get; set; }
        /// <summary>
        /// 保管期限
        /// </summary>
        public string RetentionPeriod { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string ClassType { get; set; }

    }
}
