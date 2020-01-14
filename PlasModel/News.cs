using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
   public class News
    {
        public string Title { get; set; }
        public string HomeImg { get; set; }
        public string ID { get; set; }
        public string DescMsg { get; set; }
        public string CreateDate { get; set; }
        public int rownum { get; set; }
        public int BrowseCount { get; set; }
        public string ContentAll { get; set; }
    }
    public class NewsClass
    {
        public int ID { get; set; }
        public string NewsTypeName { get; set; }
    }
}
