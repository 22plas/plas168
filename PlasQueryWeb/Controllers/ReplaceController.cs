using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasModel.Controllers
{
    public class ReplaceController : Controller
    {
        // GET: 替换
        public ActionResult Index()
        {
            //产品编号
            string Rpt = string.Empty;
            if (!string.IsNullOrEmpty(Request["Rpt"]))
            {
                Rpt = Request["Rpt"].ToString();
            }
            return View();
        }
    }
}