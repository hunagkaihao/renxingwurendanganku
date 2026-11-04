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
    public class BiometricNumberAllocator : IBiometricNumberAllocator, ITransientDependency
    {
        /// <summary>取得与绑定仓储共享当前工作单元的数据库上下文。</summary>
        private readonly IDbContextProvider<AbpProDbContext> _contexts;

        /// <summary>注入当前工作单元的上下文提供者。</summary>
        public BiometricNumberAllocator(IDbContextProvider<AbpProDbContext> contexts) { _contexts = contexts; }

        /// <inheritdoc />
        public async Task<int> AllocateAsync(string method)
        {
            // 列名只取固定白名单，编号类型作为 SQL 参数传递，不能拼接调用方提供的任意标识符。
            var column = method == "face" ? "FaceId" : method == "vein" ? "FingerId" : null;
            if (column == null) throw new UserFriendlyException("请选择有效的录入方式。");
            var context = await _contexts.GetDbContextAsync();
            var transaction = context.Database.CurrentTransaction;
            if (transaction == null) throw new InvalidOperationException("特征编号分配必须在编号预留事务中执行。");

            // 首次使用人脸时自动补齐序列行；取得排他锁并越过历史手工绑定编号，删除绑定也不回退。
            await context.Database.ExecuteSqlRawAsync($@"
INSERT INTO `BiometricNumberSequence` (`Name`, `LastNumber`)
SELECT {{0}}, COALESCE(MAX(`{column}`), 0) FROM `UserBiometricBinding`
ON DUPLICATE KEY UPDATE `LastNumber` = GREATEST(`LastNumber`, VALUES(`LastNumber`))", new object[] { method });
            var changed = await context.Database.ExecuteSqlRawAsync(@"
UPDATE `BiometricNumberSequence` SET `LastNumber` = `LastNumber` + 1
WHERE `Name` = {0} AND `LastNumber` < 2147483647", new object[] { method });
            if (changed != 1)
                throw new UserFriendlyException("无法分配特征编号，请联系管理员检查编号序列初始化及可用范围。");

            using var command = context.Database.GetDbConnection().CreateCommand();
            command.Transaction = transaction.GetDbTransaction();
            command.CommandText = "SELECT `LastNumber` FROM `BiometricNumberSequence` WHERE `Name` = @method";
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@method";
            parameter.Value = method;
            command.Parameters.Add(parameter);
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
    }
}
