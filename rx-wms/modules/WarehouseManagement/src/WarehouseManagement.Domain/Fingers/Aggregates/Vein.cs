using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace WarehouseManagement.Fingers.Aggregates
{
    public class Vein : FullAuditedAggregateRoot<int>
    {
        public Vein()
        {

        }   
        public string UserId { get; set; }

        public string FingerId { get; set; }

        public Guid? TenantId { get; set; }
    }
}
