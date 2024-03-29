﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlasBll;
using PlasCommon;
using PlasCommon.SqlCommonQuery;
using PlasModel;
using PlasModel.App_Start;
using PlasQueryWeb.Models;
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
        private PlasBll.NewsBll newbll = new PlasBll.NewsBll();
        private ModProductBll mpbll = new ModProductBll();

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
                //获取超级搜填料属性
                else if (typestr == "8")
                {
                    DataTable dt = bll.GetSysfiller("0",key, pageindex.Value, pagesize.Value);
                    list = Comm.ToDataList<parminfo>(dt);
                    return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DataTable dt = new DataTable();
                    int selecttype = Convert.ToInt32(typestr);
                    if (typestr == "11"|| typestr == "111")
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

        #region 获取助剂类别或者厂家
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetAnnotationClassOrFactory(string typestr, string key, int? pageindex = 1, int? pagesize = 10)
        {
            try
            {
                List<parminfo> list = new List<parminfo>();
                DataTable dt = bll.GetAnnotationClassOrFactory(typestr, key, pageindex, pagesize);
                list = Comm.ToDataList<parminfo>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 根据上级名称获取二级填料
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetSysfiller(string key)
        {
            try
            {
                List<parminfo> list = new List<parminfo>();
                DataTable dt = bll.GetSysfiller("1", key, 0, 0);
                list = Comm.ToDataList<parminfo>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
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
        public ActionResult AppGetPriceDateList(string priceproductguid, string bdate, string ndate,string type,string userid)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                int iscollection = 0;//检查产品是否收藏 0：否 1：是
                int isquotation = 0;//是否订阅 0：否 1：是
                string sql2 = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", priceproductguid);
                DataTable dts = SqlHelper.GetSqlDataTable(sql2);
                string pid = string.Empty;
                if (dts.Rows.Count > 0)
                {
                    pid = dts.Rows[0]["ProductGuid"].ToString();
                    //是否收藏
                    string scsql = string.Format(@"SELECT * FROM dbo.Physics_Collection WHERE UserId='{0}' AND ProductGuid='{1}'", userid, pid);
                    DataTable scdt = SqlHelper.GetSqlDataTable(scsql);
                    if (scdt.Rows.Count > 0)
                    {
                        iscollection = 1;
                    }
                    //是否订阅
                    string dysql = string.Format(@"SELECT * FROM dbo.Physics_Quotation WHERE UserId='{0}' AND ProductGuid='{1}'", userid, priceproductguid);
                    DataTable dydt = SqlHelper.GetSqlDataTable(dysql);
                    if (dydt.Rows.Count > 0)
                    {
                        isquotation = 1;
                    }
                }

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
                    sql.Append(" and pridate>='" + starttime + "' and  pridate<='" + endtime + "'");
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
                if (!string.IsNullOrWhiteSpace(userid)&&!string.IsNullOrWhiteSpace(pid))
                {
                    Physics_BrowseModel model = new Physics_BrowseModel();
                    model.BrowsCount = 1;
                    model.Btype = 2;//价格行情浏览
                    model.ProductGuid = pid;
                    model.UserId = userid;
                    string msg = "";
                    mbll.AddPhysics_Browse(model, ref msg);
                }
                //返回的数据
                var returndata = new
                {
                    maxstr = max,
                    minstr = min,
                    data = jsonstr,
                    iscollections= iscollection,
                    isquotations= isquotation
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取价格走势对比信息
        /// <summary>
        /// 获取价格走势对比信息
        /// </summary>
        /// <param name="priceproductguid">id</param>
        /// <param name="bdate">起始时间</param>
        /// <param name="ndate">结束时间</param>
        /// <param name="type">时间类型：0 近七天 1：近一个月 2：近半年 3：自定义</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetPriceDateListContrast(string priceproductguid, string bdate, string ndate, string type)
        {
            try
            {
                string[] liststrpid = priceproductguid.Split(';');
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
                else
                {
                    starttime = bdate;
                    endtime = ndate;
                }
                //返回数据集
                List<PriceContrast> listcontrast = new List<PriceContrast>();
                string jsonstr = string.Empty;
                int max = 0;
                int min = 0;
                for (int i = 0; i < liststrpid.Length; i++)
                {
                    if (!string.IsNullOrEmpty(bdate) && !string.IsNullOrEmpty(ndate))
                    {
                        sql.Append(" and pridate>='" + starttime + "' and  pridate<='" + endtime + "'");
                    }
                    if (!string.IsNullOrWhiteSpace(priceproductguid))
                    {
                        sql.Append(@" and PriceProductGuid in(SELECT PriceProductGuid FROM dbo.Pri_Product WHERE ProductGuid='" + liststrpid[i] + "')");
                    }
                    var dt = bll.GetPriceLineDt(sql.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        PriceContrast model = new PriceContrast();
                        model.pricejsondata = ToolHelper.DataTableToJson(dt);
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            int tempmax = Convert.ToInt32(dt.Rows[j]["Price"]);
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
                        model.MaxPrice = max;
                        model.MinPrice = min;
                        listcontrast.Add(model);
                    }
                }
                return Json(Common.ToJsonResult("Success", "获取成功", listcontrast), JsonRequestBehavior.AllowGet);
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
                    factoryname = dt.Rows[0]["AliasName"].ToString()==""?dt.Rows[0]["EnglishName"].ToString(): dt.Rows[0]["AliasName"].ToString();
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
        public ActionResult AppGetDetail(string prodid, string type, string userid)
        {
            try
            {
                string pid = "";
                if (type == "1")
                {
                    string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", prodid);
                    DataTable dt = SqlHelper.GetSqlDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        pid = dt.Rows[0]["ProductGuid"].ToString();
                    }
                }
                else
                {
                    pid = prodid;
                }
                int iscollection = 0;//检查产品是否收藏 0：否 1：是
                string scsql = string.Format(@"SELECT * FROM dbo.Physics_Collection WHERE UserId='{0}' AND ProductGuid='{1}'", userid, pid);
                DataTable scdt = SqlHelper.GetSqlDataTable(scsql);
                if (scdt.Rows.Count > 0)
                {
                    iscollection = 1;
                }

                #region 产品详情
                var ds = bll.NewGetModelInfo(pid, userid);//"9C37DC9C-E867-46A2-97DF-32A9489BCDC4"
                //var ds = bll.GetModelInfo(prodid);
                string descriptionjsonstr = string.Empty;//说明
                List<tempinfo> listinfo = new List<tempinfo>();
                if (ds != null && ds.Tables.Contains("ds") && ds.Tables.Count > 0)
                {
                    if (ds.Tables.Count >= 1)
                    {
                        DataTable tempdt = ds.Tables[1];
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
                            if (tempstr == "基础参数" || tempstr == "产品说明" || tempstr == "总体" || tempstr == "基本信息" || tempstr == "备注")
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
                    if (pdfdt.Rows.Count > 0)
                    {
                        for (int i = 0; i < pdfdt.Rows.Count; i++)
                        {
                            pdfinfo tm = new pdfinfo();
                            tm.BefromName = pdfdt.Rows[i]["BefromName"].ToString();
                            tm.Guid = pdfdt.Rows[i]["Guid"].ToString();
                            tm.ID = pdfdt.Rows[i]["ID"].ToString();
                            tm.ImagesColor = pdfdt.Rows[i]["ImagesColor"].ToString();
                            tm.PdfPath = PdfUrl + pdfdt.Rows[i]["PdfPath"].ToString();
                            tm.ProductGuid = pdfdt.Rows[i]["ProductGuid"].ToString();
                            tm.TestType = pdfdt.Rows[i]["TestType"].ToString();
                            tm.TypeName = pdfdt.Rows[i]["TypeName"].ToString();
                            pdfinfolist.Add(tm);
                        }
                    }
                    //pdfinfolist = Comm.ToDataList<pdfinfo>(pdfdt);
                }
                if (!string.IsNullOrWhiteSpace(userid) && !string.IsNullOrWhiteSpace(pid))
                {
                    Physics_BrowseModel model = new Physics_BrowseModel();
                    model.BrowsCount = 1;
                    model.Btype = 1;//物性详情浏览
                    model.ProductGuid = pid;
                    model.UserId = userid;
                    string msg = "";
                    mbll.AddPhysics_Browse(model, ref msg);
                }

                //新增点击次数
                //bll.ProductHit(prodid);
                var returndata = new
                {
                    prodata = listinfo,
                    pdfdata = pdfinfolist,
                    iscollections = iscollection
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
        public ActionResult GetProductPDF(string prodid,string userid)
        {
            try
            {
                string pdfUrl = "pdf/" + prodid + ".pdf";
                var ds = bll.GetModelInfo(prodid,string.Empty,string.Empty);
                string pmodel = string.Empty;
                string placeorigin = string.Empty;
                string brand = string.Empty;
                string icopath = string.Empty;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    pmodel = ds.Tables[0].Rows[0]["proModel"].ToString();
                    placeorigin = ds.Tables[0].Rows[0]["PlaceOrigin"].ToString();
                    brand = ds.Tables[0].Rows[0]["Brand"].ToString();
                    icopath = ds.Tables[0].Rows[0]["IcoPath"].ToString();
                }
                bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewDetail?prodid=" + prodid + "&userid=" + userid, pdfUrl, Server.UrlEncode(pmodel), Server.UrlEncode(placeorigin), Server.UrlEncode(brand),"0", icopath);
                if (success)
                {
                    mbll.AddOperationPay("查看下载物性", userid);
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



        /// <summary>
        /// 获取产品的pdf文档(带产品信息返回)
        /// </summary>
        /// <param name="prodid">产品id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetProductPDF2(string prodid, string userid)
        {
            try
            {
                string pdfUrl = "pdf/" + prodid + ".pdf";
                var ds = bll.GetModelInfo(prodid,string.Empty,string.Empty);
                string pmodel = string.Empty;
                string placeorigin = string.Empty;
                string brand = string.Empty;
                string icopath = string.Empty;
                string factory = string.Empty;
                string name = string.Empty;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    pmodel = ds.Tables[0].Rows[0]["proModel"].ToString();
                    placeorigin = ds.Tables[0].Rows[0]["PlaceOrigin"].ToString();
                    brand = ds.Tables[0].Rows[0]["Brand"].ToString();
                    icopath = ds.Tables[0].Rows[0]["IcoPath"].ToString();
                }

                DataTable dt = bll.GetProductFactoryInfo(prodid);
                if (dt.Rows.Count > 0)
                {
                    factory = dt.Rows[0]["AliasName"].ToString() == "" ? dt.Rows[0]["EnglishName"].ToString() : dt.Rows[0]["AliasName"].ToString();
                    name = dt.Rows[0]["Name"].ToString();
                }

                bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewDetail?prodid=" + prodid, pdfUrl, Server.UrlEncode(pmodel), Server.UrlEncode(placeorigin), Server.UrlEncode(brand), "0", icopath);
                if (success)
                {
                    mbll.AddOperationPay("查看下载物性", userid);
                    string path = MainHost + "/" + pdfUrl;
                    var returndata = new
                    {
                        retpath = path,
                        model = pmodel,
                        retfactory = factory,
                        retname = name

                    };
                    return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
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


        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetProductULPDF(string prodid,string numberid="")
        {
            try
            {
                string pdfUrl = "pdf/" + prodid + ".pdf";
                var ds = bll.GetModelInfo(prodid,string.Empty,string.Empty);
                string pmodel = string.Empty;
                string placeorigin = string.Empty;
                string brand = string.Empty;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    pmodel = ds.Tables[0].Rows[0]["proModel"].ToString();
                    placeorigin = ds.Tables[0].Rows[0]["PlaceOrigin"].ToString();
                    brand = ds.Tables[0].Rows[0]["Brand"].ToString();
                }
                //bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewDetail?prodid=" + prodid, pdfUrl);
                bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewUl_ShowPdf?prodid=" + prodid+ "&numberid="+ numberid, pdfUrl, Server.UrlEncode(pmodel), Server.UrlEncode(placeorigin), Server.UrlEncode(brand), "1",string.Empty);
                if (success)
                {
                    string path = MainHost + "/" + pdfUrl;
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
        /// <summary>
        /// 生成物性对比pdf
        /// </summary>
        /// <param name="contsval">id串</param>
        /// <param name="title1">标题1</param>
        /// <param name="title2">标题2</param>
        /// <param name="title3">标题3</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetContrastPdf(string contsval, string title1, string title2, string title3,string factory1,string factory2,string factory3,string class1,string class2,string class3)
        {
            try
            {
                string temptitle1 = Server.UrlDecode(title1);
                string temptitle2 = Server.UrlDecode(title2);
                string temptitle3 = Server.UrlDecode(title3);
                //string titletemp = contsval.Replace(';', '|');
                //string newtimestr = temptitle1 + temptitle2 + temptitle3;// DateTime.Now.ToString("yyyyMMddhhmmss");
                string newtimestr= DateTime.Now.ToString("yyyyMMddhhmmss");
                string pdfUrl = "pdf/"+ newtimestr+".pdf";
                //bool success = PlasQueryWeb.CommonClass.PdfHelper.HtmlToPdf(MainHost + "/PhysicalProducts/ViewDetail?prodid=" + prodid, pdfUrl);
                string url = MainHost + "/PhysicalProducts/ContrastPDF?contsval=" + contsval + "&title1=" + Server.UrlEncode(title1) + "&title2=" + Server.UrlEncode(title2) + "&title3=" + Server.UrlEncode(title3);
                bool success = PlasQueryWeb.CommonClass.PdfHelper.ContrastHtmlToPdf(url, pdfUrl, Server.UrlEncode(title1), Server.UrlEncode(title2), Server.UrlEncode(title3), Server.UrlEncode(factory1), Server.UrlEncode(factory2), Server.UrlEncode(factory3),
                    Server.UrlEncode(class1), Server.UrlEncode(class2), Server.UrlEncode(class3));
                if (success)
                {
                    string path = MainHost + "/" + pdfUrl;
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
        public ActionResult AppProductReplaceList(int pageindex, int pagesize, int isfilter, string factory, string proid, string ver,string wherestring,string isuser,string type="0")
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
                List<bigtype> returnfactorylist = new List<bigtype>();
                var ds = new DataSet();
                if (!string.IsNullOrWhiteSpace(proid) && !string.IsNullOrWhiteSpace(ver))
                {
                    string temppid = "";
                    if (type == "1")
                    {
                        string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", proid);
                        DataTable pdt = SqlHelper.GetSqlDataTable(sql);
                        if (pdt.Rows.Count > 0)
                        {
                            temppid = pdt.Rows[0]["ProductGuid"].ToString();
                        }
                    }
                    else
                    {
                        temppid = proid;
                    }
                    if (!string.IsNullOrWhiteSpace(wherestring))
                    {
                        ds = plbll.GetReplace(temppid, ver, "", wherestring, pageindex, pagesize, "0", "0", "");

                        if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    ReplaceResult tmodel = new ReplaceResult();
                                    tmodel.AlikePercent = dt.Rows[i]["ALikePercent"].ToString();
                                    tmodel.TargetGuid = dt.Rows[i]["ProductId"].ToString();
                                    tmodel.Name = dt.Rows[i]["name"].ToString();
                                    tmodel.PlaceOrigin = dt.Rows[i]["PlaceOrigin"].ToString();
                                    tmodel.ProModel = dt.Rows[i]["ProModel"].ToString();
                                    returnlist.Add(tmodel);
                                }
                            }
                            if (ds.Tables.Count == 3)
                            {
                                DataTable factorydt = ds.Tables[2];
                                for (int i = 0; i < factorydt.Rows.Count; i++)
                                {
                                    bigtype tempmodel = new bigtype();
                                    tempmodel.Name = factorydt.Rows[i]["PlaceOrigin"].ToString();
                                    returnfactorylist.Add(tempmodel);
                                }
                            }
                        }
                    }
                    else
                    {
                        ds = plbll.GetProductReplace(ver, temppid, pageindex, pagesize, isfilter, factory);
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
                            if (ds.Tables.Count == 3)
                            {
                                DataTable factorydt = ds.Tables[2];
                                for (int i = 0; i < factorydt.Rows.Count; i++)
                                {
                                    bigtype tempmodel = new bigtype();
                                    tempmodel.Name = factorydt.Rows[i]["PlaceOrigin"].ToString();
                                    returnfactorylist.Add(tempmodel);
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
                    var returndata = new {
                        replacedata = returnlist,
                        factoydata= returnfactorylist
                    };
                    return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
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
        public ActionResult AppMsgSearch(string ver, string use, string key, string factory, string kind, string method, string characteristic, string additive, string addingmaterial,string addghdq, int pageindex, int pagesize, string isTow)
        {

            string jsonstr = string.Empty;
            List<ClassInfo> classjsonstr = new List<ClassInfo>();//类别json数据
            List<FactoryInfo> factoryjsonstr = new List<FactoryInfo>();//厂家json数据
            List<attributeinfo> otherlist = new List<attributeinfo>();//其他属性
            List<bigtype> bigtypelist = new List<bigtype>();//大类
            List<SearchResult> resultmodellist = new List<SearchResult>();//搜索结果
            int resultcount = 0;//搜索结果总条数
            string tow = "0";
            //二次查询
            if (!string.IsNullOrWhiteSpace(characteristic))
            {
                tow = "1";
            }
            if (!string.IsNullOrWhiteSpace(use))
            {
                tow = "1";
            }
            if (!string.IsNullOrWhiteSpace(kind))
            {
                tow = "1";
            }
            if (!string.IsNullOrWhiteSpace(method))
            {
                tow = "1";
            }
            if (!string.IsNullOrWhiteSpace(factory))
            {
                tow = "1";
            }
            if (!string.IsNullOrWhiteSpace(additive))
            {
                tow = "1";
            }
            if (!string.IsNullOrWhiteSpace(addingmaterial))
            {
                tow = "1";
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                DataSet ds = new DataSet();
                if (tow == "1")//第二次分页，查询数据isTow:(0：第一次 1：多次)
                {
                    ds = bll.GetTwoSearch(pageindex, pagesize, ver, characteristic, use, kind, method, factory, additive, addingmaterial, addghdq);
                }
                else
                {
                    ds = bll.GetGeneralSearch(key.Trim(), pageindex, pagesize, ver,1);
                }
                if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
                {
                    //jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                    resultmodellist = Comm.ToDataList<SearchResult>(ds.Tables[0]);
                    resultcount = Convert.ToInt32(ds.Tables[1].Rows[0]["totalcount"]);
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
                otherdata = otherlist,
                resultcounta= resultcount
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
                List<parminfo> classjsonstr = new List<parminfo>();//类别json数据
                List<parminfo> factoryjsonstr = new List<parminfo>();//厂家json数据
                List<attributeinfo> list = new List<attributeinfo>();
                if (txname == "生产厂家")
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            parminfo tfmodel = new parminfo();
                            tfmodel.Name = dt.Rows[j]["attributevalue"].ToString();
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
                            parminfo tcmodel = new parminfo();
                            tcmodel.Name = dt.Rows[s]["attributevalue"].ToString();
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
        public ActionResult GetSuperSearchParam(string type, string keyname)
        {
            try
            {
                DataTable dt = bll.Sys_GetSuperSearchParamForApp(type, keyname);
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
        [HttpPost]
        public ActionResult AppSuperMsgSearch(int pageindex, int pagesize, string guidstr, string searchstr)
        {
            try
            {
                string isNavLink = string.Empty;
                List<SearchResult> resultmodellist = new List<SearchResult>();//搜索结果
                string tempsearchkey = Server.UrlDecode(searchstr);
                var ds = bll.Sys_SuperSearch(tempsearchkey, 2052, pageindex, pagesize, guidstr, isNavLink);
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

        #region 根据价格趋势id获取对应的物性id
        /// <summary>
        /// 根据价格趋势id获取对应的物性id
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetProductidByPrid(string prid)
        {
            try
            {
                string sql = string.Format(@"SELECT a.Model AS pricemodel,b.ProModel,a.ProductGuid FROM dbo.Pri_Product a 
                                            INNER JOIN dbo.Product b ON b.ProductGuid = a.ProductGuid WHERE a.PriceProductGuid = '{0}'", prid);
                DataTable pdt = SqlHelper.GetSqlDataTable(sql);
                string tempid = "";
                string tempmodel = "";
                if (pdt.Rows.Count > 0)
                {
                    tempid = pdt.Rows[0]["ProductGuid"].ToString();
                    tempmodel= pdt.Rows[0]["ProModel"].ToString();
                }
                var returndata = new
                {
                    pid = tempid,
                    pmodel= tempmodel
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
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
        public ActionResult AddMaterialCollection(Physics_CollectionModel model,string type="0")
        {
            try
            {
                if (type == "1")
                {
                    string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", model.ProductGuid);
                    DataTable pdt = SqlHelper.GetSqlDataTable(sql);
                    if (pdt.Rows.Count > 0)
                    {
                        model.ProductGuid = pdt.Rows[0]["ProductGuid"].ToString();
                    }
                }
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
                return Json(Common.ToJsonResult("Fail", "收藏失败", ex.Message), JsonRequestBehavior.AllowGet);
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

        #region 添加物料行情订阅
        /// <summary>
        /// 添加物料收藏
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult AddMaterialQuotation(Physics_QuotationModel model)
        {
            try
            {
                //string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", model.ProductGuid);
                //DataTable pdt = SqlHelper.GetSqlDataTable(sql);
                //if (pdt.Rows.Count > 0)
                //{
                //    model.ProductGuid = pdt.Rows[0]["ProductGuid"].ToString();
                //}
                string savesql = string.Format("select id from Physics_Quotation where ProductGuid='{0}' and UserId='{1}'", model.ProductGuid, model.UserId);
                var dt = SqlHelper.GetSqlDataTable(savesql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return Json(Common.ToJsonResult("IsCollection", "此产品已经订阅行情"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = "";
                    bool returnresult = mbll.AddPhysics_Quotation(model, ref msg);
                    if (returnresult)
                    {
                        return Json(Common.ToJsonResult("Success", "订阅成功"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(Common.ToJsonResult("Fail", "订阅失败"), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "订阅失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 添加物料对比
        /// <summary>
        /// 添加物料对比
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult AddMaterialContrast(Physics_ContrastModel model, string type)
        {
            try
            {
                if (type=="1")
                {
                    string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", model.ProductGuid);
                    DataTable pdt = SqlHelper.GetSqlDataTable(sql);
                    if (pdt.Rows.Count > 0)
                    {
                        model.ProductGuid = pdt.Rows[0]["ProductGuid"].ToString();
                    }
                }                
                string savesql = string.Format("select id from Physics_Contrast where ProductGuid='{0}' and UserId='{1}'", model.ProductGuid, model.UserId);
                var dt = SqlHelper.GetSqlDataTable(savesql.ToString());
                if (dt.Rows.Count > 0)
                {
                    return Json(Common.ToJsonResult("IsCollection", "此产品已经添加对比"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = "";
                    bool returnresult = mbll.AddPhysics_Contrast(model, ref msg);
                    if (returnresult)
                    {
                        return Json(Common.ToJsonResult("Success", "加入对比成功"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(Common.ToJsonResult("Fail", "加入对比失败"), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "加入对比失败", ex.Message), JsonRequestBehavior.AllowGet);
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
        public ActionResult AppGetMaterialReplaceDetail(string ProductID, string Ven,string isuser,string type)
        {
            try
            {
                string temppid = "";
                if (type == "1")
                {
                    string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", ProductID);
                    DataTable pdt = SqlHelper.GetSqlDataTable(sql);
                    if (pdt.Rows.Count > 0)
                    {
                        temppid = pdt.Rows[0]["ProductGuid"].ToString();
                    }
                }
                else
                {
                    temppid = ProductID;
                }
                string jsonstr = "";
                var dt = new DataTable();
                if (!string.IsNullOrWhiteSpace(temppid) && !string.IsNullOrWhiteSpace(Ven))
                {
                    dt = plbll.GetReplaceDetail(temppid, Ven, isuser);
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
        public ActionResult GetReplaceWeightList(string Rpt,string type="0")
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
                    if (type == "1")
                    {
                        string sql = string.Format("SELECT * FROM dbo.Pri_Product WHERE PriceProductGuid='{0}'", Rpt);
                        DataTable pdt = SqlHelper.GetSqlDataTable(sql);
                        if (pdt.Rows.Count > 0)
                        {
                            Rpt = pdt.Rows[0]["ProductGuid"].ToString();
                        }
                    }
                    pt = plbll.NewGetAttributeAliasList_RealKey(Rpt);
                    if (pt.Rows.Count > 0)
                    {
                        DataTable tblDatas = new DataTable("Datas");
                        var dr = pt;
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
                            if (
                                (string.IsNullOrEmpty(dr.Rows[i]["Attribute2"].ToString())&& string.IsNullOrEmpty(dr.Rows[i]["RealKey"].ToString())
                                && string.IsNullOrEmpty(dr.Rows[i]["Attribute3"].ToString())
                                && string.IsNullOrEmpty(dr.Rows[i]["Attribute4"].ToString())
                                && string.IsNullOrEmpty(dr.Rows[i]["Attribute5"].ToString()) && dr.Rows[i]["Attribute1"].ToString().Trim() != "总体")
                                ||
                                (dr.Rows[i]["Attribute1"].ToString().Trim() == "加工方法"|| dr.Rows[i]["Attribute1"].ToString().Trim() == "产品说明"
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
                                        //DataRow[] rows = pt.Select("Attribute1='" + bigName + "' and Attribute2Alias = '" + samllName + "'");
                                        //if (rows.Count() > 0)
                                        //{
                                        //    RealKey = rows[0]["RealKey"].ToString();
                                        //}

                                        model.Attribute1 = dr.Rows[i]["Attribute1"].ToString().Trim();
                                        model.Attribute2 = dr.Rows[i]["Attribute2"].ToString().Trim();
                                        model.Attribute3 = dr.Rows[i]["Attribute3"].ToString().Trim();
                                        model.Attribute4 = dr.Rows[i]["Attribute4"].ToString().Trim();
                                        model.Attribute5 = dr.Rows[i]["Attribute5"].ToString().Trim();
                                        model.lev = dr.Rows[i]["lev"].ToString().Trim();
                                        model.RealKey = dr.Rows[i]["realkey"].ToString().Trim();
                                        model.bigName = bigName;
                                        listinfo.Add(model);
                                    }
                                }
                            }
                        }
                    }
                    //var ds = bll.GetModelInfo(Rpt);
                    //pt = plbll.GetAttributeAliasList_RealKey();//替换属性RealKey
                    //if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                    //{
                    //    ProTitle = ds.Tables[0].Rows[0]["proModel"].ToString();
                    //    ProGuid = ds.Tables[0].Rows[0]["productid"].ToString();
                    //}
                    //if (ds.Tables.Contains("ds1") && ds.Tables[1].Rows.Count > 0)
                    //{
                    //    //< !--卿思明:
                    //    //产品说明；注射; 注射说明; 备注 这些都不参与对比
                    //    //，说明，加工方法，备注不允许选择-- >
                    //    //< !--总体参与对比的有（（RoHS 合规性；供货地区；加工方法；树脂ID(ISO 1043)；特性；添加剂；填料 / 增强材料；用途 ）这个是总体里要参与对比的）-->
                    //    var dr = ds.Tables[1];///此数据要过滤
                    //    DataTable tblDatas = new DataTable("Datas");

                    //    DataColumn dc = null;
                    //    dc = tblDatas.Columns.Add("lev", Type.GetType("System.Int32"));
                    //    dc = tblDatas.Columns.Add("Attribute1", Type.GetType("System.String"));
                    //    dc = tblDatas.Columns.Add("Attribute2", Type.GetType("System.String"));
                    //    dc = tblDatas.Columns.Add("Attribute3", Type.GetType("System.String"));
                    //    dc = tblDatas.Columns.Add("Attribute4", Type.GetType("System.String"));
                    //    dc = tblDatas.Columns.Add("Attribute5", Type.GetType("System.String"));
                    //    dc = tblDatas.Columns.Add("RealKey", Type.GetType("System.String"));
                    //    string lev = string.Empty;
                    //    DataRow newRow;
                    //    for (var i = 0; i < dr.Rows.Count; i++)
                    //    {
                    //        tempinfo model = new tempinfo();
                    //        if ((string.IsNullOrEmpty(dr.Rows[i]["Attribute2"].ToString())
                    //            && string.IsNullOrEmpty(dr.Rows[i]["Attribute3"].ToString())
                    //            && string.IsNullOrEmpty(dr.Rows[i]["Attribute4"].ToString())
                    //            && string.IsNullOrEmpty(dr.Rows[i]["Attribute5"].ToString()) && dr.Rows[i]["Attribute1"].ToString().Trim() != "总体")
                    //            ||
                    //            (dr.Rows[i]["Attribute1"].ToString().Trim() == "加工方法"
                    //            || dr.Rows[i]["Attribute1"].ToString().Trim() == "材料状态"
                    //            || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "资料 1".Replace(" ", "")
                    //            || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "搜索 UL 黄卡".Replace(" ", "")
                    //            || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "UL 黄卡 2".Replace(" ", "")
                    //            || dr.Rows[i]["Attribute1"].ToString().Trim().Replace(" ", "") == "UL文件号".Replace(" ", "")
                    //            )
                    //            )
                    //        {
                    //        }
                    //        else
                    //        {

                    //            //单独过滤注射
                    //            if (dr.Rows[i]["Attribute1"].ToString().Trim() == "注射")
                    //            {
                    //                //int.TryParse(dr.Rows[i]["lev"].ToString().Trim(), out lev);//记住注射
                    //                lev = "injection";
                    //            }
                    //            else
                    //            {

                    //                int count = (1 + Convert.ToInt32(dr.Rows[i]["lev"].ToString().Trim()));
                    //                if (count == 3 && lev == "injection")
                    //                {

                    //                }
                    //                else
                    //                {
                    //                    if (count == 2)//后续其他，必须清除，不然会有异常
                    //                    {
                    //                        lev = "";
                    //                    }
                    //                    newRow = tblDatas.NewRow();
                    //                    if (dr.Rows[i]["lev"].ToString() == "1")
                    //                    {
                    //                        bigName = dr.Rows[i]["Attribute1"].ToString().Trim();
                    //                    }
                    //                    else
                    //                    {
                    //                        samllName = dr.Rows[i]["Attribute1"].ToString().Trim();
                    //                    }
                    //                    DataRow[] rows = pt.Select("Attribute1='" + bigName + "' and Attribute2Alias = '" + samllName + "'");
                    //                    if (rows.Count() > 0)
                    //                    {
                    //                        RealKey = rows[0]["RealKey"].ToString();
                    //                    }
                    //                    //newRow["lev"] = dr.Rows[i]["lev"].ToString().Trim();
                    //                    //newRow["Attribute1"] = dr.Rows[i]["Attribute1"].ToString().Trim();
                    //                    //newRow["Attribute2"] = dr.Rows[i]["Attribute2"].ToString().Trim();
                    //                    //newRow["Attribute3"] = dr.Rows[i]["Attribute3"].ToString().Trim();
                    //                    //newRow["Attribute4"] = dr.Rows[i]["Attribute4"].ToString().Trim();
                    //                    //newRow["Attribute5"] = dr.Rows[i]["Attribute5"].ToString().Trim();
                    //                    //newRow["RealKey"] = RealKey;
                    //                    //tblDatas.Rows.Add(newRow);

                    //                    model.Attribute1 = dr.Rows[i]["Attribute1"].ToString().Trim();
                    //                    model.Attribute2 = dr.Rows[i]["Attribute2"].ToString().Trim();
                    //                    model.Attribute3 = dr.Rows[i]["Attribute3"].ToString().Trim();
                    //                    model.Attribute4 = dr.Rows[i]["Attribute4"].ToString().Trim();
                    //                    model.Attribute5 = dr.Rows[i]["Attribute5"].ToString().Trim();
                    //                    model.lev = dr.Rows[i]["lev"].ToString().Trim();
                    //                    model.RealKey = RealKey;
                    //                    model.bigName = bigName;
                    //                    listinfo.Add(model);
                    //                }
                    //            }
                    //        }
                    //    }
                    //    dt = tblDatas;
                    //    //var spdr=dr.Select("Attribute1<>'产品说明' and Attribute1 <> '注射' and Attribute1 <> '备注'")
                    //}
                    ////if (ds.Tables.Count > 2)
                    ////{
                    ////    //详情页标题：种类（Prd_SmallClass_l.Name）+型号（Product.ProModel）+产地（Product.PlaceOrigin）
                    ////    ViewBag.Title = ds.Tables[2].Rows[0]["Title"].ToString();
                    ////    //关键字：特性(product_l.characteristic)+用途(product_l.ProUse)
                    ////    ViewBag.Keywords = ds.Tables[2].Rows[0]["keyword"].ToString();
                    ////    //ViewBag.description2 =产品说明(只能用 exec readproduct '0004D924-5BD4-444F-A6D2-045D4EDB0DD3'命令中读出)
                    ////}
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
                List<parminfo> listresult = new List<parminfo>();
                DataTable dt = bll.GetClass(parentid, middlename, type);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        parminfo m = new parminfo();
                        m.Name = dt.Rows[i]["Name"].ToString();
                        m.Guid = dt.Rows[i]["parentguid"].ToString();
                        listresult.Add(m);
                    }
                }
                return Json(Common.ToJsonResult("Success", "获取成功", listresult), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取我的订阅
        /// <summary>
        /// 获取我的订阅
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetPhysicsQuotation(string userId, int? pageindex=1, int? pagesize=20)
        {
            List<Physics_QuotationModel> returnlist = new List<Physics_QuotationModel>();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                DataTable dt = mbll.AppGetPhyices_Quotation(userId, pageindex.Value, pagesize.Value);
                returnlist = Comm.ToDataList<Physics_QuotationModel>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
     
        #region 获取我的对比
        /// <summary>
        /// 获取我的对比
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetPhysicsContrast(string userId, int pageindex, int pagesize)
        {
            List<Physics_ContrastModel> returnlist = new List<Physics_ContrastModel>();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                DataTable dt = mbll.AppGetPhyices_Contrast(userId, pageindex, pagesize);
                returnlist = Comm.ToDataList<Physics_ContrastModel>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取我的浏览记录
        /// <summary>
        /// 获取我的浏览记录
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetPhysics_Browse(string userId, int pageindex, int pagesize)
        {
            List<Physics_BrowseModel> returnlist = new List<Physics_BrowseModel>();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string msg = "";
                int pagecount = 0;
                returnlist = mbll.GetPhysics_Browse(userId, pageindex, pagesize,ref pagecount, ref msg,"");
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取我的查看物性数量
        //获取我的查看物性数量
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetMyLookMaterial(string userid)
        {
            try
            {
                List<Physics_BrowseModel> returnlist = new List<Physics_BrowseModel>();
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    DataTable dt = mbll.GetMyLookMaterial(userid);
                    returnlist = Comm.ToDataList<Physics_BrowseModel>(dt);
                    return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
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

        #region 获取我的任务完成数量
        //获取我的任务完成数量
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetMyTaskNumber(string userid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    DataTable dt = mbll.GetMyLookMaterial(userid);//查看物性完成数量
                    int downpdfnumber = mbll.GetTaskNumber(userid, "查看下载物性");
                    var resultnumber = new
                    {
                        lookmaterialcount = dt.Rows.Count,
                        downpdfcount= downpdfnumber
                    };
                    return Json(Common.ToJsonResult("Success", "获取成功", resultnumber), JsonRequestBehavior.AllowGet);
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


        #region 删除我的浏览记录
        /// <summary>
        /// 删除我的浏览记录
        /// </summary>
        /// <param name="idstr"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult DeleteBrowse(string idstr)
        {
            try
            {
                string[] templist = idstr.Split(',');
                List<string> idlist = new List<string>();
                for (int i = 0; i < templist.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(templist[i]))
                    {
                        idlist.Add(templist[i]);
                    }
                }
                string note = string.Empty;
                bool count = mbll.RomvePhysics_Browse(idlist, ref note);
                if (count)
                {
                    return Json(Common.ToJsonResult("Success", "删除成功"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "删除失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "删除失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 删除我的对比
        /// <summary>
        /// 删除我的对比
        /// </summary>
        /// <param name="idstr"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult DeleteContrast(string idstr)
        {
            try
            {
                string[] templist = idstr.Split(',');
                List<string> idlist = new List<string>();
                for (int i = 0; i < templist.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(templist[i]))
                    {
                        idlist.Add(templist[i]);
                    }
                }
                string note = string.Empty;
                bool count = mbll.RomvePhysics_Contrast(idlist, ref note);
                if (count)
                {
                    return Json(Common.ToJsonResult("Success", "删除成功"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "删除失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "删除失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 删除我的订阅
        /// <summary>
        /// 删除我的订阅
        /// </summary>
        /// <param name="idstr"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult DeleteQuotation(string idstr)
        {
            try
            {
                string[] templist = idstr.Split(',');
                List<string> idlist = new List<string>();
                for (int i = 0; i < templist.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(templist[i]))
                    {
                        idlist.Add(templist[i]);
                    }
                }
                string note = string.Empty;
                bool count = mbll.RomvePhysics_Quotation(idlist, ref note);
                if (count)
                {
                    return Json(Common.ToJsonResult("Success", "删除成功"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "删除失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "删除失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 删除我的物料收藏
        /// <summary>
        /// 删除我的物料收藏
        /// </summary>
        /// <param name="idstr"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult DeleteCollection(string idstr)
        {
            try
            {
                string[] templist = idstr.Split(',');
                List<string> idlist = new List<string>();
                for (int i = 0; i < templist.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(templist[i]))
                    {
                        idlist.Add(templist[i]);
                    }
                }
                string note = string.Empty;
                bool count = mbll.RomvePhysics_Collection(idlist, ref note);
                if (count)
                {
                    return Json(Common.ToJsonResult("Success", "删除成功"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "删除失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "删除失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 根据厂家名称获取对应的厂家id
        /// <summary>
        /// 根据厂家名称获取对应的厂家id
        /// </summary>
        /// <param name="name">厂家名称</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetFactoryByName(string name)
        {
            try
            {
                List<parminfo> listresult = new List<parminfo>();
                string[] listname = name.Split(',');
                if (listname.Length > 0)
                {
                    for (int i = 0; i < listname.Length; i++)
                    {
                        DataTable dt = bll.AppGetFactoryByName(listname[i]);
                        if (dt.Rows.Count > 0)
                        {
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                parminfo m = new parminfo();
                                m.Name = dt.Rows[j]["AliasName"].ToString();
                                m.Guid = dt.Rows[j]["Guid"].ToString();
                                listresult.Add(m);
                            }
                        }
                    }
                }
                return Json(Common.ToJsonResult("Success", "获取成功", listresult), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取案例
        /// <summary>
        /// 获取案例
        /// </summary>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetNews(int? pageindex=1, int? pagesize=20)
        {
            try
            {
                List<News> returnlist = new List<News>();
                DataTable dt = newbll.GetNews(pageindex.Value, pagesize.Value,3);
                returnlist = Comm.ToDataList<News>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
     
        #region 获取案例详情
        /// <summary>
        /// 获取案例详情
        /// </summary>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetNewsDetail(int ID)
        {
            try
            {
                DataTable dt = newbll.GetNewsDetail(ID);
                string tempcontent = "";
                if (dt.Rows.Count > 0)
                {
                    tempcontent = dt.Rows[0]["ContentAll"].ToString();
                }
                var returndata = new
                {
                    content = tempcontent// HttpUtility.UrlEncode(tempcontent)
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
            
        }
        #endregion

        #region 获取UL数据
        /// <summary>
        /// 获取UL数据
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult ShowUlBigPDF(string ProductGuid)
        {
            try
            {
                List<Ul_HeadModel> blist = new List<Ul_HeadModel>();
                if (!string.IsNullOrWhiteSpace(ProductGuid))
                {
                    blist = bll.GetUl_Head(ProductGuid);
                    // clist = bll.GetUl_body(ProductGuid);
                }
                return Json(Common.ToJsonResult("Success", "获取成功", blist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取UL数据详情
        /// <summary>
        /// 获取UL数据详情
        /// </summary>
        /// <param name="ProductGuid"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult ShowUlPDF(string Numberid, string ProductGuid)
        //public ActionResult ShowUlPDF(string ProductGuid)
        {
            try
            {
                var blist = new Ul_HeadModel();
                var clist = new List<Ul_bodyModel>();
                if (!string.IsNullOrWhiteSpace(ProductGuid))
                {
                    var query = bll.GetUl_HeadNumber(ProductGuid);
                    if (query != null && query.Count > 0)
                    {
                        blist = query[0];
                    }
                    clist = bll.GetUl_body(Numberid);

                }
                var returndata = new
                {
                    blistdata = blist,
                    clistdata = clist
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取单位
        /// <summary>
        /// 获取单位
        /// </summary>
        /// <param name="parmname"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetUnit(string parmname)
        {
            try
            {
                List<bigtype> unitlist = new List<bigtype>();
                string getsql = string.Format(@"select distinct unit as Name from ProductAttribute where RealKey in (select * from f_split('{0}',';'))", parmname);
                DataTable dt = SqlHelper.GetSqlDataTable(getsql);
                unitlist = Comm.ToDataList<bigtype>(dt);
                bigtype one = new bigtype();
                one.Name = "全部";
                //unitlist.Add(one);
                unitlist.Insert(0,one);
                return Json(Common.ToJsonResult("Success", "获取成功", unitlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
     
        #region 获取搜索关键词转换数字后的关键词字符串
        /// <summary>
        /// 获取搜索关键词转换数字后的关键词字符串
        /// </summary>
        /// <param name="key">原本要转换的关键词</param>
        /// <returns>转换后的新关键词</returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetSearchNewKye(string key)
        {
            try
            {
                string keys = Common.DecodeMoneyCn(key);
                var newkey = new
                {
                    key = keys
                };
                return Json(Common.ToJsonResult("Success", "获取成功", newkey), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
      
        #region  获取产品助剂信息
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetAnnotationList(int pageindex, int pagesize, string key, int? type = 0)
        {
            try
            {
                List<Annotation> returnlist = new List<Annotation>();
                DataTable dt = bll.GetAnnotationList(pagesize, pageindex,key, type);
                returnlist = Comm.ToDataList<Annotation>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region  获取产品助剂详情
        //获取助剂详情
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetAnnotationDetail(int id)
        {
            try
            {
                List<KeyValue> list = new List<KeyValue>();
                List<AnnotationDetail> detaillist = new List<AnnotationDetail>();
                DataSet set = bll.GetAnnotationDetail(id);
                DataTable dt = set.Tables[0];
                DataTable dt2 = set.Tables[1];
                DataRow[] rw = dt.Select("facekey='说明'");
                if (rw.Length>0)
                {
                    dt.Rows.Remove(rw[0]);
                }                
                list = Comm.ToDataList<KeyValue>(dt);
                detaillist = Comm.ToDataList<AnnotationDetail>(dt2);
                for (int i = 0; i < detaillist.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        detaillist[i].bcolor = "ff";
                    }
                    else
                    {
                        detaillist[i].bcolor = "f2";
                    }
                }
                var returnlist = new
                {
                    headdata = list,
                    detaildata = detaillist
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取改新厂列表
        /// <summary>
        /// 获取改新厂列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetModProductList(int pageindex, int pagesize)
        {
            try
            {
                List<ModProduct> list = new List<ModProduct>();
                DataTable dt = mpbll.GetModProductList(pageindex, pagesize);
                list = Comm.ToDataList<ModProduct>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取改新厂产品详情
        /// <summary>
        /// 获取改新厂产品详情
        /// </summary>
        /// <param name="id">产品id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetModProductDetail(int id)
        {
            try
            {
                List<ModProductDetail> list = new List<ModProductDetail>();
                DataSet set = mpbll.GetModProductDetail(id);
                DataTable dt = set.Tables[0];
                list = Comm.ToDataList<ModProductDetail>(dt);
                DataTable dt1 = set.Tables[1];
                string tempmodel = string.Empty;
                string tempfactory = string.Empty;
                if (dt1.Rows.Count > 0)
                {
                    tempmodel = dt1.Rows[0]["ProModel"].ToString();
                    tempfactory = dt1.Rows[0]["PlaceOrigin"].ToString();
                }
                var returndata = new
                {
                    datalist = list,
                    thmodel = tempmodel,
                    thfactory = tempfactory
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
     
        #region 查询改新厂替换详情
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetNewFactoryth(int id)
        {
            try
            {
                List<NewFactoryth> list = new List<NewFactoryth>();
                DataTable dt = mpbll.GetNewFactoryth(id);
                list = Comm.ToDataList<NewFactoryth>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 绑定邀请人
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="usercode">用户邀请码</param>
        /// <returns>绑定结果</returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult BindUserByUserCode(string userid, string usercode)
        {
            try
            {
                DataTable dt = mbll.GetUserByUserCode(usercode);//上级用户信息
                DataTable usdt = mbll.GetUserInfo(userid);//被邀请人的用户信息
                DateTime usercreatetime = Convert.ToDateTime(usdt.Rows[0]["CreateDate"]);
                TimeSpan sp = DateTime.Now.Subtract(usercreatetime);
                //验证用户是否过了绑定时间
                if (sp.Days > 10)
                {
                    return Json(Common.ToJsonResult("OutTime", "已经超过绑定时间"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int usercount = dt.Rows.Count;
                    if (usercount < 0)
                    {
                        return Json(Common.ToJsonResult("NotFind", "不存在"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string parentuserid = dt.Rows[0]["ID"].ToString();
                        string thisuserleadid = dt.Rows[0]["LeaderUserName"].ToString();
                        //检测是否相互绑定(如果邀请人的上级用户id和当前被邀请人的用户id相同就是相互绑定)
                        if (thisuserleadid == userid)
                        {
                            return Json(Common.ToJsonResult("MutualBind", "不能相互绑定"), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            string bindresult = mbll.UpdateUserInfobll("LeaderUserName", parentuserid, userid);
                            if (bindresult == "Success")
                            {
                                mbll.AddOperationPay("注册", userid);
                                return Json(Common.ToJsonResult("Success", "绑定成功"), JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(Common.ToJsonResult("Fail", "绑定失败"), JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "绑定失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取我的下级用户
        /// <summary>
        /// 获取我的下级用户
        /// </summary>
        /// <param name="userid">我的用户id</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页获取数量</param>
        /// <returns>用户信息datatable集合</returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetMyUser(string userid, int pageindex, int pagesize)
        {
            try
            {
                List<MyUserInfo> returnlist = new List<MyUserInfo>();
                DataTable dt = mbll.GetMyUserInfo(userid, pageindex, pagesize);
                returnlist = Comm.ToDataList<MyUserInfo>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 设置邀请码
        /// <summary>
        /// 设置邀请码
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="usid">用户id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult SetUserCode(string phone, string usid,string type)
        {
            try
            {
                int tempcode = Convert.ToInt32(phone.Substring(7, 4));
                string usercode = mbll.getusebycode(phone,tempcode);
                string bindresult = mbll.binduserphone(phone, usid);
                //string bindresult = mbll.UpdateUserInfobll("phone", phone, usid);
                if (bindresult == "Success")
                {
                    var returndata = new
                    {
                        usercodes = usercode
                    };
                    return Json(Common.ToJsonResult("Success", "设置成功", returndata), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "设置失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "设置失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 设置用户积分
        /// <summary>
        /// 设置用户积分
        /// </summary>
        /// <param name="type">使用场景</param>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult SetUserIntegral(string type, string userid)
        {
            try
            {
                mbll.AddOperationPay(type, userid);
                return Json(Common.ToJsonResult("Success", "获取成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取用户积分收入明细
        /// <summary>
        /// 获取用户积分收入明细
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetUserIntegralDetail(string userid, int pageindex, int pagesize)
        {
            try
            {
                List<UserIntegralDetail> returnlist = new List<UserIntegralDetail>();
                DataTable dt = mbll.GetUserIntegralDetail(userid, pageindex, pagesize);
                returnlist = Comm.ToDataList<UserIntegralDetail>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 首页推荐pdf
        //首页推荐pdf
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetListPDF()
        {
            try
            {
                DataTable dt = bll.GetListPDF(30);
                List<pdflist> returnlist = new List<pdflist>();
                returnlist = Comm.ToDataList<pdflist>(dt);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 添加关键词搜索记录
        //首页推荐pdf
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult SaveSearchKeyLog(HeadSearchLog model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.userid))
                {
                    model.userid = "";
                }
                int returnid = mbll.AddHeadSearchLog(model);
                var returndata = new
                {
                    id = returnid
                };
                return Json(Common.ToJsonResult("Success", "添加搜索记录成功", returndata), JsonRequestBehavior.AllowGet);
                //if (returnbool)
                //{
                //    return Json(Common.ToJsonResult("Success", "添加搜索记录成功"), JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(Common.ToJsonResult("Fail", "添加搜索记录失败"), JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception)
            {
                return Json(Common.ToJsonResult("Fail", "添加搜索记录失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region 修改关键词搜索记录为提示
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult UpdateSearchKeyLogToReply(int id)
        {
            try
            {
                bool upresult = mbll.UpdateSearchKeyLogToReply(id);
                if (upresult)
                {
                    return Json(Common.ToJsonResult("Success", "操作成功"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "操作失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "操作失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 获取我的问题反馈记录
        /// <summary>
        /// 获取我的问题反馈记录
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetMyProblem(string userId, int pageindex, int pagesize)
        {
            List<Problem> returnlist = new List<Problem>();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string msg = "";
                int pagecount = 0;
                returnlist = mbll.GetMyProblem(userId, pageindex, pagesize, ref pagecount, ref msg);
                return Json(Common.ToJsonResult("Success", "获取成功", returnlist), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetPdfInfo(string prodid)
        {
            try
            {
                var ds = bll.GetModelInfo(prodid,string.Empty,string.Empty);
                string promodel = "";
                string proplaceorigin = "";
                string proband = "";
                List<webpdfinfo> pdflist = new List<webpdfinfo>();

                DataTable pdfdt = new DataTable();
                List<pdfinfo> pdfinfolist = new List<pdfinfo>();
                if (!string.IsNullOrEmpty(prodid))
                {
                    pdfdt = bll.GetProductPdf(prodid);
                    if (pdfdt.Rows.Count > 0)
                    {
                        for (int i = 0; i < pdfdt.Rows.Count; i++)
                        {
                            pdfinfo tm = new pdfinfo();
                            tm.BefromName = pdfdt.Rows[i]["BefromName"].ToString();
                            tm.Guid = pdfdt.Rows[i]["Guid"].ToString();
                            tm.ID = pdfdt.Rows[i]["ID"].ToString();
                            tm.ImagesColor = pdfdt.Rows[i]["ImagesColor"].ToString();
                            tm.PdfPath = PdfUrl + pdfdt.Rows[i]["PdfPath"].ToString();
                            tm.ProductGuid = pdfdt.Rows[i]["ProductGuid"].ToString();
                            tm.TestType = pdfdt.Rows[i]["TestType"].ToString();
                            tm.TypeName = pdfdt.Rows[i]["TypeName"].ToString();
                            pdfinfolist.Add(tm);
                        }
                    }
                    //pdfinfolist = Comm.ToDataList<pdfinfo>(pdfdt);
                }

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        promodel = ds.Tables[0].Rows[0]["proModel"].ToString();
                        proplaceorigin = ds.Tables[0].Rows[0]["PlaceOrigin"].ToString();
                        proband = ds.Tables[0].Rows[0]["Brand"].ToString();
                    }
                    if (ds.Tables.Count > 1)
                    {
                        //物性
                        //ViewBag.PhysicalInfo = ds.Tables[1];
                        pdflist = Comm.ToDataList<webpdfinfo>(ds.Tables[1]);
                    }
                    var data = new
                    {
                        model = promodel,
                        placeorigin = proplaceorigin,
                        brand = proband,
                        list = pdflist,
                        headlist= pdfinfolist
                    };
                    return Json(Common.ToJsonResult("Success", "获取成功", data), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }

        //获取新闻类别
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetNewClass()
        {
            try
            {
                DataTable dt = newbll.GetNewClass();
                List<NewsClass> list = new List<NewsClass>();
                if (dt.Rows.Count > 0)
                {
                    list = Comm.ToDataList<NewsClass>(dt);
                }
                return Json(Common.ToJsonResult("Success", "获取成功", list), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
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
            public string icopath { get; set; }
        }

        //价格对比类
        public class PriceContrast
        {
            //价格json数据
            public string pricejsondata { get; set; }
            //最大价格
            public int MaxPrice { get; set; }
            //最小价格
            public int MinPrice { get; set; }
        }
        //案例
        public class News
        {
            //标题
            public string Title { get; set; }
            //描述
            public string DescMsg { get; set; }
            //ID
            public int ID { get; set; }
            //图片
            public string HomeImg { get; set; }
            public string CreateDate { get; set; }
        }
        
        //产品助剂详情
        public class AnnotationDetail {
            public string Property { get; set; }
            public string ValueUnit { get; set; }
            public string TestType { get; set; }
            public string TestCondition { get; set; }
            public string bcolor { get; set; }
        }
        public class KeyValue {
            public string facekey { get; set; }
            public string facevalue { get; set; }
        }

        //改新厂
        public class ModProduct
        {
            public int Id { get; set; }
            public string ProModel { get; set; }
            public string ProType { get; set; }
            public string Color { get; set; }
            public string Comments { get; set; }
            public string PictPath { get; set; }
            public string WheelPath { get; set; }
        }

        //改新厂产品详情
        //p.ProType,p.Color,pr.MaxQty,pm.Native,pm.Brand,pm.IsImported,pm.Catgory,pr.Packing,pr.RateType
        public class ModProductDetail
        {
            public string ProductGuid { get; set; }//产品guid
            public int Id { get; set; }
            public string ProModel { get; set; }//型号
            public string PictPath { get; set; }//主图
            public string WheelPath { get; set; }//轮播图
            public decimal Price { get; set; }//价格
            public string DiscountPrice { get; set; }//折扣价
            public string Title { get; set; }//产品标题
            public int MinQty { get; set; }//起售量
            public string DetailsMemo { get; set; }//详情
            public string Unit { get; set; }//单位
            public string Name { get; set; }//厂家名称
            public string MainPhoto { get; set; }//厂家log
            public string MainBusiness { get; set; }//厂家主营类目
            public string PayType { get; set; }//付款方式
            public string Color { get; set; }//颜色
            public string MaxQty { get; set; }//库存
            public string Native { get; set; }//产地
            public string Brand { get; set; }//品牌
            public string IsImported { get; set; }//是否进口
            public string Catgory { get; set; }//类型
            public string Packing { get; set; }//包装
            public string RateType { get; set; }//含税种类
            public string Ccategry { get; set; }//企业性质
            public string RegisteredCapital { get; set; }//注册资本
            public string Address { get; set; }//公司地址
            public string WebSite { get; set; }//官网
            public string RegisterTime { get; set; }//成立时间 
            public string Service { get; set; }//服务
            public string Salesperson { get; set; }//联系人
            public string SalseMoble { get; set; }//联系电话
            public string SalseQQ { get; set; }//联系qq
            public string SalseWechat { get; set; }//联系微信
            public string SalseMail { get; set; }//联系邮箱
        }
        //改新厂替换详情
        public class NewFactoryth
        {
            public string ProductGuid { get; set; }//产品id
            public string ProModel { get; set; }//型号
            public string PlaceOrigin { get; set; }//生产厂商
        }

        //用户信息
        public class MyUserInfo {
            //用户名
            public string UserName { get; set; }
            //头像
            public string HeadImage { get; set; }
            //注册时间
            public string CreateDate { get; set; }
        }

        //用户积分明细
        public class UserIntegralDetail
        {
            public string Operation { get; set; }
            public int InPay { get; set; }
            public string CreateTime { get; set; }
        }
        //分享页面的pdf
        public class webpdfinfo
        {
            public int lev { get; set; }
            public string Attribute1 { get; set; }
            public string Attribute2 { get; set; }
            public string Attribute3 { get; set; }
            public string Attribute4 { get; set; }
            public string Attribute5 { get; set; }
        }
    }
}
