using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
    public class ModProductDal
    {
        /// <summary>
        /// 获取改新厂列表
        /// </summary>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页条数</param>
        /// <returns></returns>
        public DataTable GetModProductList(int pageindex, int pagesize)
        {
            int strnumber = ((pageindex - 1) * pagesize) - 1;
            int endnumber = pageindex * pagesize;
            string sql = string.Format(@"SELECT * FROM (SELECT row_number()over(order by Id) as rowNumber,* FROM Mod_Product) a WHERE a.rowNumber BETWEEN {0} AND {1}", strnumber, endnumber);
            return SqlHelper.GetSqlDataTable(sql);
        }

        //public 
    }
}
