using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    /// <summary>
    /// 普通搜索结果
    /// </summary>
   public class ProductSharet
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string prodid { get; set; }

        /// <summary>
        /// 牌号
        /// </summary>
        public string ProModel { get; set; }

        /// <summary>
        /// 厂家
        /// </summary>
        public string PlaceOrigin { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string ProUse { get; set; }


        /// <summary>
        /// 特性
        /// </summary>
        public string characteristic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string custguid { get; set; }

    }
}
