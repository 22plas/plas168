using PlasBll;
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
    public class PhysicalProductsController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        private PlasBll.ContrastBll pcbll = new PlasBll.ContrastBll();
        protected string MainHost = System.Web.Configuration.WebConfigurationManager.AppSettings["MainHost"];
        protected string PdfUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["PdfUrl"];
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        // GET:超级搜索
        public ActionResult Index()
        {
            //int rowscount = 10;
            var ys_character = new DataTable();
            var texing = new DataTable();
            var zuran = new DataTable();
            var company = new DataTable();
            var jiagong = new DataTable();
            var used = new DataTable();
            var addliao = new DataTable();
            var tianjiaji = new DataTable();


            //种类
            ys_character = bll.GetSearchParam(1, "");
            //特性
            texing = bll.GetSearchParam(2, "");
            //阻燃等级
            zuran = bll.GetSearchParam(3, "");
            //生产厂家
           var  comp = bll.GetSearchParam(4, "");
            if (comp != null && comp.Rows.Count > 0)
            {
                DataTable tblDatas = new DataTable("Datas");
                DataColumn dc = null;
                dc = tblDatas.Columns.Add("SmallGuid", Type.GetType("System.String"));
                dc = tblDatas.Columns.Add("Name", Type.GetType("System.String"));
                DataRow newRow;
                for (var k = 0; k < comp.Rows.Count; k++)
                {
                    var isadd = true;
                    var kvalue = comp.Rows[k]["Name"].ToString();
                    var samllGuid = comp.Rows[k]["SmallGuid"].ToString();
                    if (string.IsNullOrWhiteSpace(kvalue))
                    {
                        kvalue = comp.Rows[k]["EnglishName"].ToString();
                    }

                    for (var j = 0; j < tblDatas.Rows.Count; j++)
                    {
                        if (kvalue == tblDatas.Rows[j]["Name"].ToString())
                        {
                            isadd = false;
                            break;
                        }
                    }
                    if (isadd)
                    {
                        newRow = tblDatas.NewRow();
                        newRow["SmallGuid"] = GetDetail(comp, comp.Rows[k]["Name"].ToString());//comp.Rows[k]["SmallGuid"].ToString();
                        newRow["Name"] = comp.Rows[k]["Name"].ToString();
                        tblDatas.Rows.Add(newRow);
                    }
                    

                }
                company = tblDatas;
                ///DataView dv = new DataView(comp);
                //company = dv.ToTable(true, new string[] { "max(SmallGuid)", "Name" });// comp.Select("max(SmallGuid) as SmallGuid,Name");
            }
            //加工方法
            jiagong = bll.GetSearchParam(5,"");
            //产品用途
            used = bll.GetSearchParam(7, "");
            //填料/增强
            addliao = bll.GetSearchParam(8, "");
            //添加剂
            tianjiaji = bll.GetSearchParam(9, "");

            //属性值
           // var attr = new DataTable();
            
            var attr = bll.Sys_GetSuperSearchParam(-1);
            ViewBag.attr = attr;


            ViewBag.ProductType = ys_character;
            ViewBag.texing = texing;
            //zuran company jiagong used addliao tianjiaji
            ViewBag.zuran = zuran;
            ViewBag.company = company;
            ViewBag.jiagong = jiagong;
            ViewBag.used = used;
            ViewBag.addliao = addliao;
            ViewBag.tianjiaji = tianjiaji;
            return View();
        }

        /// <summary>
        /// 返回重复GUID
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        private string GetDetail(DataTable dt, string strName)
        {
            string retGuid = string.Empty;
            if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrWhiteSpace(strName))
            {
                DataRow[] arrRows = dt.Select("Name='" + strName + "'");
                var datadt = ToDataTable(arrRows);
                if (datadt.Rows.Count > 0)
                {
                    for (var i = 0; i < datadt.Rows.Count; i++)
                    {
                        retGuid += datadt.Rows[i]["SmallGuid"].ToString() + ";";
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(retGuid))
            {
                retGuid = retGuid.Substring(0, retGuid.Length - 1);
            }
            return retGuid;
        }


        private DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone(); // 复制DataRow的表结构
            foreach (DataRow row in rows)
            {
                tmp.ImportRow(row); // 将DataRow添加到DataTable中
            }
            return tmp;
        }





        //产品详情
        public ActionResult Detail(string prodid)
        {
            
            #region 产品详情
            var ds = bll.GetModelInfo(prodid);
            if (ds != null && ds.Tables.Contains("ds") && ds.Tables.Count > 0)
            {
                if (ds.Tables.Contains("ds") &&  ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.ProModel = ds.Tables[0].Rows[0]["proModel"];
                    ViewBag.ProFactory= ds.Tables[0].Rows[0]["cnname"];
                    ViewBag.ProType = ds.Tables[0].Rows[0]["cntype"];
                }
                if (ds.Tables.Contains("ds1") && ds.Tables.Count > 1)
                {
                    //物性
                    ViewBag.PhysicalInfo = ds.Tables[1];
                    //从物性中匹配产品说明
                    for (var i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        if (ds.Tables[1].Rows[i]["lev"].ToString() == "1" && ds.Tables[1].Rows[i]["Attribute1"].ToString().Contains("产品说明"))
                        {
                            if (i + 1 <= ds.Tables[1].Rows.Count && ds.Tables[1].Rows[i + 1]["lev"].ToString() == "2")
                            {
                                ViewBag.description = ds.Tables[1].Rows[i + 1]["Attribute1"].ToString().Replace("\n", "").Replace("\r\n", "").Replace(" ", "");
                            }
                            break;
                        }
                    }

                }
                if (ds.Tables.Count > 2)
                {
                    //详情页标题：种类（Prd_SmallClass_l.Name）+型号（Product.ProModel）+产地（Product.PlaceOrigin）
                    ViewBag.Title2 = ds.Tables[2].Rows[0]["Title"].ToString();
                    //关键字：特性(product_l.characteristic)+用途(product_l.ProUse)
                    ViewBag.Keywords = ds.Tables[2].Rows[0]["keyword"].ToString();
                    //ViewBag.description2 =产品说明(只能用 exec readproduct '0004D924-5BD4-444F-A6D2-045D4EDB0DD3'命令中读出)
                }

            }
            //新增点击次数
            bll.ProductHit(prodid);
            #endregion

            #region 猜你喜欢和同厂家产品
            var LiveDs = bll.GetCompanyAndLiveProduct(10, prodid);
            #endregion

            #region 黄卡UL
            var blist = new List<Ul_HeadModel>();
            //  var clist = new List<Ul_bodyModel>();
            if (!string.IsNullOrWhiteSpace(prodid))
            {
                blist = bll.GetUl_Head(prodid);
                // clist = bll.GetUl_body(ProductGuid);
            }
            ViewBag.blistCount = blist.Count();
            #endregion

            var pdfdt = new DataTable();
            if (!string.IsNullOrEmpty(prodid))
            {
                pdfdt = bll.GetProductPdf(prodid);
            }

            ///添加用户浏览
            if (AccountData != null && !string.IsNullOrWhiteSpace(AccountData.UserID) && !string.IsNullOrWhiteSpace(prodid))
            {
                MemberCenterBll mbll = new MemberCenterBll();
                PlasModel.Physics_BrowseModel mModels = new Physics_BrowseModel();
                mModels.BrowsCount = 1;
                mModels.Btype = 1;
                mModels.CreateDate = DateTime.Now;
                mModels.ProductGuid = prodid;
                mModels.UserId = AccountData.UserID;
                string errMsg = string.Empty;
                mbll.AddPhysics_Browse(mModels, ref errMsg);
            }
            //System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"(?<=^|>)[^<>]+(?=<|$)");
            ViewBag.PdfUrl = PdfUrl;
            ViewBag.ProdID = prodid;
            ViewBag.LiveProdcut = LiveDs;
            ViewBag.pdfdt = pdfdt;
            return View();
        }




        public ActionResult More(int rid, string rname, string more)
        {
            int datatCount = 1000000;
            var allData = new DataTable();
            var restuesDt = new DataTable();
            if (rid > 0)
            {
                allData = bll.GetSearchParam(rid,"", datatCount);
            }
            if (allData != null && allData.Rows.Count > 0)
            {
                DataTable tblDatas = new DataTable("Datas");
                DataColumn dc = null;
                dc = tblDatas.Columns.Add("SmallGuid", Type.GetType("System.String"));
                dc = tblDatas.Columns.Add("AliasName", Type.GetType("System.String"));
                dc = tblDatas.Columns.Add("Name", Type.GetType("System.String"));
                DataRow newRow;
                for (var k = 0; k < allData.Rows.Count; k++)
                {
                    var isadd = true;
                    var alisaName= allData.Rows[k]["AliasName"].ToString();
                    var kvalue = allData.Rows[k]["Name"].ToString();
                    for (var j = 0; j < tblDatas.Rows.Count; j++)
                    {
                        if (kvalue == tblDatas.Rows[j]["Name"].ToString() && alisaName == tblDatas.Rows[j]["AliasName"].ToString())
                        {
                            isadd = false;
                            break;
                        }
                    }
                    if (isadd)
                    {
                        newRow = tblDatas.NewRow();
                        newRow["AliasName"] = allData.Rows[k]["AliasName"].ToString();
                        newRow["SmallGuid"] = GetDetail(allData, allData.Rows[k]["Name"].ToString().Replace("'","''"));//allData.Rows[k]["SmallGuid"].ToString();
                        newRow["Name"] = allData.Rows[k]["Name"].ToString();
                        tblDatas.Rows.Add(newRow);
                    }
                    restuesDt = tblDatas;
                }

                ///DataView dv = new DataView(comp);
                //company = dv.ToTable(true, new string[] { "max(SmallGuid)", "Name" });// comp.Select("max(SmallGuid) as SmallGuid,Name");
            }
            ViewBag.allData = restuesDt;
            ViewBag.rname = rname;
            ViewBag.rid = rid;
            ViewBag.more = more;
            return View();
        }

        public static DataTable Distinct(DataTable dt, string[] filedNames)
        {
            DataView dv = dt.DefaultView;
            DataTable DistTable = dv.ToTable("Dist", true, filedNames);
            return DistTable;
        }


        //属性值
        public ActionResult Attribute()
        {
            var attr = new DataTable();
            attr = bll.Sys_GetSuperSearchParam(-1).Tables[0];
            ViewBag.attr = attr;
            return View();
        }

        /// <summary>
        /// 超级搜素
        /// </summary>
        /// <returns></returns>
        public JsonResult SuperMsgSearch(int pageindex, int pagesize, string guidstr)
        {
            string searchstr = string.Empty;//关键词
            if (!string.IsNullOrWhiteSpace(Request["searchstr"]))
            {
                searchstr = Request["searchstr"].ToString();
            }
            string isNavLink = string.Empty;
            if (!string.IsNullOrEmpty(Request["isNavLink"]))
            {
                isNavLink = Request["isNavLink"].ToString();
            }

            var ds = bll.Sys_SuperSearch(searchstr, 2052, pageindex, pagesize, guidstr, isNavLink);
            string jsonstr = PlasCommon.ToolHelper.DataTableToJson(ds.Tables[0]);
            int count = 0;
            if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                int.TryParse(ds.Tables[1].Rows[0]["totalcount"].ToString(), out count);
            }
            return Json(new { data = jsonstr, totalCount = count }, JsonRequestBehavior.AllowGet);
        }



        public DataTable DtSortA(DataTable Dt, string column)
        {
            DataTable dtNew = Dt.Clone();
            dtNew.Columns[column].DataType = typeof(DateTime);
            foreach (DataRow s in Dt.Rows)
            {
                dtNew.ImportRow(s);//导入旧数据
            }
            dtNew.DefaultView.Sort = column + "";
            dtNew = dtNew.DefaultView.ToTable();
            return dtNew;
        }


        /// <summary>
        /// 查询单位
        /// </summary>
        /// <param name="bigname"></param>
        /// <param name="samllname"></param>
        /// <returns></returns>
        public JsonResult GetUnitList(string bigname,string samllname)
        {
            var list = new List<PlasModel.unitModels>();
            if (!string.IsNullOrWhiteSpace(bigname) && !string.IsNullOrWhiteSpace(samllname))
            {
                var query = bll.GetUnitModels(bigname.Replace("'","''"), samllname.Replace("'", "''"));
                if (query != null)
                {
                    list = query;
                }
            }
            return Json(new { list= list },JsonRequestBehavior.AllowGet);
        }

        #region 生成PDF
        public string ViewPdf(string prodid, string prodModel = "")
        {
            if (string.IsNullOrWhiteSpace(prodModel))
                return "";
            //prodModel = HttpUtility.UrlEncode(prodModel);
            //prodModel = prodModel.Replace(" ","+");
            var ds = bll.GetModelInfo(prodid);
            string pmodel = string.Empty;
            string placeorigin = string.Empty;
            string brand = string.Empty;
            string icopath = string.Empty;
            if (ds.Tables[0].Rows.Count > 0)
            {
                pmodel = ds.Tables[0].Rows[0]["proModel"].ToString();
                placeorigin = ds.Tables[0].Rows[0]["PlaceOrigin"].ToString();
                brand = ds.Tables[0].Rows[0]["Brand"].ToString();
                icopath= ds.Tables[0].Rows[0]["IcoPath"].ToString();
            }
            string pdfUrl = "pdf/" + prodid + ".pdf";
            bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewDetail?prodid=" + prodid, pdfUrl, Server.UrlEncode(pmodel), Server.UrlEncode(placeorigin), Server.UrlEncode(brand),"0", icopath);
            if (success)
                return pdfUrl;
            return "";
        }

        /// <summary>
        /// 用于生成pdf的页面，只保留产品信息 
        /// </summary>
        /// <param name="prodid"></param>
        /// <returns></returns>
        public ActionResult ViewDetail(string prodid)
        {
            var ds = bll.GetModelInfo(prodid);
            if (ds != null && ds.Tables.Count > 0)
            {
                if ( ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.ProModel = ds.Tables[0].Rows[0]["proModel"];
                    ViewBag.PlaceOrigin = ds.Tables[0].Rows[0]["PlaceOrigin"];
                    ViewBag.Brand = ds.Tables[0].Rows[0]["Brand"];
                }
                if ( ds.Tables.Count > 1)
                {
                    //物性
                    ViewBag.PhysicalInfo = ds.Tables[1];
                }
            }
            return View();
        }

        /// <summary>
        /// 用于生成物性对比pdf的页面
        /// </summary>
        /// <param name="prodid"></param>
        /// <returns></returns>
        /// , string title1, string title2, string title3
        public ActionResult ContrastPDF(string contsval, string title1, string title2, string title3)
        {
            ViewBag.title1 = title1;
            ViewBag.title2 = title2;
            ViewBag.title3 = title3;
            DataTable dt = pcbll.GetContrastList(contsval);
            if (dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(dt.Rows[i]["unit"].ToString()))
                    {
                        dt.Rows[i]["unit"] = "(单位：" + dt.Rows[i]["unit"] + ")";
                    }
                    if (!string.IsNullOrWhiteSpace(dt.Rows[i]["testtype"].ToString())&&dt.Rows[i]["testtype"].ToString()!= "测试方法")
                    {
                        dt.Rows[i]["testtype"] = "(测试方法：" + dt.Rows[i]["testtype"] + ")";
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                ViewBag.ContrastInfo = dt;
            }
            return View();
        }

        #endregion

        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetPdfHeaderInfo(string pid)
        {
            try
            {
                var ds = bll.GetModelInfo(pid);
                //string pmodel = string.Empty;
                //string placeorigin = string.Empty;
                //string brand = string.Empty;
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    pmodel = ds.Tables[0].Rows[0]["proModel"].ToString();
                //    placeorigin = ds.Tables[0].Rows[0]["PlaceOrigin"].ToString();
                //    brand = ds.Tables[0].Rows[0]["Brand"].ToString();
                //}
                //var resultdata = new
                //{
                //    pmodelstr = pmodel,
                //    placeoriginstr = placeorigin,
                //    brandstr = brand
                //};
                string jsonstr = PlasCommon.ToolHelper.DataTableToJson(ds.Tables[0]);
                //return Json(Common.ToJsonResult("Success", "获取成功", resultdata), JsonRequestBehavior.AllowGet);
                return Json(new { data = jsonstr}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #region 生成UL


        /// <summary>
        /// 显示UL数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowUlBigPDF(string ProductGuid)
        {
            //  ProductGuid = "1298B2AA-ED90-4B79-8ACB-535704D463FD";
            PlasBll.ProductBll bll = new ProductBll();
            var blist = new List<Ul_HeadModel>();
            //  var clist = new List<Ul_bodyModel>();
            if (!string.IsNullOrWhiteSpace(ProductGuid))
            {
                blist = bll.GetUl_Head(ProductGuid);
                // clist = bll.GetUl_body(ProductGuid);
            }
            ViewBag.blist = blist;
            //ViewBag.clist = clist;
            return View();
        }



        /// <summary>
        /// 显示UL数据详情
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowUlPDF(string ProductGuid)
        {
          //  ProductGuid = "1298B2AA-ED90-4B79-8ACB-535704D463FD";
            PlasBll.ProductBll bll = new ProductBll();
            var blist = new Ul_HeadModel();
            var clist = new List<Ul_bodyModel>();
            if (!string.IsNullOrWhiteSpace(ProductGuid))
            {
                var query = bll.GetUl_HeadNumber(ProductGuid);
                if (query!=null && query.Count > 0)
                {
                    blist = query[0];
                }
                clist = bll.GetUl_body(ProductGuid);

            }
            ViewBag.blist = blist;
            ViewBag.clist = clist;
            return View();
        }


        /// <summary>
        /// 显示PDF生成
        /// </summary>
        /// <param name="ProductGuid"></param>
        /// <returns></returns>
        public ActionResult ViewUl_ShowPdf(string prodid)
        {
            PlasBll.ProductBll bll = new ProductBll();
            var blist = new Ul_HeadModel();
            var clist = new List<Ul_bodyModel>();
            if (!string.IsNullOrWhiteSpace(prodid))
            {
                var query = bll.GetUl_HeadNumber(prodid);
                if (query != null && query.Count > 0)
                {
                    blist = query[0];
                }
                clist = bll.GetUl_body(prodid);

            }
            ViewBag.blist = blist;
            ViewBag.clist = clist;
            return View();
        }


        //
        public string ViewUL_Pdf(string prodid, string prodModel = "")
        {
            if (string.IsNullOrWhiteSpace(prodModel))
                return "";
            //prodModel = HttpUtility.UrlEncode(prodModel);
            //prodModel = prodModel.Replace(" ","+");
            var ds = bll.GetModelInfo(prodid);
            string pmodel = string.Empty;
            string placeorigin = string.Empty;
            string brand = string.Empty;
            if (ds.Tables[0].Rows.Count > 0)
            {
                pmodel = ds.Tables[0].Rows[0]["proModel"].ToString();
                placeorigin = ds.Tables[0].Rows[0]["PlaceOrigin"].ToString();
                brand = ds.Tables[0].Rows[0]["Brand"].ToString();
            }
            string pdfUrl = "pdf/" + prodid + ".pdf";
            bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewUl_ShowPdf?prodid=" + prodid, pdfUrl, string.Empty, string.Empty, string.Empty,"1",string.Empty);
            if (success)
                return pdfUrl;
            return "";
        }
        #endregion


    }
}