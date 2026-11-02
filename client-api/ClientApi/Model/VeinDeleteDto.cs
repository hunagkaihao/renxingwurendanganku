using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    /// <summary>指静脉删除请求。</summary>
    public class VeinDeleteDto
    {
        /// <summary>待删除的设备指静脉编号，不能为 0。</summary>
        public int VeinId { get; set; }

    }
}
