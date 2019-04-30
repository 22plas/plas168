using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasCommon.Enums;

namespace PlasModel
{
    /// <summary>
    /// 收货地址
    /// </summary>
   public class DeliveryAddress
    {
        public string Id { get; set; }
        public string UserId { get; set; }//用户ID
        public string Tel { get; set; }//座机电话
        public string Fax { get; set; }//传真(不用)
        [Required(ErrorMessage = "联系人不能为空")]
        public string Contacts { get; set; }//联系人
        [Required(ErrorMessage = "手机号不能为空")]
        public string ContactsMobile { get; set; }//联系人手机号
        public string QQ { get; set; }//qq(不用)
        public string WeChat { get; set; }//微信(不用)
        [Required(ErrorMessage = "请选择省份")]
        public string Province { get; set; }//省份
        [Required(ErrorMessage = "请选择城市")]
        public string City { get; set; }//城市
        [Required(ErrorMessage = "请选择镇区")]
        public string Count { get; set; }//城镇
        [Required(ErrorMessage = "详细地址不能为空")]
        public string Address { get; set; }//详细地址
        public bool IsDefault { get; set; }//是否默认
        public string IsDefaultStr { get; set; }//是否默认(用户列表显示)
    }
}
