using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    public class Problem
    {
        public int fid { get; set; }
        public string userid { get; set; }
        public string searchstring { get; set; }
        public string createtime { get; set; }
        public int hasresult { get; set; }
        public string completeedstr { get; set; }
        public int completeed { get; set; }
    }
}
