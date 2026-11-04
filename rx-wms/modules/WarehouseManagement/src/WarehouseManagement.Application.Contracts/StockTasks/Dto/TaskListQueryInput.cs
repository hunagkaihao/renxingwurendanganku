using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lion.AbpPro.Extension.Customs.Dtos;

namespace WarehouseManagement.StockTasks.Dto
{
    /// <summary>
    /// 第三方任务清单查询参数，仅公开分页、条码关键字、创建时间和任务状态。
    /// </summary>
    public class TaskListQueryInput : IValidatableObject
    {
        /// <summary>页码，从 1 开始，默认 1。</summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>每页条数，默认 10，最大值沿用系统分页限制。</summary>
        public int PageSize { get; set; } = 10;

        /// <summary>物料条码关键字；为空时不筛选。</summary>
        public string Filter { get; set; }

        /// <summary>创建时间下限，包含边界；未传时不限制。</summary>
        public DateTime StartCreationTime { get; set; }

        /// <summary>创建时间上限，包含边界；未传时不限制。</summary>
        public DateTime EndCreationTime { get; set; }

        /// <summary>任务状态枚举名称；All 或空值表示不按状态筛选。</summary>
        public string TaskStatus { get; set; }

        /// <summary>沿用系统分页校验规则和错误消息。</summary>
        /// <param name="validationContext">当前请求的验证上下文。</param>
        /// <returns>分页参数的验证错误。</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new PagingBase(PageIndex, PageSize).Validate(validationContext);
        }
    }
}
