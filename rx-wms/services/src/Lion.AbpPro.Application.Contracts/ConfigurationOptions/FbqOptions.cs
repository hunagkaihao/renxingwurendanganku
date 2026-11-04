using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.AbpPro.ConfigurationOptions
{
    public class FbqOptions
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        public string Enable { get; set; }
        /// <summary>
        /// Fbq服务器地址
        /// </summary>
        public string Server { get; set; }
    }
}
