using Volo.Abp.Domain.Repositories;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.CheckHiss
{
    public interface ICheckHisRepository : IRepository<CheckHis, int>
    {
    }
}
