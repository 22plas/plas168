using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
   public class cp_Company
    {
        public string Id { get; set; }//公司id
        public string Name { get; set; }//公司名称
        public string Trade { get; set; }//
        public string Province { get; set; }//所在省份
        public string City { get; set; }//所在城市
        public string Address { get; set; }//地址
        public string Contacts { get; set; }//联系人
        public string Tel { get; set; }//座机
        public string Fax { get; set; }//传真
        public string QQ { get; set; }//QQ
        public string logo { get; set; }//公司logo
        public string UserId { get; set; }//用户id
        public string WeChat { get; set; }//微信
        public string Mobile { get; set; }//手机号
        public string Area { get; set; }//城镇
        public string Email { get; set; }//邮箱
        public string AccountOpenBank { get; set; }//开户账号
        public string AccountOpening { get; set; }//开户地址
        public string OpenBank { get; set; }//开户行
        public string CreateDate { get; set; }//创建时间
        public string isdefault { get; set; }//是否默认
        public string  TaxId { get; set; }//税号

    }


    public class cp_CompanyView
    {
        public string Id { get; set; }//公司id
        public string Name { get; set; }//公司名称
        public string Trade { get; set; }//
        public string Province { get; set; }//所在省份
        public string City { get; set; }//所在城市
        public string Address { get; set; }//地址
        public string Contacts { get; set; }//联系人
        public string Tel { get; set; }//座机
        public string Fax { get; set; }//传真
        public string QQ { get; set; }//QQ
        //公司logo
        //public FileUpload ImagePath { get; set; } = new FileUpload
        //{
        //    AutoInit = true,
        //    Max = 5,
        //    Name = "ImagePath",
        //    Server = UploadServer.QinQiu,
        //    Sortable = true,
        //    Type = FileType.Image,
        //};
        public string UserId { get; set; }//用户id
        public string WeChat { get; set; }//微信
        public string Mobile { get; set; }//手机号
        public string Area { get; set; }//城镇
        public string Email { get; set; }//邮箱
        public string AccountOpenBank { get; set; }//开户账号
        public string AccountOpening { get; set; }//开户地址
        public string OpenBank { get; set; }//开户行
        public string CreateDate { get; set; }//创建时间
        public string isdefault { get; set; }//是否默认
        public string TaxId { get; set; }//税号

    }
}
