using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    /// <summary>
    /// 检索关键词
    /// </summary>
    public class wordModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 提取关键词
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 映照前端JS
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 分类，根据型号跳转页面
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductGuid { get; set; }
    }
}
