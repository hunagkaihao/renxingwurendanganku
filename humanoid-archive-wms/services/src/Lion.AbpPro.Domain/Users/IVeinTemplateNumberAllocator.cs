using System.Threading.Tasks;

namespace Lion.AbpPro.Users
{
    /// <summary>在数据库事务内分配全库唯一的指静脉模板编号，跨租户共用且不会因解除绑定而回退。</summary>
    public interface IVeinTemplateNumberAllocator
    {
        /// <summary>取得下一个 1～2147483647 的模板编号；必须与编号预留使用同一个事务提交。</summary>
        Task<int> AllocateAsync();
    }
}
