using PlasModel;
using PlasModel.App_Start;
using System;
using System.Web.Caching;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlasCommon;

namespace PlasQueryWeb.Controllers
{
    public class HomeController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        //获取当前登录的用户信息
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        //首页
        public ActionResult Index2()
        {
            if (AccountData != null)
            {
                ViewBag.username = AccountData.UserName;
            }
            else
            {
                ViewBag.username = "";
            }
            return View();
        }

        public ActionResult Index()
        {
            var ds = bll.HotProducts(5);
            if (ds != null && ds.Rows.Count > 0)
            {
                var list = PlasCommon.ToolClass<PlasModel.ProductViewModel>.ConvertDataTableToModel(ds);
                ViewBag.HotList = list;
            }
            return View();
        }

        public ActionResult Home()
        {
            var ds = bll.HotProducts(5);
            if (ds != null && ds.Rows.Count > 0)
            {
                var list = PlasCommon.ToolClass<PlasModel.ProductViewModel>.ConvertDataTableToModel(ds);
                ViewBag.HotList = list;
            }
            return View();
        }

        /// <summary>
        /// 生成JS
        /// </summary>
        /// <returns></returns>
        public JsonResult SetJs()
        {
            string keyword = string.Empty;
            //string strJson = string.Empty;//Json数据
            int count = 0;
            var strJson =new List<wordModel>();
            if (!string.IsNullOrEmpty(Request["keyword"]))
            {
                keyword = Request["keyword"].ToString();
                var list = Comm.FindSearchsWord();

                strJson = list.Where(p => p.Word.ToLower().Contains(keyword.ToLower())).Take(15).ToList();
                count = strJson.Count();
            }
            return Json(new { result = strJson, count= count }, JsonRequestBehavior.AllowGet);
        }


     




    }
}