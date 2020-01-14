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
        public ActionResult Detail()
        {
            Sidebar();
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
    }
}