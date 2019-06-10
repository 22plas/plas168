using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    /// <summary>
    /// 浏览
    /// </summary>
   public class Physics_BrowseModel
    {
        public int Id { get; set; }
        public string ProductGuid { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// 浏览类型1，产品，2新闻，3，其他
        /// </summary>
        public int Btype { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int BrowsCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
    }
}
