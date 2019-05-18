using PlasCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace PlasModel.Controllers
{
    public class PriceController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // GET: 价格
        public ActionResult Index()
        {
            var companyAndtype = new DataSet();
            //在Pri_DayAvgPrice 取
            //种类
            //生产厂家

            companyAndtype = bll.GetPriceType(0);
 
            ViewBag.company = companyAndtype;
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
            return Json(new { data = jsonstr, totalCount = count }, JsonRequestBehavior.AllowGet);
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
                sql.Append(" and  datediff(d,PriDate,getdate())<=" + priceDate);
            }

            //开始时间
            string bdate = string.Empty;
            if (!string.IsNullOrEmpty(Request["bdate"]))
            {
                bdate = Request["bdate"].ToString();
            }
            //ndate
            string ndate = string.Empty;
            if (!string.IsNullOrEmpty(Request["ndate"]))
            {
                ndate = Request["ndate"].ToString();
            }

            if (!string.IsNullOrEmpty(bdate) && !string.IsNullOrEmpty(ndate))
            {
                sql.Append(" and pridate>='" + bdate + "' and  pridate<='" + ndate + "'");
            }

            var dt = bll.GetPriceLineDt(sql.ToString());

            if (dt != null && dt.Rows.Count > 0)
            {
                jsonstr = ToolHelper.DataTableToJson(dt);
                count = dt.Rows.Count;
            }
            return Json(new { data = jsonstr, totalCount = count }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult More(int rid, string rname, string more)
        {
            var companyAndtype = new DataSet();
            var allData = new DataTable();
            //在Pri_DayAvgPrice 取
            //种类 --1
            //生产厂家----2
            companyAndtype = bll.GetPriceType(0);
            if (rid == 1)//类别
            {
                allData = companyAndtype.Tables[0];
            }
            else
            {
                allData = companyAndtype.Tables[1];
            }

            ViewBag.allData = allData;
            ViewBag.rname = rname;
            ViewBag.rid = rid;
            ViewBag.more = more;
            return View();
        }


    }
}