using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    public partial class ProductViewModel
    {
        public string BigClassId { get; set; }
        public string SmallClassId { get; set; }
        public bool isShow { get; set; }
        public string ProModel { get; set; }
        public string PlaceOrigin { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ProductGuid { get; set; }
        public int HitCount { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string Brand { get; set; }
        public string Edition { get; set; }
        public string classname { get; set; }
        public string UlAddress { get; set; }
        public int ZrMin { get; set; }
        public int ZrMax { get; set; }
    }
}
