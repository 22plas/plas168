using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
   public class parmmodel
    {
        public int id { get; set; }
        public string ParentID { get; set; }
        public string ParentGuid { get; set; }
        public string FaceKey { get; set; }
        public string MinValueFaceKey { get; set; }
        public string MaxValueFaceKey { get; set; }
        public string UnitFaceKey { get; set; }
        public string RangeFaceKey { get; set; }
        public string Realkey { get; set; }
        public string Ts { get; set; }
        public string realkeygroup { get; set; }
    }
}
