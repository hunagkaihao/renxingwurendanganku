using System;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.CheckHiss.Aggregates;

namespace WarehouseManagement.CheckHiss
{
    public interface ICheckDetailHisRepository : IRepository<CheckDetailHis, int>
    {
    }
}
