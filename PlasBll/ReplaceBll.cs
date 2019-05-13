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
        public DataSet GetReplace(string SourceId, string ver, string UserId, string WhereString,int pageindex=1,int pagesize=20, string isLink = "0",string isfilter="0",string companys="")
        {
            return dal.GetReplace(SourceId, ver, UserId, WhereString, pageindex, pagesize, isLink, isfilter, companys);
        }


        //查找替换里面的RealKey
        public DataTable GetAttributeAliasList_RealKey()
        {
            return dal.GetAttributeAliasList_RealKey();
        }

        //根据产品获取替换数据
        public DataSet GetProductReplace(string ver, string proGuid, int pageno, int pagesize, int isfilter, string companys)
        {
            return dal.GetProductReplace(ver, proGuid, pageno, pagesize, isfilter, companys);
        }
    }
}
