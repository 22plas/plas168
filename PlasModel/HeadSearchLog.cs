using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasQueryWeb.Models
{
    public class HeadSearchLog
    {
        public string userid { get; set; }
        public string searchstring { get; set; }
        public int hasresult { get; set; }
        public int fid { get; set; }
    }
}