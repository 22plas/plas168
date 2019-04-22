using PlasCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace PlasQueryWeb.Controllers
{
    public class PriceController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // GET: 价格
        public ActionResult Index()
        {
            var ys_character = new DataTable();
            var company = new DataTable();
            //种类
            ys_character = bll.GetSearchParam(1);
            //生产厂家
            company = bll.GetSearchParam(4);

            ViewBag.ProductType = ys_character;
            ViewBag.company = company;
            return View();
        }


        public JsonResult GetPriceList(int pageindex, int pagesize)
        {
            var ds = new DataSet();
            //SmallClass = 'ABS' and Manufacturer = '台湾台化' and Model = ''
            string SmallClass = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["SmallClass"]))
            {
                SmallClass = Request["SmallClass"].ToString();
             }
            string Manufacturer = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["Manufacturer"]))
            {
                Manufacturer = Request["Manufacturer"].ToString();
            }
            string Model = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["Model"]))
            {
                Model = Request["Model"].ToString();
            }

            ds = bll.getPriceFile(SmallClass, Manufacturer, Model, pageindex, pagesize);
            string jsonstr = string.Empty;
            int count = 0;
            if (ds.Tables.Contains("ds") && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
            }
            if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                int.TryParse(ds.Tables[1].Rows[0]["totalcount"].ToString(), out count);
            }
            return Json(new { data = jsonstr, totalCount = count}, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetPriceDateList()
        {
            //SmallClass = 'ABS' and Manufacturer = '台湾台化' and Model = ''
            StringBuilder sql = new StringBuilder();
            string jsonstr = string.Empty;
            string SmallClass = string.Empty;
            int count = 0;
            if (!string.IsNullOrWhiteSpace(Request["SmallClass"]))
            {
                SmallClass = Request["SmallClass"].ToString();
                sql.Append(" and SmallClass like '" + SmallClass + "'");
            }
            string Manufacturer = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["Manufacturer"]))
            {
                Manufacturer = Request["Manufacturer"].ToString();
                sql.Append(" and Manufacturer like '" + Manufacturer + "'");
            }
            string Model = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["Model"]))
            {
                Model = Request["Model"].ToString();
                sql.Append(" and Model like '" + Model + "'");
            }
            string priceDate = string.Empty;//距离多少天
            if (!string.IsNullOrEmpty(Request["priceDate"]))
            {
                priceDate = Request["priceDate"].ToString();
                sql.Append(" and  datediff(d,PriDate,getdate())<="+ priceDate);
            }

            var dt = bll.GetPriceLineDt(sql.ToString());

            if (dt != null && dt.Rows.Count > 0)
            {
                jsonstr = ToolHelper.DataTableToJson(dt);
                count = dt.Rows.Count;
            }
            return Json(new { data= jsonstr, totalCount= count },JsonRequestBehavior.AllowGet);
        }


    }
}