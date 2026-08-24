using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.TaskHiss.Aggregates;

namespace WarehouseManagement.TaskHiss
{
    public interface ITaskHisDetailRepository : IRepository<TaskHisDetail, int>
    {

    }
}
