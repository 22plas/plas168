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
        //是否
        public enum YesOrNo
        {
            [Display(Name = "否")]
            No,
            [Display(Name = "是")]
            Yes
        }
        //用户积分流水类型
        public enum UserInType
        {
            [Display(Name = "注册")]
            AddUser=0,
            [Display(Name = "查看产品")]
            lookproduct=1,
            [Display(Name = "查看下载物性")]
            lookproductdown = 2,
            [Display(Name = "签到")]
            signin=3,
            [Display(Name = "上传文档")]
            uploaddocuments = 4,

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
        //操作
        public enum Operation
        {
            [Display(Name = "新增")]
            Add,
            [Display(Name = "修改")]
            Update,
            [Display(Name = "删除")]
            Delete,
            [Display(Name = "查询")]
            Select
        }
    }
}
