using PlasBll;
using PlasCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasModel.Controllers
{
    public class AreaController : Controller
    {
        AreaBll abll = new AreaBll();
        // 获取区域信息
        public ActionResult Getarea(string parentname, string level)
        {
            List<string> list = abll.pliststrbll(parentname, level);
            return Json(Common.ToJsonResult("Success", "获取成功", new { data = list }), JsonRequestBehavior.AllowGet);
        }
    }
}