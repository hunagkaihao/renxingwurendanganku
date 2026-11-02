using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Lion.AbpPro.Users;
using Volo.Abp.Identity;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Lion.AbpPro.EntityFrameworkCore
{
    public static class AbpProDbContextModelCreatingExtensions
    {
        public static void ConfigureAbpPro(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<UserBiometricBinding>(b =>
            {
                b.ToTable("UserBiometricBinding").HasComment("WMS 账号与人脸人员编号、指静脉模板编号的登录关联");
                b.ConfigureByConvention();
                b.Property(x => x.Id).HasComment("绑定记录 GUID");
                b.Property(x => x.UserId).IsRequired().HasComment("关联 AbpUsers.Id，账号改名不影响绑定");
                b.Property(x => x.TenantId).HasComment("所属租户 GUID，NULL 表示宿主");
                b.Property(x => x.FaceId).HasComment("人脸人员编号，正整数，未绑定时为 NULL");
                b.Property(x => x.FingerId).HasComment("指静脉设备模板编号，正整数，未绑定时为 NULL");
                b.Property(x => x.IsEnabled).HasDefaultValue(true).ValueGeneratedNever()
                    .HasComment("是否允许本组编号登录，停用仍保留编号归属");
                b.Property(x => x.CreationTime).HasComment("绑定创建时间");
                b.Property(x => x.CreatorId).HasComment("创建绑定的用户 GUID，系统操作可为 NULL");
                b.Property(x => x.LastModificationTime).HasComment("最后修改时间，未修改时为 NULL");
                b.Property(x => x.LastModifierId).HasComment("最后修改用户 GUID，未修改时为 NULL");
                b.Property(x => x.ConcurrencyStamp).HasComment("并发更新标记，防止覆盖其他管理员的修改");
                b.Property(x => x.ExtraProperties).HasComment("ABP 扩展属性 JSON");
                // 编号来自同一设备身份库，全库唯一也能覆盖 TenantId 为 NULL 的宿主账号。
                b.HasIndex(x => x.FaceId).IsUnique().HasDatabaseName("UX_UserBiometricBinding_FaceId");
                b.HasIndex(x => x.FingerId).IsUnique().HasDatabaseName("UX_UserBiometricBinding_FingerId");
                b.HasIndex(x => x.UserId).HasDatabaseName("IX_UserBiometricBinding_UserId");
                b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasCheckConstraint("CK_UserBiometricBinding_Identifiers",
                    "(`FaceId` IS NOT NULL OR `FingerId` IS NOT NULL) AND (`FaceId` IS NULL OR `FaceId` > 0) AND (`FingerId` IS NULL OR `FingerId` > 0)");
            });

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(AbpProConsts.DbTablePrefix + "YourEntities", AbpProConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}
