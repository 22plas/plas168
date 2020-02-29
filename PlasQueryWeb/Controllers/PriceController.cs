using PlasCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using PlasModel.App_Start;

namespace PlasModel.Controllers
{
    public class PriceController : Controller
    {
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        public void Sidebar(string name = "价格行情")
        {
            ViewBag.Sidebar = name;
        }
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // GET: 价格
        public ActionResult Index()
        {
            
            var companyAndtype = new DataSet();
            //在Pri_DayAvgPrice 取
            //种类
            //生产厂家

            companyAndtype = bll.GetPriceType(0);
            Sidebar();
            ViewBag.company = companyAndtype;
            return View();
        }
        //获取价格趋势头部搜索条件数据
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetTopParmList()
        {
            try
            {
                DataTable parentparmdt = bll.GetPriceParentParm();
                List<PriceParm> returnlist = new List<PriceParm>();
                if (parentparmdt.Rows.Count > 0)
                {
                    for (int i = 0; i < parentparmdt.Rows.Count; i++)
                    {
                        PriceParm model = new PriceParm();
                        DataTable dt = bll.GetPriceTopChildParm(parentparmdt.Rows[i]["parentname"].ToString());
                        List<ChildPriceParm> tlist = Comm.ToDataList<ChildPriceParm>(dt);
                        for (int j = 0; j < tlist.Count; j++)
                        {
                            if (j>8)
                            {
                                tlist[j].classstr = "none";
                            }
                        }
                        string tempclassstr = "";
                        if (i>1)
                        {
                            tempclassstr = "itemboxclass";
                        }
                        model.tname = "更多 ∨";
                        model.classstr = tempclassstr;
                        model.Name = parentparmdt.Rows[i]["parentname"].ToString();
                        model.childlist = tlist;
                        returnlist.Add(model);
                    }
                    
                }
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        public class PriceParm
        {
            public string Name { get; set; }
            public string classstr { get; set; }
            public List<ChildPriceParm> childlist { get; set; }
            public string tname { get; set; }
                    }
        public class ChildPriceParm
        {
            public string Name { get; set; }
            public string classstr { get; set; }
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
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int count = 0;
            if (!string.IsNullOrWhiteSpace(Request["SmallClass"]))
            {
                SmallClass = Request["SmallClass"].ToString();
                dic.Add("SmallClass", SmallClass);
            }
            string Manufacturer = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["Manufacturer"]))
            {
                Manufacturer = Request["Manufacturer"].ToString();
                dic.Add("Manufacturer", Manufacturer);
            }
            string Model = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["Model"]))
            {
                Model = Request["Model"].ToString();
                dic.Add("Model", Model);
            }
            string priceDate = string.Empty;//距离多少天
            if (!string.IsNullOrEmpty(Request["priceDate"]))
            {
                priceDate = Request["priceDate"].ToString();
                // sql.Append(" and  datediff(d,PriDate,getdate())<=" + priceDate);
                dic.Add("priceDate", priceDate);
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
                //    sql.Append(" and pridate>='" + bdate + "' and  pridate<='" + ndate + "'");
                DateTime start = Convert.ToDateTime(bdate);
                DateTime end = Convert.ToDateTime(ndate);
                TimeSpan sp = end.Subtract(start);
                int days = sp.Days;
                dic.Add("days", days.ToString());
            }
            string errMsg = string.Empty;

            var dt = bll.GetPriceNewList(dic, ref errMsg); //bll.GetPriceLineDt(sql.ToString());

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


        /// <summary>
        /// 订阅行其
        /// </summary>
        /// <returns></returns>
        public JsonResult TrueQuotation(string textval)
        {
            PlasBll.MemberCenterBll mbll = new PlasBll.MemberCenterBll();
            string errMsg = string.Empty;
            bool isadd = false;
            if (!string.IsNullOrWhiteSpace(textval))
            {
                if (AccountData != null && AccountData.UserID != null)
                {
                    PlasModel.Physics_QuotationModel model = new Physics_QuotationModel();
                    model.UserId = AccountData.UserID;
                    model.ProductGuid = textval;
                    isadd= mbll.AddPhysics_Quotation(model, ref errMsg);
                }
                else
                {
                    errMsg = "订阅行情请先登录！";
                }
            }
            else
            {
                errMsg = "订阅号不能未空！";
            }
            return Json(new { errMsg= errMsg, isadd= isadd },JsonRequestBehavior.AllowGet);
        }

    }
}