using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasModel.Controllers
{
    public class PublicController : Controller
    {
       private  PlasBll.ProductBll bll = new PlasBll.ProductBll();
        //获取当前登录的用户信息
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        // 公用
        public ActionResult Header()
        {
            ViewBag.username = AccountData.UserName;
            return View();
        }

        //子页面导航
        public ActionResult SubPageNav()
        {
            return View();
        }


        /// <summary>
        /// 页脚
        /// </summary>
        /// <returns></returns>
        public ActionResult Bottom()
        {
            return View();
        }

        /// <summary>
        /// 产品搜索
        /// </summary>
        /// <returns></returns>
        public ActionResult PubSearch()
        {
            var ds = bll.HotProducts(5);
            if (ds != null && ds.Rows.Count > 0)
            {
                var list = PlasCommon.ToolClass<PlasModel.ProductViewModel>.ConvertDataTableToModel(ds);
                ViewBag.HotList = list;
            }
            return PartialView();
        }
    }
}