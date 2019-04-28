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
        public DataSet GetProductList(int pageSize = 10, int pageIndex = 1,string wherestr="")
        {
           var  ds= Sys_GetSearchParam("Product", pageSize, pageIndex, " ProModel,PlaceOrigin,ProductGuid ", "and isShow=1 "+ wherestr, " HitCount desc,ModifyDate desc");
            return ds;
        }

        //查询对比数据
        public DataTable GetContrastList(string wherestr)
        {
            //exec prodcompare '678245F3-C804-4F7B-9F38-5E34C2E8AE65;FF221977-3A2E-4277-B17C-60DCEF0CE981;1C2107F6-45BD-45D9-A023-B0CC921F6A5B'
            string sql = string.Format("exec prodcompare '{0}'", wherestr);
            var dt = SqlHelper.GetSqlDataTable(sql.ToString());
            return dt;
        }
    }
}
