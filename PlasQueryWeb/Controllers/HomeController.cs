using PlasModel;
using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Index()
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

    }
}