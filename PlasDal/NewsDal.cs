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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="type">新闻类型()</param>
        /// <returns></returns>
        public DataTable GetNews(int pageindex, int pagesize,int ?type=3)
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
            string sql = string.Format(@"SELECT * FROM ( SELECT ROW_NUMBER() over(order by ID desc)as rownum,Title,case when HomeImg is null or HomeImg='' then '' else 'http://www.admin.168plas.com'+HomeImg end as HomeImg,
                                        CONVERT(varchar(100), CreateDate, 23) AS CreateDate,ID,DescMsg,BrowseCount FROM dbo.News where states=1 and ClassID={2} ) s WHERE s.rownum BETWEEN {0} AND {1}",  pageBegin, pageEnd,type);
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
        //获取案例详情
        public DataTable GetNewsDetail(int ID)
        {
            string sql = string.Format(@"SELECT ContentAll,BrowseCount,Title,CreateDate FROM dbo.News WHERE ID={0}", ID);
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
        //获取新闻分类
        public DataTable GetNewClass()
        {
            string sql = string.Format(@"SELECT * FROM dbo.NewsClass");
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
    }
}
