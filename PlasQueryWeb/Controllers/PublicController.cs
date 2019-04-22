using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class PublicController : Controller
    {
        PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // 公用
        public ActionResult Header()
        {
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