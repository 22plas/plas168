using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    /// <summary>
    /// 行情订阅
    /// </summary>
   public class Physics_QuotationModel
    {
        public int Id { get; set; }
        public string ProductGuid { get; set; }
        public string UserId { get; set; }
        public DateTime? CreateDate { get; set; }

    }
}
