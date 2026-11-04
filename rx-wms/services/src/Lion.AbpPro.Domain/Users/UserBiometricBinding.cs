using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Lion.AbpPro.Users
{
    /// <summary>WMS 用户与人脸人员编号、指静脉模板编号的数据库关联，不保存照片或特征模板。</summary>
    public class UserBiometricBinding : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        /// <summary>关联的 AbpUsers.Id；使用稳定主键，账号改名后绑定仍有效。</summary>
        public Guid UserId { get; private set; }

        /// <summary>所属租户 GUID；为空表示宿主，必须与关联用户的租户一致。</summary>
        public Guid? TenantId { get; private set; }

        /// <summary>人脸录入使用的正整数人员编号；未绑定人脸时为空。</summary>
        public int? FaceId { get; private set; }

        /// <summary>指静脉设备内的正整数模板编号；未绑定指静脉时为空。</summary>
        public int? FingerId { get; private set; }

        /// <summary>是否允许通过本组编号登录；停用后保留编号归属，解除绑定需删除记录。</summary>
        public bool IsEnabled { get; private set; }

        /// <summary>供 EF Core 从数据库还原绑定。</summary>
        protected UserBiometricBinding() { }

        /// <summary>为指定用户创建一组绑定，至少提供一种已录入的正整数编号。</summary>
        public UserBiometricBinding(Guid id, Guid userId, Guid? tenantId, int? faceId, int? fingerId, bool isEnabled = true)
            : base(id)
        {
            if (userId == Guid.Empty) throw new UserFriendlyException("请选择有效的 WMS 用户。");
            UserId = userId;
            TenantId = tenantId;
            UpdateIdentifiers(faceId, fingerId, isEnabled);
        }

        /// <summary>更新识别编号和启用状态；不能借此更换绑定用户或租户。</summary>
        /// <param name="faceId">正整数人脸人员编号，未录入时为空。</param>
        /// <param name="fingerId">正整数设备模板编号，未录入时为空。</param>
        /// <param name="isEnabled">是否允许本组编号登录。</param>
        public void UpdateIdentifiers(int? faceId, int? fingerId, bool isEnabled)
        {
            if ((!faceId.HasValue && !fingerId.HasValue) || faceId <= 0 || fingerId <= 0)
                throw new UserFriendlyException("至少绑定一种识别编号，且编号必须为正整数。");
            FaceId = faceId;
            FingerId = fingerId;
            IsEnabled = isEnabled;
        }
    }
}
