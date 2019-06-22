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
        public DateTime PriDate { get; set; }
        public string SmallClass { get; set; }
        public string ManuFacturer { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public decimal Diff { get; set; }
        public string PriceProductGuid { get; set; }
        public int SupplierrQty { get; set; }

    }
}
