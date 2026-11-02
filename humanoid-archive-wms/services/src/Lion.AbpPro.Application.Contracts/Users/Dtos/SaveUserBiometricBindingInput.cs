using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lion.AbpPro.Users.Dtos
{
    /// <summary>新增或修改一组用户绑定；用户由路径指定，租户由当前请求确定。</summary>
    public class SaveUserBiometricBindingInput : IValidatableObject
    {
        /// <summary>绑定记录 GUID；新增时为空，修改时使用查询返回的 Id。</summary>
        public Guid? Id { get; set; }
        /// <summary>正整数人脸人员编号；未绑定时为空。</summary>
        [Range(1, int.MaxValue)]
        public int? FaceId { get; set; }
        /// <summary>正整数指静脉设备模板编号；未绑定时为空。</summary>
        [Range(1, int.MaxValue)]
        public int? FingerId { get; set; }
        /// <summary>是否启用本组绑定，默认启用。</summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>修改时使用查询返回的并发标记；新增时不需要。</summary>
        [StringLength(40)]
        public string ConcurrencyStamp { get; set; }

        /// <summary>要求至少一种编号，修改时必须携带有效记录 ID 和并发标记。</summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!FaceId.HasValue && !FingerId.HasValue)
                yield return new ValidationResult("至少绑定一种识别编号。", new[] { nameof(FaceId), nameof(FingerId) });
            if (Id == Guid.Empty)
                yield return new ValidationResult("绑定记录 ID 无效。", new[] { nameof(Id) });
            if (Id.HasValue && string.IsNullOrWhiteSpace(ConcurrencyStamp))
                yield return new ValidationResult("修改绑定需要携带并发标记。", new[] { nameof(ConcurrencyStamp) });
        }
    }
}
