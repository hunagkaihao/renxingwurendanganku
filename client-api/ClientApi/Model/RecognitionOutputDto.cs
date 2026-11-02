using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    public class RecognitionOutputDto
    {
        public int ret_code { get; set; }
        public int RequestValue { get; set; }
        public string log { get; set; }
        public List<UserList> user_list { get; set; }
        public RecognitionOutputDto(int retCode,string userId,double score, string _log)
        {
            ret_code = retCode;
            log = _log;
            RequestValue = 2819;
            user_list = new List<UserList>();
            if (userId != "0")
            {
                user_list.Add(new UserList(userId, score));
            }
        }
    }
}
