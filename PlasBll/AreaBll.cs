using PlasDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
    public class AreaBll
    {
        AreaDal adal = new AreaDal();
        public List<string> pliststrbll(string parentname,string level)
        {
            return adal.pliststrdal(parentname,level);
        }
    }
}
