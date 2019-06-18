using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlasCommon;
using PlasCommon.SqlCommonQuery;
using PlasModel;
using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class AppApiController : Controller
    {
        protected string PdfUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["PdfUrl"];
        protected string MainHost = System.Web.Configuration.WebConfigurationManager.AppSettings["MainHost"];
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        private PlasBll.ReplaceBll plbll = new PlasBll.ReplaceBll();
        private PlasBll.MemberCenterBll mbll = new PlasBll.MemberCenterBll();
        private PlasBll.ContrastBll cbll = new PlasBll.ContrastBll();

        #region 获取行情走势
        /// <summary>
        /// 获取行情走势
        /// </summary>
        /// <param name="smallclass">类别</param>
        /// <param name="manufacturer">厂家</param>
        /// <param name="model">型号</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetPriceList(string smallclass, string manufacturer, string model, int? pageindex = 1, int? pagesize = 10)
        {
            try
            {
                var ds = new DataSet();
                //SmallClass = 'ABS' and Manufacturer = '台湾台化' and Model = ''
                string SmallClass = string.Empty;
                if (!string.IsNullOrWhiteSpace(smallclass))
                {
                    SmallClass = smallclass.Trim();
                }
                string Manufacturer = string.Empty;
                if (!string.IsNullOrWhiteSpace(manufacturer))
                {
                    Manufacturer = manufacturer.Trim();
                }
                string Model = string.Empty;
                if (!string.IsNullOrWhiteSpace(model))
                {
                    Model = model.Trim();
                }
                ds = bll.getPriceFile(SmallClass, Manufacturer, Model, pageindex.Value, pagesize.Value);
                string jsonstr = string.Empty;
                if (ds.Tables.Contains("ds") && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                }
                return Json(Common.ToJsonResult("Success", "获取成功", jsonstr), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取行情走势里的类别或者厂家
        /// <summary>
        /// 获取行情走势里的类别或者厂家
        /// </summary>
        /// <param name="typestr">获取类别(0：获取类别 1：厂家)</param>
        /// <returns>json字符串</returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetClassOrFactory(string typestr, string key, int? pageindex = 1, int? pagesize = 10)
        {
            try
            {
                List<parminfo> list = new List<parminfo>();
                if (typestr == "0" || typestr == "1")
                {
                    DataSet ds = bll.GetPagePriceTypeOrFactory(key, typestr, pageindex, pagesize);
                    string jsonstr = string.Empty;
                    //获取类别\厂家
                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        //jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                        list = Comm.ToDataList<parminfo>(ds.Tables[0]);
                    }
                    return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DataTable dt = new DataTable();
                    int selecttype = Convert.ToInt32(typestr);
                    if (typestr == "11")
                    {
                        selecttype = 11;
                        dt = bll.GetSearchParam(selecttype, key, pagesize.Value, pageindex.Value);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                parminfo tm = new parminfo();
                                tm.Guid = dt.Rows[i]["guid"].ToString();
                                tm.Name = dt.Rows[i]["Name"].ToString();
                                tm.rownum = 0;
                                list.Add(tm);
                            }
                        }
                    }
                    else
                    {
                        selecttype = Convert.ToInt32(typestr);
                        dt = bll.GetSearchParam(selecttype, key, pagesize.Value, pageindex.Value);
                        list = Comm.ToDataList<parminfo>(dt);
                    }
                    return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取价格走势信息
        /// <summary>
        /// 获取价格走势信息
        /// </summary>
        /// <param name="priceproductguid">id</param>
        /// <param name="bdate">起始时间</param>
        /// <param name="ndate">结束时间</param>
        /// <param name="type">时间类型：0 近七天 1：近一个月 2：近半年 3：自定义</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetPriceDateList(string priceproductguid, string bdate, string ndate,string type)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                string starttime = "";
                string endtime = "";
                if (type == "0")
                {
                    starttime = DateTime.Now.AddDays(-7).ToShortDateString();
                    endtime = DateTime.Now.ToShortDateString();
                }
                else if (type == "1")
                {
                    starttime = DateTime.Now.AddDays(-30).ToShortDateString();
                    endtime = DateTime.Now.ToShortDateString();
                }
                else if (type == "2")
                {
                    starttime = DateTime.Now.AddMonths(6).ToShortDateString();
                    endtime = DateTime.Now.ToShortDateString();
                }
                else {
                    starttime = bdate;
                    endtime = ndate;
                }
                string jsonstr = string.Empty;
                if (!string.IsNullOrEmpty(bdate) && !string.IsNullOrEmpty(ndate))
                {
                    sql.Append(" and pridate>='" + bdate + "' and  pridate<='" + ndate + "'");
                }
                if (!string.IsNullOrWhiteSpace(priceproductguid))
                {
                    sql.Append(" and PriceProductGuid ='" + priceproductguid + "'");
                }

                var dt = bll.GetPriceLineDt(sql.ToString());
                int max = 0;
                int min = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    jsonstr = ToolHelper.DataTableToJson(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int tempmax = Convert.ToInt32(dt.Rows[i]["Price"]);
                        if (max < tempmax)
                        {
                            max = tempmax;
                        }
                        if (min == 0)
                        {
                            min = tempmax;
                        }
                        else
                        {
                            if (min > tempmax)
                            {
                                min = tempmax;
                            }
                        }
                    }
                }
                //返回的数据
                var returndata = new
                {
                    maxstr = max,
                    minstr = min,
                    data = jsonstr
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 根据产品id获取厂家和名称等信息
        /// <summary>
        /// 根据产品id获取厂家和名称等信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetProductFactoryInfo(string pid)
        {
            try
            {
                string factoryname = string.Empty;
                string name = string.Empty;
                DataTable dt = bll.GetProductFactoryInfo(pid);
                if (dt.Rows.Count > 0)
                {
                    factoryname = dt.Rows[0]["AliasName"].ToString();
                    name = dt.Rows[0]["Name"].ToString();
                }
                var returndata = new
                {
                    factory = factoryname,
                    names = name
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取商品详情
        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="prodid">产品id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetDetail(string prodid)
        {
            try
            {
                string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'",prodid);
                DataTable dt = SqlHelper.GetSqlDataTable(sql);
                string pid = "";
                if (dt.Rows.Count>0)
                {
                    pid = dt.Rows[0]["ProductGuid"].ToString();
                }
                #region 产品详情
                var ds = bll.NewGetModelInfo(pid);//"9C37DC9C-E867-46A2-97DF-32A9489BCDC4"
                //var ds = bll.GetModelInfo(prodid);
                string descriptionjsonstr = string.Empty;//说明
                List<tempinfo> listinfo = new List<tempinfo>();
                if (ds != null && ds.Tables.Contains("ds") && ds.Tables.Count > 0)
                {
                    if (ds.Tables.Count >= 1)
                    {
                        DataTable tempdt = ds.Tables[0];
                        string tempstr = "";
                        for (int i = 0; i < tempdt.Rows.Count; i++)
                        {
                            tempinfo model = new tempinfo();
                            string templev = tempdt.Rows[i]["lev"].ToString();
                            string temp1 = tempdt.Rows[i]["Attribute1"].ToString();
                            string temp2 = tempdt.Rows[i]["Attribute2"].ToString();
                            string temp3 = tempdt.Rows[i]["Attribute3"].ToString();
                            string temp4 = tempdt.Rows[i]["Attribute4"].ToString();
                            string temp5 = tempdt.Rows[i]["Attribute5"].ToString();
                            int tempconpan = 0;
                            if (string.IsNullOrWhiteSpace(temp2) && string.IsNullOrWhiteSpace(temp3) && string.IsNullOrWhiteSpace(temp4))
                            {
                                tempconpan = 4;
                            }
                            else if (!string.IsNullOrWhiteSpace(temp2) && string.IsNullOrWhiteSpace(temp3) && string.IsNullOrWhiteSpace(temp4))
                            {
                                tempconpan = 3;
                            }
                            else if (!string.IsNullOrWhiteSpace(temp2) && !string.IsNullOrWhiteSpace(temp3) && string.IsNullOrWhiteSpace(temp4))
                            {
                                tempconpan = 2;
                            }
                            if (templev == "1")
                            {
                                tempstr = temp1;
                            }

                            model.lev = templev;
                            if (tempstr == "基础参数" || tempstr == "产品说明" || tempstr == "总体" || tempstr == "备注")
                            {
                                model.Attribute1 = temp1.Trim();
                                model.Attribute2 = temp2.Trim();
                                model.Attribute3 = temp3.Trim();
                                model.Attribute4 = temp4.Trim();
                                model.Attribute5 = temp5.Trim();
                            }
                            else
                            {
                                model.Attribute1 = temp1.Trim() == "" ? "--" : temp1.Trim();
                                model.Attribute2 = temp2.Trim() == "" ? "--" : temp2.Trim();
                                model.Attribute3 = temp3.Trim() == "" ? "--" : temp3.Trim();
                                model.Attribute4 = temp4.Trim() == "" ? "--" : temp4.Trim();
                                model.Attribute5 = temp5.Trim() == "" ? "--" : temp5.Trim();
                            }
                            model.stylestr = ".boxtable tbody .trcolor";
                            model.colspan = tempconpan;
                            listinfo.Add(model);
                        }
                    }
                }
                DataTable pdfdt = new DataTable();
                List<pdfinfo> pdfinfolist = new List<pdfinfo>();
                if (!string.IsNullOrEmpty(prodid))
                {
                    pdfdt = bll.GetProductPdf(prodid);
                    if (pdfdt.Rows.Count>0)
                    {
                        for (int i = 0; i < pdfdt.Rows.Count; i++)
                        {
                            pdfinfo tm = new pdfinfo();
                            tm.BefromName = pdfdt.Rows[i]["BefromName"].ToString();
                            tm.Guid = pdfdt.Rows[i]["Guid"].ToString();
                            tm.ID = pdfdt.Rows[i]["ID"].ToString();
                            tm.ImagesColor = pdfdt.Rows[i]["ImagesColor"].ToString();
                            tm.PdfPath = PdfUrl+ pdfdt.Rows[i]["PdfPath"].ToString();
                            tm.ProductGuid = pdfdt.Rows[i]["ProductGuid"].ToString();
                            tm.TestType = pdfdt.Rows[i]["TestType"].ToString();
                            tm.TypeName = pdfdt.Rows[i]["TypeName"].ToString();
                            pdfinfolist.Add(tm);
                        }
                    }
                    //pdfinfolist = Comm.ToDataList<pdfinfo>(pdfdt);
                }
                
                //新增点击次数
                //bll.ProductHit(prodid);
                var returndata = new {
                    prodata= listinfo,
                    pdfdata= pdfinfolist
                };
                #endregion
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取产品的pdf文档
        /// <summary>
        /// 获取产品的pdf文档
        /// </summary>
        /// <param name="prodid">产品id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetProductPDF(string prodid)
        {
            try
            {
                string pdfUrl = "pdf/" + prodid + ".pdf";
                bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewDetail?prodid=" + prodid, pdfUrl);
                if (success)
                {
                    string path = MainHost+"/"+pdfUrl;
                    return Json(Common.ToJsonResult("Success", "获取成功", path), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取下拉关键词数据
        /// <summary>
        /// 获取下拉关键词数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetJsonByKey(string key)
        {
            try
            {
                string strJson = string.Empty;//Json数据
                List<wordModel> listkeyinfo = new List<wordModel>();
                string newkey = Comm.ToLower(key);
                if (!string.IsNullOrEmpty(newkey))
                {
                    var list = Comm.FindSearchsWord();
                    listkeyinfo = list.Where(s => s.Word.ToLower().Contains(newkey.Trim().ToLower())).Take(10).ToList();
                }

                //if (!string.IsNullOrEmpty(Request["keyword"]))
                //{
                //    keyword = Request["keyword"].ToString();
                //    var list = Comm.FindSearchsWord();

                //    strJson = list.Where(p => p.Word.ToLower().Contains(keyword.ToLower())).Take(15).ToList();
                //    count = strJson.Count();
                //}

                return Json(Common.ToJsonResult("Success", "生成成功", listkeyinfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "生成失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取产品列表数据信息
        /// <summary>
        /// 获取产品列表数据信息
        /// </summary>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <param name="isfilter">是否过滤</param>
        /// <param name="proid">产品id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppProductReplaceList(int pageindex, int pagesize, int isfilter, string factory, string proid, string ver,string wherestring)
        {
            string sErr = string.Empty;
            string jsonstr = string.Empty;
            int count = 0;
            string companys = string.Empty;
            try
            {
                //if (!string.IsNullOrEmpty(Request["isfilter"]))
                //{
                //    int.TryParse(Request["isfilter"].ToString(), out isfilter);
                //}
                //本次执行运算的唯一版本号
                //string ver = "b07f3ff6-ac47-48f4-c7dc-c8c9befbfd58";// Guid.NewGuid().ToString();
                List<ReplaceResult> returnlist = new List<ReplaceResult>();
                var ds = new DataSet();
                if (!string.IsNullOrWhiteSpace(proid) && !string.IsNullOrWhiteSpace(ver))
                {
                    if (!string.IsNullOrWhiteSpace(wherestring))
                    {
                        ds = plbll.GetReplace(proid, ver, "", wherestring, pageindex, pagesize, "0", "0", "");
                        
                        if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt.Rows.Count>0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    ReplaceResult tmodel = new ReplaceResult();
                                    tmodel.AlikePercent = dt.Rows[i]["ALikePercent"].ToString();
                                    tmodel.TargetGuid= dt.Rows[i]["ProductId"].ToString();
                                    tmodel.Name= dt.Rows[i]["name"].ToString();
                                    tmodel.PlaceOrigin= dt.Rows[i]["PlaceOrigin"].ToString();
                                    tmodel.ProModel = dt.Rows[i]["ProModel"].ToString();
                                    returnlist.Add(tmodel);
                                }
                            }
                        }
                    }
                    else
                    {
                        ds = plbll.GetProductReplace(ver, proid, pageindex, pagesize, isfilter, factory);
                        if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    ReplaceResult tmodel = new ReplaceResult();
                                    tmodel.AlikePercent = dt.Rows[i]["AlikePercent"].ToString();
                                    tmodel.TargetGuid = dt.Rows[i]["TargetGuid"].ToString();
                                    tmodel.Name = dt.Rows[i]["Name"].ToString();
                                    tmodel.PlaceOrigin = dt.Rows[i]["PlaceOrigin"].ToString();
                                    tmodel.ProModel = dt.Rows[i]["ProModel"].ToString();
                                    returnlist.Add(tmodel);
                                }
                            }
                        }
                    }
                    
                    //if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    //{
                    //    int.TryParse(ds.Tables[1].Rows[0]["counts"].ToString(), out count);
                    //}
                    //if (ds.Tables.Contains("ds2") && ds.Tables[2].Rows.Count > 0)
                    //{
                    //    companys = ToolHelper.DataTableToJson(ds.Tables[2]);
                    //}
                    return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //sErr = "违法操作，筛选数据异常！";
                    return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
            //return Json(new { data = jsonstr, count = count, companys = companys, msg = sErr }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取5个热搜型号 
        /// <summary>
        /// 获取5个热搜型号
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetPubHotSearch()
        {
            var ds = bll.HotProducts(5);
            if (ds != null && ds.Rows.Count > 0)
            {
                var list = PlasCommon.ToolClass<PlasModel.ProductViewModel>.ConvertDataTableToModel(ds);
                return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 普通搜索
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="usid">用户id</param>
        /// <param name="key">关键词</param>
        /// <param name="use">用途</param>
        /// <param name="factory">生产厂家</param>
        /// <param name="kind">类别</param>
        /// <param name="method">加工方法</param>
        /// <param name="characteristic">产品特性</param>
        /// <param name="additive">添加剂</param>
        /// <param name="addingmaterial">增料</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppMsgSearch(string ver, string use, string key, string factory, string kind, string method, string characteristic, string additive, string addingmaterial, int pageindex, int pagesize, string isTow)
        {

            string jsonstr = string.Empty;
            List<ClassInfo> classjsonstr = new List<ClassInfo>();//类别json数据
            List<FactoryInfo> factoryjsonstr = new List<FactoryInfo>();//厂家json数据
            List<attributeinfo> otherlist = new List<attributeinfo>();//其他属性
            List<bigtype> bigtypelist = new List<bigtype>();//大类
            List<SearchResult> resultmodellist = new List<SearchResult>();//搜索结果

            if (!string.IsNullOrWhiteSpace(key))
            {
                DataSet ds = new DataSet();
                if (isTow == "1")//第二次分页，查询数据isTow:(0：第一次 1：多次)
                {
                    ds = bll.GetTwoSearch(pageindex, pagesize, ver, characteristic, use, kind, method, factory, additive, addingmaterial);
                }
                else
                {
                    ds = bll.GetGeneralSearch(key.Trim(), pageindex, pagesize, ver,1);
                }
                if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
                {
                    //jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                    resultmodellist = Comm.ToDataList<SearchResult>(ds.Tables[0]);
                }
                
                //DataTable dt = new DataTable();
                //if (ds.Tables.Contains("ds2") && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                //{
                //    DataView dv = ds.Tables[2].DefaultView;
                //    dt = dv.ToTable(true, "attribute");
                //    if (dt.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dt.Rows.Count; i++)
                //        {
                //            string tempstr = dt.Rows[i]["attribute"].ToString();
                //            if (tempstr == "生产厂家")
                //            {
                //                DataTable tempfactory = ds.Tables[2].Select("attribute='生产厂家'").CopyToDataTable();
                //                if (tempfactory.Rows.Count > 0)
                //                {
                //                    for (int j = 0; j < tempfactory.Rows.Count; j++)
                //                    {
                //                        FactoryInfo tfmodel = new FactoryInfo();
                //                        tfmodel.ManuFacturer = tempfactory.Rows[j]["attributevalue"].ToString();
                //                        factoryjsonstr.Add(tfmodel);
                //                    }
                //                    //factoryjsonstr = ToolHelper.DataTableToJson(tempfactory);
                //                }
                //            }
                //            else if (tempstr == "产品种类")
                //            {
                //                DataTable tempclass = ds.Tables[2].Select("attribute='产品种类'").CopyToDataTable();
                //                if (tempclass.Rows.Count > 0)
                //                {
                //                    for (int s = 0; s < tempclass.Rows.Count; s++)
                //                    {
                //                        ClassInfo tcmodel = new ClassInfo();
                //                        tcmodel.SmallClass = tempclass.Rows[s]["attributevalue"].ToString();
                //                        classjsonstr.Add(tcmodel);
                //                    }
                //                    //classjsonstr = ToolHelper.DataTableToJson(tempclass);
                //                }
                //            }
                //            else
                //            {
                //                bigtype bigmodel = new bigtype();
                //                bigmodel.Name = tempstr;
                //                bigtypelist.Add(bigmodel);
                //                DataRow[] dr = ds.Tables[2].Select("attribute='" + tempstr + "'");
                //                if (dr.Length > 0)
                //                {
                //                    for (int r = 0; r < dr.Length; r++)
                //                    {
                //                        attributeinfo tamodel = new attributeinfo();
                //                        tamodel.attribute= dr[r]["attribute"].ToString();
                //                        tamodel.attributevalue = dr[r]["attributevalue"].ToString();
                //                        otherlist.Add(tamodel);
                //                    }
                //                }
                //                //DataTable tempdt = dr.CopyToDataTable();
                //                //if (tempdt.Rows.Count > 0)
                //                //{
                //                //    if (otherjsonstr == "")
                //                //    {
                //                //        otherjsonstr = ToolHelper.DataTableToJson(tempdt);
                //                //    }
                //                //    else
                //                //    {
                //                //        otherjsonstr = otherjsonstr + "," + ToolHelper.DataTableToJson(tempdt);
                //                //    }
                //                //}
                //            }
                //        }
                //    }
                //}
            }
            var returndata = new
            {
                datalist = resultmodellist,
                factorydata = factoryjsonstr,
                classdata = classjsonstr,
                bigtypedata= bigtypelist,
                otherdata = otherlist
            };
            return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 获取产品属性
        /// <summary>
        /// 获取产品属性
        /// </summary>
        /// <param name="ver">版本号</param>
        /// <param name="txname">属性名称</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetAttribute(string ver, string txname)
        {
            try
            {
                DataTable dt = bll.GetProductAttribute(ver, txname);
                List<ClassInfo> classjsonstr = new List<ClassInfo>();//类别json数据
                List<FactoryInfo> factoryjsonstr = new List<FactoryInfo>();//厂家json数据
                List<attributeinfo> list = new List<attributeinfo>();
                if (txname == "生产厂家")
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            FactoryInfo tfmodel = new FactoryInfo();
                            tfmodel.ManuFacturer = dt.Rows[j]["attributevalue"].ToString();
                            factoryjsonstr.Add(tfmodel);
                        }
                    }
                    return Json(Common.ToJsonResult("Success", "成功", factoryjsonstr), JsonRequestBehavior.AllowGet);
                }
                else if (txname == "产品种类")
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int s = 0; s < dt.Rows.Count; s++)
                        {
                            ClassInfo tcmodel = new ClassInfo();
                            tcmodel.SmallClass = dt.Rows[s]["attributevalue"].ToString();
                            classjsonstr.Add(tcmodel);
                        }
                    }
                    return Json(Common.ToJsonResult("Success", "成功", classjsonstr), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    list = Comm.ToDataList<attributeinfo>(dt);
                    return Json(Common.ToJsonResult("Success", "成功", list), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取超级搜索筛选参数
        /// <summary>
        /// 获取超级搜索筛选参数
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetSuperSearchParam(int parentid)
        {
            try
            {
                DataTable dt = bll.Sys_GetSuperSearchParamForApp(parentid);
                string jsonstr = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    jsonstr = ToolHelper.DataTableToJson(dt);
                    return Json(Common.ToJsonResult("Success", "获取成功", jsonstr), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 超级搜素
        /// <summary>
        /// 超级搜素
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppSuperMsgSearch(int pageindex, int pagesize, string guidstr, string searchstr)
        {
            try
            {
                string isNavLink = string.Empty;
                List<SearchResult> resultmodellist = new List<SearchResult>();//搜索结果
                var ds = bll.Sys_SuperSearch(searchstr, 2052, pageindex, pagesize, guidstr, isNavLink);
                //string jsonstr = PlasCommon.ToolHelper.DataTableToJson(ds.Tables[0]);
                resultmodellist = Comm.ToDataList<SearchResult>(ds.Tables[0]);
                return Json(Common.ToJsonResult("Success", "获取成功", resultmodellist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加物料收藏
        /// <summary>
        /// 添加物料收藏
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult AddMaterialCollection(Physics_CollectionModel model)
        {
            try
            {
                string savesql = string.Format("select id from Physics_Collection where ProductGuid='{0}' and UserId='{1}'", model.ProductGuid, model.UserId);
                var dt = SqlHelper.GetSqlDataTable(savesql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return Json(Common.ToJsonResult("IsCollection", "此产品已经收藏"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = "";
                    bool returnresult = mbll.AddPhysics_Collection(model, ref msg);
                    if (returnresult)
                    {
                        return Json(Common.ToJsonResult("Success", "收藏成功"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(Common.ToJsonResult("Fail", "收藏失败"), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 物性对比搜索材料
        /// <summary>
        /// 物性对比搜索材料
        /// </summary>
        /// <param name="txtQuery">查询值</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetProduct(int pageindex, int pagesize, string txtQuery)
        {
            try
            {
                string jsonstr = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(txtQuery))
                {
                    sql = " and ProModel like ''%" + txtQuery + "%''";
                }
                var ds = cbll.GetProductList(pagesize, pageindex, sql);
                if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
                {
                    jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                }
                return Json(Common.ToJsonResult("Success", "获取成功", jsonstr), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
       
        #region 物料对比搜索物料
        /// <summary>
        /// 物料对比搜索物料
        /// </summary>
        /// <param name="contsval">搜索关键词</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppContrastSearch(string contsval)
        {
            try
            {
                List<ContrastModel> jsonlist = new List<ContrastModel>();
                if (!string.IsNullOrWhiteSpace(contsval))
                {
                    DataTable dt = cbll.GetContrastList(contsval);
                    jsonlist = ToolClass<PlasModel.ContrastModel>.ConvertDataTableToModel(dt);
                }
                return Json(Common.ToJsonResult("Success", "获取成功", jsonlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 我的收藏
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetMyCollcation(string userid, string smallclassid, int? pageindex = 1, int? pagesize = 10)
        {
            try
            {
                string msg = "";
                int count=0;
                List<Physics_CollectionModel> listcollection = new List<Physics_CollectionModel>();
                listcollection=mbll.GetPhysics_Collection(userid, smallclassid, pageindex.Value, pagesize.Value, ref count, ref msg);
                return Json(Common.ToJsonResult("Success", "获取成功", listcollection), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 替换详情
        /// <summary>
        /// 替换详情
        /// </summary>
        /// <param name="ProductID">产品id</param>
        /// <param name="Ven">对比产品id</param>
        /// <param name="isuser">是否自定义</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetMaterialReplaceDetail(string ProductID, string Ven,string isuser)
        {
            try
            {
                string jsonstr = "";
                var dt = new DataTable();
                if (!string.IsNullOrWhiteSpace(ProductID) && !string.IsNullOrWhiteSpace(Ven))
                {
                    dt = plbll.GetReplaceDetail(ProductID, Ven, isuser);
                    jsonstr = ToolHelper.DataTableToJson(dt);
                }
                return Json(Common.ToJsonResult("Success", "获取成功", jsonstr), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取自定义权重
        /// <summary>
        /// 获取自定义权重
        /// </summary>
        /// <param name="Rpt"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetReplaceWeightList(string Rpt)
        {
            try
            {
                //产品编号
                string ProTitle = "请选择产品";
                string ProGuid = string.Empty;
                var dt = new DataTable();
                var pt = new DataTable();//替换属性RealKey
                string bigName = string.Empty;
                string samllName = string.Empty;
                string RealKey = string.Empty;
                List<tempinfo> listinfo = new List<tempinfo>();
                if (!string.IsNullOrEmpty(Rpt))
                {
                    var ds = bll.GetModelInfo(Rpt);
                    pt = plbll.GetAttributeAliasList_RealKey();//替换属性RealKey
                    if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                    {
                        ProTitle = ds.Tables[0].Rows[0]["proModel"].ToString();
                        ProGuid = ds.Tables[0].Rows[0]["productid"].ToString();
                    }
                    if (ds.Tables.Contains("ds1") && ds.Tables[1].Rows.Count > 0)
                    {
                        //< !--卿思明:
                        //产品说明；注射; 注射说明; 备注 这些都不参与对比
                        //，说明，加工方法，备注不允许选择-- >
                        //< !--总体参与对比的有（（RoHS 合规性；供货地区；加工方法；树脂ID(ISO 1043)；特性；添加剂；填料 / 增强材料；用途 ）这个是总体里要参与对比的）-->
                        var dr = ds.Tables[1];///此数据要过滤
                        DataTable tblDatas = new DataTable("Datas");

                        DataColumn dc = null;
                        dc = tblDatas.Columns.Add("lev", Type.GetType("System.Int32"));
                        dc = tblDatas.Columns.Add("Attribute1", Type.GetType("System.String"));
                        dc = tblDatas.Columns.Add("Attribute2", Type.GetType("System.String"));
                        dc = tblDatas.Columns.Add("Attribute3", Type.GetType("System.String"));
                        dc = tblDatas.Columns.Add("Attribute4", Type.GetType("System.String"));
                        dc = tblDatas.Columns.Add("Attribute5", Type.GetType("System.String"));
                        dc = tblDatas.Columns.Add("RealKey", Type.GetType("System.String"));
                        string lev = string.Empty;
                        DataRow newRow;
                        for (var i = 0; i < dr.Rows.Count; i++)
                        {
                            tempinfo model = new tempinfo();
                            if ((string.IsNullOrEmpty(dr.Rows[i]["Attribute2"].ToString())
                                && string.IsNullOrEmpty(dr.Rows[i]["Attribute3"].ToString())
                                && string.IsNullOrEmpty(dr.Rows[i]["Attribute4"].ToString())
                                && string.IsNullOrEmpty(dr.Rows[i]["Attribute5"].ToString()) && dr.Rows[i]["Attribute1"].ToString().Trim() != "总体")
                                ||
                                (dr.Rows[i]["Attribute1"].ToString().Trim() == "加工方法"
                                || dr.Rows[i]["Attribute1"].ToString().Trim() == "材料状态"
                                || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "资料 1".Replace(" ", "")
                                || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "搜索 UL 黄卡".Replace(" ", "")
                                || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "UL 黄卡 2".Replace(" ", "")
                                || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "UL文件号".Replace(" ", "")
                                )
                                )
                            {
                            }
                            else
                            {

                                //单独过滤注射
                                if (dr.Rows[i]["Attribute1"].ToString().Trim() == "注射")
                                {
                                    //int.TryParse(dr.Rows[i]["lev"].ToString().Trim(), out lev);//记住注射
                                    lev = "injection";
                                }
                                else
                                {

                                    int count = (1 + Convert.ToInt32(dr.Rows[i]["lev"].ToString().Trim()));
                                    if (count == 3 && lev == "injection")
                                    {

                                    }
                                    else
                                    {
                                        if (count == 2)//后续其他，必须清除，不然会有异常
                                        {
                                            lev = "";
                                        }
                                        newRow = tblDatas.NewRow();
                                        if (dr.Rows[i]["lev"].ToString() == "1")
                                        {
                                            bigName = dr.Rows[i]["Attribute1"].ToString().Trim();
                                        }
                                        else
                                        {
                                            samllName = dr.Rows[i]["Attribute1"].ToString().Trim();
                                        }
                                        DataRow[] rows = pt.Select("Attribute1='" + bigName + "' and Attribute2Alias = '" + samllName + "'");
                                        if (rows.Count() > 0)
                                        {
                                            RealKey = rows[0]["RealKey"].ToString();
                                        }
                                        //newRow["lev"] = dr.Rows[i]["lev"].ToString().Trim();
                                        //newRow["Attribute1"] = dr.Rows[i]["Attribute1"].ToString().Trim();
                                        //newRow["Attribute2"] = dr.Rows[i]["Attribute2"].ToString().Trim();
                                        //newRow["Attribute3"] = dr.Rows[i]["Attribute3"].ToString().Trim();
                                        //newRow["Attribute4"] = dr.Rows[i]["Attribute4"].ToString().Trim();
                                        //newRow["Attribute5"] = dr.Rows[i]["Attribute5"].ToString().Trim();
                                        //newRow["RealKey"] = RealKey;
                                        //tblDatas.Rows.Add(newRow);

                                        model.Attribute1= dr.Rows[i]["Attribute1"].ToString().Trim();
                                        model.Attribute2= dr.Rows[i]["Attribute2"].ToString().Trim();
                                        model.Attribute3 = dr.Rows[i]["Attribute3"].ToString().Trim();
                                        model.Attribute4 = dr.Rows[i]["Attribute4"].ToString().Trim();
                                        model.Attribute5 = dr.Rows[i]["Attribute5"].ToString().Trim();
                                        model.lev = dr.Rows[i]["lev"].ToString().Trim();
                                        model.RealKey = RealKey;
                                        model.bigName = bigName;
                                        listinfo.Add(model);
                                    }
                                }
                            }
                        }
                        dt = tblDatas;
                        //var spdr=dr.Select("Attribute1<>'产品说明' and Attribute1 <> '注射' and Attribute1 <> '备注'")
                    }
                    //if (ds.Tables.Count > 2)
                    //{
                    //    //详情页标题：种类（Prd_SmallClass_l.Name）+型号（Product.ProModel）+产地（Product.PlaceOrigin）
                    //    ViewBag.Title = ds.Tables[2].Rows[0]["Title"].ToString();
                    //    //关键字：特性(product_l.characteristic)+用途(product_l.ProUse)
                    //    ViewBag.Keywords = ds.Tables[2].Rows[0]["keyword"].ToString();
                    //    //ViewBag.description2 =产品说明(只能用 exec readproduct '0004D924-5BD4-444F-A6D2-045D4EDB0DD3'命令中读出)
                    //}
                }
                //string jsonstr = ToolHelper.DataTableToJson(dt);
                var returndata = new
                {
                    physicalinfo = listinfo,
                    promodel = ProTitle,
                    proguid = ProGuid
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 产品类别
        /// <summary>
        /// 产品类别
        /// </summary>
        /// <param name="parentid">上级id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetClass(string parentid, string middlename, string type)
        {
            try
            {
                List<NewClassInfo> listresult = new List<NewClassInfo>();
                DataTable dt = bll.GetClass(parentid, middlename, type);
                listresult = Comm.ToDataList<NewClassInfo>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", listresult), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        //pdf数据
        public class pdfinfo {
            public string TypeName { get; set; }
            public string Guid { get; set; }
            public string ProductGuid { get; set; }
            public string TestType { get; set; }
            public string PdfPath { get; set; }
            public string BefromName { get; set; }
            public string ID { get; set; }
            public string ImagesColor { get; set; }
        }
        //选择参数类
        public class parminfo
        {
            public int rownum { get; set; }
            public string Name { get; set; }
            public string Guid { get; set; }
        }
        //大类
        public class bigtype {
            public string Name { get; set; }
        }

        //产品属性
        public class attributeinfo
        {
            public string attribute { get; set; }
            public string attributevalue { get; set; }//属性值
        }
        //厂家信息
        public class FactoryInfo
        {
            public string ManuFacturer { get; set; }
        }
        //类别
        public class ClassInfo
        {
            public string SmallClass { get; set; }
            public string Parnetid { get; set; }
        }

        //类别
        public class NewClassInfo
        {
            public string Name { get; set; }
            public string parentguid { get; set; }
        }

        //获取商品详情时需要用到该类
        public class tempinfo
        {
            public string lev { get; set; }
            public string Attribute1 { get; set; }
            public string Attribute2 { get; set; }
            public string Attribute3 { get; set; }
            public string Attribute4 { get; set; }
            public string Attribute5 { get; set; }
            public int colspan { get; set; }
            public string stylestr { get; set; }
            public string RealKey { get; set; }
            public string bigName { get; set; }
        }

        //替换结果
        public class ReplaceResult {
            public string AlikePercent { get; set; }
            public string ProModel { get; set; }
            public string Name { get; set; }
            public string PlaceOrigin { get; set; }
            public string TargetGuid { get; set; }
        }

        //搜索结果类
        public class SearchResult
        {
            public int rn { get; set; }
            public int rank { get; set; }
            public string prodid { get; set; }
            public string productid { get; set; }

            public string ProModel { get; set; }
            public string PlaceOrigin { get; set; }
            public string Name { get; set; }
            public string ProUse { get; set; }
            public string characteristic { get; set; }
            public string custguid { get; set; }
            public string isColl { get; set; }
        }
    }
}
