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
            string sql = string.Format(@"SELECT ContentAll,BrowseCount,Title,CONVERT(varchar(100), CreateDate, 23) AS CreateDate,HIt,DescMsg FROM dbo.News WHERE ID={0}", ID);
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                int temphit = Convert.ToInt32(dt.Rows[0]["HIt"]);
                string addsql = string.Format("UPDATE dbo.News SET HIt={0} WHERE ID={1}", temphit + 1, ID);
                SqlHelper.ExecuteScalar(SqlHelper.ConnectionStrings, addsql, null);
            }
            return dt;
        }
        //获取新闻分类
        public DataTable GetNewClass()
        {
            string sql = string.Format(@"SELECT * FROM dbo.NewsClass");
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }

        //获取新闻首页顶部数据
        public DataTable GetNewsIndexTopData()
        {
            string sql = string.Format(@"SELECT TOP 1 Title,case when HomeImg is null or HomeImg='' then '' else 'http://www.admin.168plas.com'+HomeImg end as HomeImg,
                                        CONVERT(varchar(100), CreateDate, 23) AS CreateDate,ID,DescMsg,HIt FROM News WHERE HomeImg IS NOT NULL AND HomeImg<>'' AND DescMsg IS NOT NULL ORDER  BY HIt DESC");
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
        //获取新闻首页最新新闻数据
        public DataTable GetNewsIndexDataList()
        {
            string sql = string.Format(@"SELECT * FROM(SELECT TOP 15 Title,case when HomeImg is null or HomeImg='' then '' else 'http://www.admin.168plas.com'+HomeImg end as HomeImg,
                                        CONVERT(varchar(100), CreateDate, 23) AS CreateDate,ID,DescMsg,HIt FROM dbo.News ORDER BY CreateDate DESC) a ORDER BY a.HomeImg DESC");
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }

        //获取新闻首页最新排行数据
        public DataTable GetNewsIndexDataListOrderByHitAndTime()
        {
            string sql = string.Format(@"SELECT TOP 10 Title,case when HomeImg is null or HomeImg='' then '' else 'http://www.admin.168plas.com'+HomeImg end as HomeImg,
                                        CONVERT(varchar(100), CreateDate, 23) AS CreateDate,ID,DescMsg,HIt FROM  dbo.News ORDER BY HIt,CreateDate DESC");
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }

        //获取新闻首页热点推荐
        public DataTable GetNewsIndexDataListByHot()
        {
            string sql = string.Format(@"SELECT TOP 10 Title,case when HomeImg is null or HomeImg='' then '' else 'http://www.admin.168plas.com'+HomeImg end as HomeImg,
                                        CONVERT(varchar(100), CreateDate, 23) AS CreateDate,ID,DescMsg,HIt FROM dbo.News WHERE ClassID=8");
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
    }
}
