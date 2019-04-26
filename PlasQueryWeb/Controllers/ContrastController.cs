using PlasCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasModel.Controllers
{
    public class ContrastController : Controller
    {
        private PlasBll.ContrastBll bll = new PlasBll.ContrastBll();
        // GET: 物料对比
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 选择产品
        /// </summary>
        /// <returns></returns>
        public ActionResult More()
        {
            return View();
        }

        public JsonResult MoreDataList(int pageindex, int pagesize)
        {
            var ds = bll.GetProductList(pagesize, pageindex);
            string jsonstr = string.Empty;
            int count = 0;
            if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
            {
                jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
            }

            if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                int.TryParse(ds.Tables[1].Rows[0]["totalcount"].ToString(), out count);
            }

            return Json(new { data = jsonstr, totalCount = count }, JsonRequestBehavior.AllowGet);
        }

    }
}