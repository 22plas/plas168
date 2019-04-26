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

        /// <summary>
        /// 占位图
        /// </summary>
        public enum DummyImage
        {
            [Display(Name = "默认")]
            Default,
            [Display(Name = "头像")]
            Avatar
        }

        public enum ResizerMode
        {
            Pad,
            Crop,
            Max,
        }

        public enum ReszieScale
        {
            Down,
            Both,
            Canvas
        }
    }
}
