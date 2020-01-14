using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasQueryWeb.Models
{
    public class Prices
    {
        public int ID { get; set; }
        public string SmallClass { get; set; }
        public string ManuFacturer { get; set; }
        public string PriceProductGuid { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public int Diff { get; set; }
        public string PriDatestr { get; set; }
    }
}