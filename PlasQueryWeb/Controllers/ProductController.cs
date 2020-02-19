using PlasBll;
using PlasCommon;
using PlasModel.App_Start;
using PlasQueryWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasModel.Controllers
{
    public class ProductController : Controller
    {
        //获取当前登录的用户信息
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        CommonBll cbll = new CommonBll();
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // GET:普通搜索
        public ActionResult Index()
        {
            string key = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["key"]))
            {
                key = Request["key"].ToString();
            }
            int keyid = 0;
            if (!string.IsNullOrWhiteSpace(Request["keyid"]))
            {
                int.TryParse(Request["keyid"].ToString(), out keyid);
            }
            //添加累计次数;
            if (keyid > 0)
            {
                new PlasCommon.Common().Addsys_AutokeyHit(keyid);
            }

            ViewBag.key = key;
            return View();
        }


        public JsonResult MsgSearch(int pageindex, int pagesize,int searchtype)
        {
            //如果在没有登录的情况下提示用户登录
            if (pageindex >= 3 && AccountData == null)
            {
                return Json(new { state = "NedLogin", errmsg = "请先登录后在查看！" }, JsonRequestBehavior.AllowGet);
            }
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
            var kindid = "";//分类id
            if (!string.IsNullOrWhiteSpace(Request["kindid"]))
            {
                kindid = Request["kindid"].ToString();
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

            var jsonstr = new List<ProductSharet>();//产品结果集合
            int count = 0;
            //产品属性分为两个
            var  BigType = new List<ProductAttr>();
            var SamllType = new List<ProductAttr>();

            if (!string.IsNullOrWhiteSpace(key)|| searchtype==0)
            {
                DataSet ds = new DataSet();
                if (isTow)//第二次分页，查询数据
                {
                    ds = bll.GetTwoSearch(pageindex, pagesize, strGuid, Characteristic, Use, Kind, Method, Factory, Additive, AddingMaterial,"");
                }
                else
                {
                    //根据分类id查询
                    if (searchtype==0)
                    {
                        ds = bll.GetGeneralSearch(kindid, pageindex, pagesize, strGuid,0, searchtype);
                    }
                    else
                    {
                        ds = bll.GetGeneralSearch(key, pageindex, pagesize, strGuid,0, searchtype);
                    }                    
                }
                if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
                {
                    jsonstr = ToolClass<ProductSharet>.ConvertDataTableToModel(ds.Tables[0]); //ToolHelper.DataTableToJson(ds.Tables[0]);
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
                    //返回list数据，可以解决特殊字符问题
                    BigType = ToolClass<ProductAttr>.ConvertDataTableToModel(dt);//ToolHelper.DataTableToJson(dt);
                    
                    SamllType = ToolClass<ProductAttr>.ConvertDataTableToModel(ds.Tables[2]);// ToolHelper.DataTableToJson(ds.Tables[2]);
                }
            }
            return Json(new { state="Success", data = jsonstr, totalCount = count, BigType = BigType, SamllType = SamllType }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取首页顶部数据(价格走势、新闻、厂家)
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetIndexTopData()
        {
            try
            {
                NewsBll nbll = new NewsBll();
                ProductBll productbll = new ProductBll();
                DataSet priceset = productbll.getPriceFile("", "", "", 1, 5);
                DataTable pricedt = priceset.Tables[0];//首页价格走势数据
                DataTable newdt = nbll.GetNews(1, 5,8);//首页热点资讯新闻数据(顶部)
                DataTable btnewdt = nbll.GetNews(1, 4, 8);//首页热点资讯新闻数据(底部)
                DataTable newggdt = nbll.GetNews(1,5, 7);//首页平台公告新闻数据
                DataTable yyaldt = nbll.GetNews(1, 3, 3);//首页案例数据
                DataTable slhqdt = nbll.GetNews(1, 8, 2);//首页塑料行情
                DataTable manufacturerdt = productbll.GetManufacturer();//首页厂家信息
                DataTable pdfdt = bll.GetListPDF(8);//认证报告数据
                DataTable productdt = bll.GetProductList(8);//物性报告产品数据


                List<pdflist> pdflist = Comm.ToDataList<pdflist>(pdfdt);
                List<tempproduct> prolist= Comm.ToDataList<tempproduct>(productdt);
                List<parminfo> typelist = cbll.listparminfo("", "", "0");
                List<News> listnew = Comm.ToDataList<News>(newdt);
                List<News> yyallistnew = Comm.ToDataList<News>(yyaldt);
                List<News> slhqlistnew = Comm.ToDataList<News>(slhqdt);
                List<News> btlistnew = Comm.ToDataList<News>(btnewdt);
                List<News> gglistnew= Comm.ToDataList<News>(newggdt);
                List<Prices> pricelist = Comm.ToDataList<Prices>(pricedt);
                List<Manufacturer> mlist = Comm.ToDataList<Manufacturer>(manufacturerdt);
                DataTable dt = productbll.GetAnnotationList(16, 1, "", 0);
                var annotationlist = Comm.ToDataList<Annotation>(dt);
                string username = string.Empty;
                string userimg = string.Empty;
                if (AccountData!=null)
                {
                    username = AccountData.UserName;
                    userimg = AccountData.HeadImage;
                }
                var returndata = new
                {
                    newdata = listnew,
                    yyaldata= yyallistnew,
                    slhqldata= slhqlistnew,
                    btnewdata = btlistnew,
                    newggdata = gglistnew,
                    pricedata = pricelist,
                    manufacturerdata = mlist,
                    annotationdata = annotationlist,
                    typelistdata= typelist,
                    pdfdata= pdflist,
                    prodata= prolist,
                    usname= username,
                    usimg= userimg
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        //获取首页二级三级分类
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetIndexClass(string classid)
        {
            try
            {
                List<tempclass> returnlist = new List<tempclass>();
                
                List<parminfo> tempclasslist = new List<parminfo>();
                tempclasslist = cbll.listparminfo(classid, "", "1");
                if (tempclasslist.Count > 0)
                {
                    for (int i = 0; i < tempclasslist.Count; i++)
                    {
                        string tname = tempclasslist[i].Name;
                        tempclass templist = new tempclass();
                        templist.MiddleName = tname;
                        templist.childclasslist = cbll.listparminfo("", tname, "2");
                        returnlist.Add(templist);
                    }
                }
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        public class tempclass {
            public string MiddleName { get; set; }
            public List<parminfo> childclasslist { get; set; }
        }
        public class tempproduct {
            public string ProductGuid { get; set; }
            public string ProModel { get; set; }
            public string AliasName { get; set; }
            public string Name { get; set; }
            public string CreateDate { get; set; }
        }
    }
}