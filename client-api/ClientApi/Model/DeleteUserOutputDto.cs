using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    public class DeleteUserOutputDto
    {
        public int ret_code { get; set; }
        public int RequestValue { get; set; }
        public string log { get; set; }
        public string user_id { get; set; }
        public DeleteUserOutputDto(int retCode, string userId,  string _log)
        {
            ret_code = retCode;
            log = _log;
            user_id = userId;
            RequestValue = 2819;
        }
    }
}
