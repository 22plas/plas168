using PlasCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class ProductController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // GET:普通搜索
        public ActionResult Index()
        {
            string key = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["key"]))
            {
                key = Request["key"].ToString();
            }
            ViewBag.key = key;
            return View();
        }


        public JsonResult MsgSearch(int pageindex, int pagesize)
        {
            string key = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["key"]))
            {
                key = Request["key"].ToString();
            }
            string strGuid = string.Empty; //前端传入的GUID
            if (!string.IsNullOrWhiteSpace(Request["strGuid"]))
            {
                strGuid = Request["strGuid"].ToString();
            }
            //二次查询
            var Characteristic = string.Empty;//特性
            var Use = string.Empty;//用途
            var Kind = string.Empty;//种类
            var Method = string.Empty;//方法
            var Factory = string.Empty;//厂家
            var Additive = string.Empty;//添加剂
            var AddingMaterial = string.Empty;//增料
            var isTow = false;
            if (!string.IsNullOrWhiteSpace(Request["Characteristic"]))
            {
                Characteristic = Request["Characteristic"].ToString();
                isTow = true;
            }
            if (!string.IsNullOrWhiteSpace(Request["Use"]))
            {
                Use = Request["Use"].ToString();
                isTow = true;
            }
            if (!string.IsNullOrWhiteSpace(Request["Kind"]))
            {
                Kind = Request["Kind"].ToString();
                isTow = true;
            }
            if (!string.IsNullOrWhiteSpace(Request["Method"]))
            {
                Method = Request["Method"].ToString();
                isTow = true;
            }
            if (!string.IsNullOrWhiteSpace(Request["Factory"]))
            {
                Factory = Request["Factory"].ToString();
                isTow = true;
            }
            if (!string.IsNullOrWhiteSpace(Request["Additive"]))
            {
                Additive = Request["Additive"].ToString();
                isTow = true;
            }
            if (!string.IsNullOrWhiteSpace(Request["AddingMaterial"]))
            {
                AddingMaterial = Request["AddingMaterial"].ToString();
                isTow = true;
            }

            string jsonstr = string.Empty;
            int count = 0;
            //产品属性分为两个
            string BigType = string.Empty;
            string SamllType = string.Empty;

            if (!string.IsNullOrWhiteSpace(key))
            {
                DataSet ds = new DataSet();
                if (isTow)//第二次分页，查询数据
                {
                    ds = bll.GetTwoSearch(pageindex, pagesize, strGuid, Characteristic, Use, Kind, Method, Factory, Additive, AddingMaterial);
                }
                else
                {
                    ds = bll.GetGeneralSearch(key, pageindex, pagesize, strGuid);
                }
                if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
                {
                    jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                }
                if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    int.TryParse(ds.Tables[1].Rows[0]["totalcount"].ToString(), out count);
                }
                var dt = new DataTable();

                if (ds.Tables.Contains("ds2") && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    DataView dv = ds.Tables[2].DefaultView;
                    dt = dv.ToTable(true, "attribute");
                    BigType = ToolHelper.DataTableToJson(dt);
                    SamllType = ToolHelper.DataTableToJson(ds.Tables[2]);
                }
            }
            return Json(new { data = jsonstr, totalCount = count, BigType = BigType, SamllType = SamllType }, JsonRequestBehavior.AllowGet);
        }
    }
}