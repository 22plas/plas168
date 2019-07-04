using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
   public class NewsDal
    {
        //获取案例
        public DataTable GetNews(int pageindex, int pagesize)
        {
            int pageBegin = 1;
            int pageEnd = 1;
            if (pageindex == 0)
            {
                pageindex = 1;
            }
            if (pagesize == 0)
            {
                pagesize = 1;
            }
            pageBegin = (pageindex - 1) * pagesize;
            pageEnd = (pagesize * pageindex);
            string sql = string.Format(@"SELECT * FROM ( SELECT ROW_NUMBER() over(order by ID desc)as rownum,Title,HomeImg,CreateDate,ID,DescMsg FROM dbo.News ) s WHERE s.rownum BETWEEN {0} AND {1}",  pageBegin, pageEnd);
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
        //获取案例详情
        public DataTable GetNewsDetail(int ID)
        {
            string sql = string.Format(@"SELECT ContentAll FROM dbo.News WHERE ID={0}", ID);
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
    }
}
