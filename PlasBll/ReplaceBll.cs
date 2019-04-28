using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
   public class ReplaceBll
    {

        private PlasDal.ReplaceDal dal = new PlasDal.ReplaceDal();

        //查找替换产品
        public DataSet GetReplace(string SourceId, string ver, string UserId, string WhereString)
        {
            return dal.GetReplace(SourceId, ver, UserId, WhereString);
        }

    }
}
