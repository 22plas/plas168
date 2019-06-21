using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    /// <summary>
    /// 趋势价格
    /// </summary>
   public class Pri_DayAvgPriceModel
    {
        public DateTime? PriDate { get; set; }
        public string SmallClass { get; set; }
        public string ManuFacturer { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public decimal Diff { get; set; }
        public string PriceProductGuid { get; set; }
        public int SupplierrQty { get; set; }
    }
}
