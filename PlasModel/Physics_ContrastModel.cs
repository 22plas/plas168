using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    /// <summary>
    /// 对比
    /// </summary>
    public class Physics_ContrastModel
    {
        public int Id { get; set; }
        public string ProductGuid { get; set; }
        public string UserId { get; set; }
        public DateTime? CreateDate { get; set; }


        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProModel { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string PlaceOrigin { get; set; }

        /// <summary>
        ///名称
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

    }
}
