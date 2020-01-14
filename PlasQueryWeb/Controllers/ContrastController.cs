using PlasCommon;
using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasModel.Controllers
{
    public class ContrastController : Controller
    {
        private PlasBll.ContrastBll bll = new PlasBll.ContrastBll();

        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        public void Sidebar(string name = "物性对比")
        {
            ViewBag.Sidebar = name;
        }
        // GET: 物料对比
        public ActionResult Index()
        {
            Sidebar();
            PlasBll.MemberCenterBll mbll = new PlasBll.MemberCenterBll();
            var userName = string.Empty;
            string errMsg = string.Empty;
            if (AccountData != null)
            {
                userName = AccountData.UserID;
            }
            var list = new List<PlasModel.Physics_ContrastModel>();
            if (!string.IsNullOrWhiteSpace(userName))
            {
                list= mbll.GetPhysics_Contrast(userName, ref errMsg);

            }
            ViewBag.ContrastList = list;
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
           
            string jsonstr = string.Empty;
            string txtQuery = string.Empty;//查询值
            string sql = string.Empty;
            if (!string.IsNullOrEmpty(Request["txtQuery"]))
            {
                txtQuery = Request["txtQuery"].ToString();
                sql = " and ProModel like ''%" + txtQuery + "%''";
            }
            int count = 3;
            var ds = bll.GetProductList(pagesize, pageindex, sql);
            if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
            {
                jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
            }

            //if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            //{
            //    int.TryParse(ds.Tables[1].Rows[0]["totalcount"].ToString(), out count);
            //}

            return Json(new { data = jsonstr, totalCount = count }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ContrastSearch()
        {
            var dt = new DataTable();
            string contsval = string.Empty;
            if (!string.IsNullOrEmpty(Request["contsval"]))
            {
                contsval = Request["contsval"].ToString();
                dt = bll.GetContrastList(contsval);
                var jsonlist = ToolClass<PlasModel.ContrastModel>.ConvertDataTableToModel(dt);//ToolHelper.DataTableToJson(dt);
                return Json(new { data = jsonlist }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

           
        }


    }
}