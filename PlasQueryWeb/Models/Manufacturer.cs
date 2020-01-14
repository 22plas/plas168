using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasQueryWeb.Models
{
    public class Manufacturer
    {
        public string Guid { get; set; }
        public string CreateDate { get; set; }
        public string EnglishName { get; set; }
        public string AliasName { get; set; }
        public string Weight { get; set; }
        public string ShortName { get; set; }
        public string IcoPath { get; set; }
    }
}