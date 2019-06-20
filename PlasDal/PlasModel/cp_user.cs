using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
   public class cp_user
    {
        public int ID { get; set; }
        public string UserName { get; set; }//用户名(账号)
        public string UserPwd { get; set; }//密码
        public string Email { get; set; }//邮箱
        public string Phone { get; set; }//手机号
        public string Address { get; set; }//地址
        public string TestQQ { get; set; }//QQ
        public DateTime CreateDate { get; set; }//注册时间
        public int states { get; set; }//状态
        public string ErrorDate { get; set; }//错误时间
        [Required]
        public int ErrorCount { get; set; }//报错次数
        public string WeChat { get; set; }//微信
        public string ContentAddress { get; set; }//联系地址
        public string LeaderUserName { get; set; }//上级联系人ID
        public string HeadImage { get; set; }//头像
    }
    public class cp_userview
    {
        public int ID { get; set; }
        public string UserName { get; set; }//用户名(账号)
        public string UserPwd { get; set; }//密码
        public string Email { get; set; }//邮箱
        public string Phone { get; set; }//手机号
        public string Address { get; set; }//地址
        public string TestQQ { get; set; }//QQ
        public DateTime CreateDate { get; set; }//注册时间
        public int states { get; set; }//状态
        public string ErrorDate { get; set; }//错误时间
        public int ErrorCount { get; set; }//报错次数
        public string WeChat { get; set; }//微信
        public string ContentAddress { get; set; }//联系地址
        public string LeaderUserName { get; set; }//上级联系人ID
        public string HeadImage { get; set; }//头像
        public string RecommendPhone { get; set; }//推荐人手机号
        public string wxopenid { get; set; }//微信openid
        public string qqopenid { get; set; }//qqopenid
    }
}
