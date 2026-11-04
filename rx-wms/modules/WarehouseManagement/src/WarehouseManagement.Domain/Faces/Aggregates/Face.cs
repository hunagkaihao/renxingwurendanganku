using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WarehouseManagement.Faces.Aggregates
{
    public  class Face : FullAuditedAggregateRoot<int>
    {
        public Face()
        {

        }
        public string UserId { get; set; }
        public string ImageDate { get; set; }

        public Guid? TenantId { get; set; }


    }
}
