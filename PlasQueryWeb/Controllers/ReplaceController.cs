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
    public class ReplaceController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        //获取当前登录的用户信息
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }



        private PlasBll.ReplaceBll plbll = new PlasBll.ReplaceBll();
        // GET: 替换

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ReplaceGetList()
        {
            PlasBll.ProductBll bll = new PlasBll.ProductBll();
            //产品编号
            string Rpt = string.Empty;
            string ProTitle = "请选择产品";
            string ProGuid = string.Empty;
            if (!string.IsNullOrEmpty(Request["Rpt"]))
            {
                Rpt = Request["Rpt"].ToString();
            }
            var dt = new DataTable();
            var pt = new DataTable();//替换属性RealKey
            string bigName = string.Empty;
            string samllName = string.Empty;
            string RealKey = string.Empty;
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
                                    newRow["lev"] = dr.Rows[i]["lev"].ToString().Trim();
                                    newRow["Attribute1"] = dr.Rows[i]["Attribute1"].ToString().Trim();
                                    newRow["Attribute2"] = dr.Rows[i]["Attribute2"].ToString().Trim();
                                    newRow["Attribute3"] = dr.Rows[i]["Attribute3"].ToString().Trim();
                                    newRow["Attribute4"] = dr.Rows[i]["Attribute4"].ToString().Trim();
                                    newRow["Attribute5"] = dr.Rows[i]["Attribute5"].ToString().Trim();
                                    newRow["RealKey"] = RealKey;
                                    tblDatas.Rows.Add(newRow);
                                }

                            }

                        }

                    }
                    dt = tblDatas;
                    //var spdr=dr.Select("Attribute1<>'产品说明' and Attribute1 <> '注射' and Attribute1 <> '备注'")


                }
                if (ds.Tables.Count > 2)
                {
                    //详情页标题：种类（Prd_SmallClass_l.Name）+型号（Product.ProModel）+产地（Product.PlaceOrigin）
                    ViewBag.Title = ds.Tables[2].Rows[0]["Title"].ToString();
                    //关键字：特性(product_l.characteristic)+用途(product_l.ProUse)
                    ViewBag.Keywords = ds.Tables[2].Rows[0]["keyword"].ToString();
                    //ViewBag.description2 =产品说明(只能用 exec readproduct '0004D924-5BD4-444F-A6D2-045D4EDB0DD3'命令中读出)
                }
            }
            ViewBag.PhysicalInfo = dt;
            ViewBag.ProModel = ProTitle;
            ViewBag.ProGuid = ProGuid;
            return View();
        }



        /// <summary>
        /// 返回列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ReplaceList()
        {
            string WhereString = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["name"]))
            {
                WhereString = Request["name"].ToString();
            }
            string ProductGuid = string.Empty;//产品编号
            if (!string.IsNullOrEmpty(Request["ProductGuid"]))
            {
                ProductGuid = Request["ProductGuid"].ToString();
            }
            string ProModel = "，请先选择产品";//牌号
            if (!string.IsNullOrEmpty(Request["ProModel"]))
            {
                ProModel = Request["ProModel"].ToString();
            }
            var company = new DataTable();
            //生产厂家
            company = bll.GetSearchParam(4, 15);
            ViewBag.ProGuid = ProductGuid;
            ViewBag.WhereString = WhereString;
            ViewBag.company = company;
            ViewBag.ProModel = ProModel;
            return View();
        }


        public JsonResult GetReplaceList(int pageindex, int pagesize, string guidstr, string proid)
        {
            string sErr = string.Empty;
            string jsonstr = string.Empty;
            string isLink = string.Empty;//分页
            string isfilter = "0";//是否过滤
            int count = 0;
            try
            {
                string WhereString = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request["WhereString"]))
                {
                    WhereString = Request["WhereString"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(Request["isLink"]))
                {
                    isLink = Request["isLink"].ToString();
                }
                if (!string.IsNullOrEmpty(Request["isfilter"]))
                {
                    isfilter = Request["isfilter"].ToString();
                }
                // GetReplace(string SourceId, string ver, string UserId, string WhereString)
                //计算一个产品的相似度
                string SourceId = proid; //"088CF6D2-C231-499E-AD9F-82688EB6F9D5";// "2D1636B6-9548-4790-8F14-66DCD5879F2A";// "D161293C-4A8C-4267-B38A-D19A19B89682";// "2350C07B-D47C-46FD-88FF-00A903EF4594"; //TextProductId.Text.Trim();//

                //本次执行运算的唯一版本号
                string ver = guidstr;//Guid.NewGuid().ToString();
                string Companys = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request["Companys"]))
                {
                    Companys = Request["Companys"].ToString();
                }
                //用户ID，应该从Session中取得
                string UserId = string.Empty;
                if (AccountData != null)
                {
                    UserId = AccountData.UserName;//"张三或李四";
                }
                UserId = "黄远林";
                //@WhereString: 要求参与相似度计算的所有属性及权重列表，这个是你前端用JS拼装出来的。
                //下面共有4个属性参与运算，每个属性用{}分开，每个{}中的最后一个是数值，代表这个属性的权重
                //string WhereString = "{物理性能=)密度=>10}{机械性能=)伸长率=>10}{物理性能=)熔流率=>15}{可燃性=)阻燃等级=>15}";
                //采用多少个任务来处理本次相似度运算，我们的SQL服务器有32个逻辑内核，这里采用30个任务来处理，
                //当需要处理的目标物料较少时，由SQL存储过程会自动任务个数来保持执行效率
                var ds = new DataSet();
                //if (!string.IsNullOrWhiteSpace(UserId))
                //{
                if (!string.IsNullOrWhiteSpace(WhereString) && !string.IsNullOrWhiteSpace(SourceId))
                {
                    ds = plbll.GetReplace(SourceId, ver, UserId, WhereString, pageindex, pagesize, isLink, isfilter, Companys);
                    if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                    {
                        jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                    }
                    if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                        int.TryParse(ds.Tables[1].Rows[0]["ts"].ToString(), out count);
                    }
                }
                else
                {
                    sErr = "违法操作，筛选数据异常！";
                }
                //}
                //else {
                //    sErr = "请先登陆！";
                //}
            }
            catch (Exception ex)
            {
                sErr = ex.Message.ToString();
            }


            return Json(new { data = jsonstr, count = count, msg = sErr }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 通过产品
        ///GUID查询产品
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductReplace()
        {
            string ProductGuid = string.Empty;
            string ProductName = string.Empty;
            if (!string.IsNullOrEmpty(Request["PGuid"]))
            {
                ProductGuid = Request["PGuid"].ToString();
                var dt = bll.GetProductMessage(ProductGuid);
                if (dt.Rows.Count > 0)
                {
                    ProductName = dt.Rows[0]["ProModel"].ToString();
                    dt.Dispose();
                }
                
            }
            
            var company = new DataTable();
            //生产厂家
            company = bll.GetSearchParam(4, 15);
            ViewBag.ProGuid = ProductGuid;
            ViewBag.company = company;
            ViewBag.ProductName = ProductName;
            return View();
        }

        //获取产品列表数据信息
        public JsonResult ProductReplaceList(int pageindex, int pagesize, string guidstr, string proid)
        {
            string sErr = string.Empty;
            string jsonstr = string.Empty;
            int isfilter=0;//是否过滤
            int count = 0;
            try
            {
      
                if (!string.IsNullOrEmpty(Request["isfilter"]))
                {
                       int.TryParse(Request["isfilter"].ToString(),out isfilter);
                }
                //本次执行运算的唯一版本号
                string ver = guidstr;//Guid.NewGuid().ToString();
                                     //用户ID，应该从Session中取得
                string UserId = string.Empty;
                if (AccountData != null)
                {
                    UserId = AccountData.UserName;//"张三或李四";
                }
                string Companys = string.Empty;
                if (!string.IsNullOrWhiteSpace(Request["Companys"]))
                {
                    Companys = Request["Companys"].ToString();
                }

                var ds = new DataSet();
                //if (!string.IsNullOrWhiteSpace(UserId))
                //{
                if (!string.IsNullOrWhiteSpace(proid) && !string.IsNullOrWhiteSpace(guidstr))
                {
                    ds = plbll.GetProductReplace(ver, proid, pageindex, pagesize, isfilter, Companys);
                    if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                    {
                        jsonstr = ToolHelper.DataTableToJson(ds.Tables[0]);
                    }
                    if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    {
                        int.TryParse(ds.Tables[1].Rows[0]["counts"].ToString(), out count);
                    }
                }
                else
                {
                    sErr = "违法操作，筛选数据异常！";
                }
                //}
                //else {
                //    sErr = "请先登陆！";
                //}
            }
            catch (Exception ex)
            {
                sErr = ex.Message.ToString();
            }


            return Json(new { data = jsonstr, count = count, msg = sErr }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 显示详情
        /// </summary>
        /// <returns></returns>
        public ActionResult More(string ProductID,string Ven)
        {
            var dt = new DataTable();
            if (!string.IsNullOrWhiteSpace(ProductID) && !string.IsNullOrWhiteSpace(Ven))
            {
                dt = plbll.GetReplaceDetail(ProductID, Ven);
            }
            ViewBag.allData = dt;
            return View();
        }

    }
}