using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasQueryWeb.Models
{
    public class Annotation
    {
        //产品助剂
        public string Model { get; set; }
        public string Supplier { get; set; }
        public string comments { get; set; }
        public string Product_Type { get; set; }
        public string Product_Status { get; set; }
        public int id { get; set; }
    }
}