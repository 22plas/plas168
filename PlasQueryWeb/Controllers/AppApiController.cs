﻿using PlasCommon;
using PlasCommon.SqlCommonQuery;
using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class AppApiController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        private PlasBll.ReplaceBll plbll = new PlasBll.ReplaceBll();


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
                DataSet ds = bll.GetPagePriceTypeOrFactory(key, typestr, pageindex, pagesize);
                string jsonstr = string.Empty;
                //获取类别\厂家
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                }
                return Json(Common.ToJsonResult("Success", "获取成功", jsonstr), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 获取价格走势信息
        /// </summary>
        /// <param name="priceproductguid">id</param>
        /// <param name="bdate">起始时间</param>
        /// <param name="ndate">结束时间</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult AppGetPriceDateList(string priceproductguid, string bdate, string ndate)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
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
                #region 产品详情
                var ds = bll.NewGetModelInfo(prodid);//"9C37DC9C-E867-46A2-97DF-32A9489BCDC4"
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
                //新增点击次数
                //bll.ProductHit(prodid);
                #endregion
                return Json(Common.ToJsonResult("Success", "获取成功", listinfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", "获取失败", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

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
        public ActionResult AppProductReplaceList(int pageindex, int pagesize, int isfilter, string factory, string proid, string ver)
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
                var ds = new DataSet();
                if (!string.IsNullOrWhiteSpace(proid) && !string.IsNullOrWhiteSpace(ver))
                {
                    ds = plbll.GetProductReplace(ver, proid, pageindex, pagesize, isfilter, factory);
                    if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                    {
                        jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                    }
                    //if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    //{
                    //    int.TryParse(ds.Tables[1].Rows[0]["counts"].ToString(), out count);
                    //}
                    //if (ds.Tables.Contains("ds2") && ds.Tables[2].Rows.Count > 0)
                    //{
                    //    companys = ToolHelper.DataTableToJson(ds.Tables[2]);
                    //}
                    return Json(Common.ToJsonResult("Success", "获取成功", jsonstr), JsonRequestBehavior.AllowGet);
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

            if (!string.IsNullOrWhiteSpace(key))
            {
                DataSet ds = new DataSet();
                if (isTow == "1")//第二次分页，查询数据isTow:(0：第一次 1：多次)
                {
                    ds = bll.GetTwoSearch(pageindex, pagesize, ver, characteristic, use, kind, method, factory, additive, addingmaterial);
                }
                else
                {
                    ds = bll.GetGeneralSearch(key, pageindex, pagesize, ver);
                }
                if (ds.Tables.Contains("ds") && ds.Tables[0] != null)
                {
                    jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                }
                DataTable dt = new DataTable();
                if (ds.Tables.Contains("ds2") && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    DataView dv = ds.Tables[2].DefaultView;
                    dt = dv.ToTable(true, "attribute");
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string tempstr = dt.Rows[i]["attribute"].ToString();
                            if (tempstr == "生产厂家")
                            {
                                DataTable tempfactory = ds.Tables[2].Select("attribute='生产厂家'").CopyToDataTable();
                                if (tempfactory.Rows.Count > 0)
                                {
                                    for (int j = 0; j < tempfactory.Rows.Count; j++)
                                    {
                                        FactoryInfo tfmodel = new FactoryInfo();
                                        tfmodel.ManuFacturer = tempfactory.Rows[j]["attributevalue"].ToString();
                                        factoryjsonstr.Add(tfmodel);
                                    }
                                    //factoryjsonstr = ToolHelper.DataTableToJson(tempfactory);
                                }
                            }
                            else if (tempstr == "产品种类")
                            {
                                DataTable tempclass = ds.Tables[2].Select("attribute='产品种类'").CopyToDataTable();
                                if (tempclass.Rows.Count > 0)
                                {
                                    for (int s = 0; s < tempclass.Rows.Count; s++)
                                    {
                                        ClassInfo tcmodel = new ClassInfo();
                                        tcmodel.SmallClass = tempclass.Rows[s]["attributevalue"].ToString();
                                        classjsonstr.Add(tcmodel);
                                    }
                                    //classjsonstr = ToolHelper.DataTableToJson(tempclass);
                                }
                            }
                            else
                            {
                                bigtype bigmodel = new bigtype();
                                bigmodel.Name = tempstr;
                                bigtypelist.Add(bigmodel);
                                DataRow[] dr = ds.Tables[2].Select("attribute='" + tempstr + "'");
                                if (dr.Length > 0)
                                {
                                    for (int r = 0; r < dr.Length; r++)
                                    {
                                        attributeinfo tamodel = new attributeinfo();
                                        tamodel.attribute= dr[r]["attribute"].ToString();
                                        tamodel.attributevalue = dr[r]["attributevalue"].ToString();
                                        otherlist.Add(tamodel);
                                    }
                                }
                                //DataTable tempdt = dr.CopyToDataTable();
                                //if (tempdt.Rows.Count > 0)
                                //{
                                //    if (otherjsonstr == "")
                                //    {
                                //        otherjsonstr = ToolHelper.DataTableToJson(tempdt);
                                //    }
                                //    else
                                //    {
                                //        otherjsonstr = otherjsonstr + "," + ToolHelper.DataTableToJson(tempdt);
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
            var returndata = new
            {
                datalist = jsonstr,
                //bigtype = BigType,
                //smalltype = samlltypelist
                factorydata = factoryjsonstr,
                classdata = classjsonstr,
                bigtypedata= bigtypelist,
                otherdata = otherlist
            };
            return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
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
        }
    }
}
