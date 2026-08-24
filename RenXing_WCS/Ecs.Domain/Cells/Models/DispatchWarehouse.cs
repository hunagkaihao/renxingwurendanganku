using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Ecs.Cells.Models
{
    public class DispatchWarehouse : Entity<int>
    {
        [StringLength(50)]
        [Required]
        public string WarehouseName { get; set; } = string.Empty;

        [StringLength(250)]
        public string Description { get; set; }

        protected DispatchWarehouse()
        {

        }

        public DispatchWarehouse(string name, string description)
        {
            WarehouseName = Check.NotNullOrEmpty(name, nameof(name), 50, 1);
            Description = description;
        }
    }
}