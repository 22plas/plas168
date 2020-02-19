using PlasBll;
using PlasCommon;
using PlasModel;
using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class NewsController : Controller
    {
        NewsBll bll = new NewsBll();
        public void Sidebar(string name = "行业资讯")
        {
            ViewBag.Sidebar = name;
        }
        // GET: News
        public ActionResult Index()
        {
            Sidebar();
            return View();
        }
        //详情
        public ActionResult Detail(int id)
        {
            Sidebar();
            DataTable dt = bll.GetNewsDetail(id);
            string title = "";
            string dmsg = "";
            if (dt.Rows.Count>0)
            {
                title= dt.Rows[0]["Title"].ToString();
                dmsg = dt.Rows[0]["DescMsg"].ToString();
            }
            ViewBag.Title2 = title;
            ViewBag.Keywords = "塑蚁网-塑料行业资讯";
            ViewBag.description = dmsg;
            return View();
        }
        //获取新闻详情
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetDetailByID(int id)
        {
            try
            {
                DataTable dt = bll.GetNewsDetail(id);
                News model = new News();
                if (dt.Rows.Count > 0)
                {
                    model.BrowseCount = Convert.ToInt32(dt.Rows[0]["BrowseCount"]);
                    model.HIt= Convert.ToInt32(dt.Rows[0]["HIt"]);
                    model.Title = dt.Rows[0]["Title"].ToString();
                    model.ContentAll = dt.Rows[0]["ContentAll"].ToString();
                    model.CreateDate = dt.Rows[0]["CreateDate"].ToString();
                }
                return Json(Common.ToJsonResult("Success", "获取成功", model), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        //获取新闻首页数据
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetNewsData()
        {
            try
            {
                DataTable topnewsdatadt = bll.GetNewsIndexTopData();//顶部热门推荐
                DataTable newsnewdatadt = bll.GetNewsIndexDataList();//最新
                DataTable newggdt = bll.GetNews(1, 4, 7);//首页平台公告新闻数据
                DataTable bzphdt = bll.GetNewsIndexDataListOrderByHitAndTime();//本周排行
                DataTable rdtjdt = bll.GetNewsIndexDataListByHot();//热点推荐

                List<News> topdata = Comm.ToDataList<News>(topnewsdatadt);
                List<News> newdata = Comm.ToDataList<News>(newsnewdatadt);
                List<News> ggdata = Comm.ToDataList<News>(newggdt);
                List<News> bzphdata = Comm.ToDataList<News>(bzphdt);
                List<News> rdtjdata = Comm.ToDataList<News>(rdtjdt);
                var returndata = new
                {
                    toplist = topdata,
                    newlist = newdata,
                    gglist = ggdata,
                    bzphlist = bzphdata,
                    rdtjlist = rdtjdata
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}