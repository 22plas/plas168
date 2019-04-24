using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasCommon
{
   public class Enums
    {
        //用户积分流水类型
        public enum UserInType
        {
            [Display(Name = "新人注册")]
            AddUser=0,
            [Display(Name = "充值")]
            UserRecharge=1
        }
    }
}
