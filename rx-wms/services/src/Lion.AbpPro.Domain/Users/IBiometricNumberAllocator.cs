using System.Threading.Tasks;

namespace Lion.AbpPro.Users
{
    /// <summary>在数据库事务内按特征类型分配全库唯一编号，跨租户共用且不会因解除绑定而回退。</summary>
    public interface IBiometricNumberAllocator
    {
        /// <summary>取得下一个 1～2147483647 的编号；必须与绑定预留使用同一个事务提交。</summary>
        /// <param name="method">face 为人脸人员编号，vein 为指静脉模板编号，两种类型分别递增。</param>
        Task<int> AllocateAsync(string method);
    }
}
