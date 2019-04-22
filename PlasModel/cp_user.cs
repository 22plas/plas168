using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
   public class cp_user
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string TestQQ { get; set; }
        public string CreateDate { get; set; }
        public int states { get; set; }
        public string ErrorDate { get; set; }
        public string ErrorCount { get; set; }
        public string WeChat { get; set; }
        public string ContentAddress { get; set; }
        public string LeaderUserName { get; set; }
    }
}
