using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    /// <summary>删除用户全部人脸样本的请求。</summary>
    public class FaceDeleteDto
    {
        /// <summary>待删除人脸样本的业务用户编号。</summary>
        public string UserId { get; set; }

    }
}
