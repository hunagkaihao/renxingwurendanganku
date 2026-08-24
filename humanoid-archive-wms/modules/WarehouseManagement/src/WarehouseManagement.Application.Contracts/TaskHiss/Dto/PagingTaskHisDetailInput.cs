using Lion.AbpPro.Extension.Customs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace WarehouseManagement.TaskHiss.Dto
{
    public class PagingTaskHisDetailInput : PagingBase
    {
        public int TaskHisId { get; set; }
        public string Filter { get; set; }
    }
}
