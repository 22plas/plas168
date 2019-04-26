using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
   public class ContrastDal
    {




        //查询公用数据存储过程
        public DataSet Sys_GetSearchParam(string tableName, int pageSize = 10, int pageIndex = 1, string filterColumns = "", string whereStr = "", string orderBy = "")
        {
            string sql = string.Format("exec pr_common_page '{0}','{1}','{2}','{3}',{4},{5}", tableName, filterColumns, whereStr, orderBy, pageIndex, pageSize);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        /// <summary>
        /// 对比查询产品列表More
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public DataSet GetProductList(int pageSize = 10, int pageIndex = 1)
        {
           var  ds= Sys_GetSearchParam("Product", pageSize, pageIndex, " ProModel,PlaceOrigin,ProductGuid ", "and isShow=1", " HitCount desc,ModifyDate desc");
            return ds;
        }


    }
}
