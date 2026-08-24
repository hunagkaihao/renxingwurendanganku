using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.AbpPro.ConfigurationOptions
{
    public class AllocationRulesOptions
    {

        public string Enable { get; set; }

        public bool ZBigToSmall { get; set; }
        public bool YBigToSmall { get; set; }
        public bool XBigToSmall { get; set; }

    }
}
