using System;
using System.Threading.Tasks;
using Lion.AbpPro.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Lion.AbpPro.EntityFrameworkCore.Users
{
    /// <summary>通过 MySQL 序列行锁串行分配编号，事务提交后永久保留已分配的上限。</summary>
    public class VeinTemplateNumberAllocator : IVeinTemplateNumberAllocator, ITransientDependency
    {
        /// <summary>取得与绑定仓储共享当前工作单元的数据库上下文。</summary>
        private readonly IDbContextProvider<AbpProDbContext> _contexts;

        /// <summary>注入当前工作单元的上下文提供者。</summary>
        public VeinTemplateNumberAllocator(IDbContextProvider<AbpProDbContext> contexts) { _contexts = contexts; }

        /// <inheritdoc />
        public async Task<int> AllocateAsync()
        {
            var context = await _contexts.GetDbContextAsync();
            var transaction = context.Database.CurrentTransaction;
            if (transaction == null) throw new InvalidOperationException("指静脉编号分配必须在编号预留事务中执行。");

            // 更新取得序列行排他锁；同时越过历史手工绑定编号，删除绑定也不降低已分配上限。
            await context.Database.ExecuteSqlRawAsync(@"
UPDATE `BiometricNumberSequence`
SET `LastNumber` = GREATEST(`LastNumber`, (SELECT COALESCE(MAX(`FingerId`), 0) FROM `UserBiometricBinding`))
WHERE `Name` = 'vein'");
            var changed = await context.Database.ExecuteSqlRawAsync(@"
UPDATE `BiometricNumberSequence` SET `LastNumber` = `LastNumber` + 1
WHERE `Name` = 'vein' AND `LastNumber` < 2147483647");
            if (changed != 1)
                throw new UserFriendlyException("无法分配指静脉模板编号，请联系管理员检查编号序列初始化及可用范围。");

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.Transaction = transaction.GetDbTransaction();
            command.CommandText = "SELECT `LastNumber` FROM `BiometricNumberSequence` WHERE `Name` = 'vein'";
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
    }
}
