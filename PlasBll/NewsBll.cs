using PlasDal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
    public class NewsBll
    {
        NewsDal dal = new NewsDal();
        //获取案例
        public DataTable GetNews(int pageindex, int pagesize, int? type = 3)
        {
            return dal.GetNews(pageindex, pagesize, type);
        }
        //获取案例详情
        public DataTable GetNewsDetail(int ID)
        {
            return dal.GetNewsDetail(ID);
        }
        //获取新闻分类
        public DataTable GetNewClass()
        {
            return dal.GetNewClass();
        }
        //获取新闻首页顶部数据
        public DataTable GetNewsIndexTopData()
        {
            return dal.GetNewsIndexTopData();
        }
        //获取新闻首页最新新闻数据
        public DataTable GetNewsIndexDataList()
        {
            return dal.GetNewsIndexDataList();
        }
        //获取新闻首页最新排行数据
        public DataTable GetNewsIndexDataListOrderByHitAndTime()
        {
            return dal.GetNewsIndexDataListOrderByHitAndTime();
        }
        //获取新闻首页热点推荐
        public DataTable GetNewsIndexDataListByHot()
        {
            return dal.GetNewsIndexDataListByHot();
        }
    }
}
