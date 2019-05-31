using PlasModel;
using PlasModel.App_Start;
using System;
using System.Web.Caching;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlasCommon;

namespace PlasQueryWeb.Controllers
{
    public class HomeController : Controller
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
        //首页
        public ActionResult Index2()
        {
            if (AccountData != null)
            {
                ViewBag.username = AccountData.UserName;
            }
            else
            {
                ViewBag.username = "";
            }
            return View();
        }

        public ActionResult Index()
        {
            var ds = bll.HotProducts(5);
            if (ds != null && ds.Rows.Count > 0)
            {
                var list = PlasCommon.ToolClass<PlasModel.ProductViewModel>.ConvertDataTableToModel(ds);
                ViewBag.HotList = list;
            }
            return View();
        }

        public ActionResult Home()
        {
            var ds = bll.HotProducts(5);
            if (ds != null && ds.Rows.Count > 0)
            {
                var list = PlasCommon.ToolClass<PlasModel.ProductViewModel>.ConvertDataTableToModel(ds);
                ViewBag.HotList = list;
            }
            return View();
        }

        /// <summary>
        /// 生成JS
        /// </summary>
        /// <returns></returns>
        public JsonResult SetJs()
        {
            string keyword = string.Empty;
            string strJson = string.Empty;//Json数据
            if (!string.IsNullOrEmpty(Request["keyword"]))
            {
                keyword = Request["keyword"].ToString();
                var list = FindSearchsWord();

                strJson = Newtonsoft.Json.JsonConvert.SerializeObject(list.Where(p => p.Word.Contains(keyword)).ToList());
            }
            //string path = Server.MapPath("../Scripts/SearchKeywordsJs.js");

            //// Delete the file if it exists. 
            //if (System.IO.File.Exists(path))
            //{
            //    System.IO.File.Delete(path);
            //}
            //var dt = new PlasCommon.Common().Getsys_Autokey();
            //// Create the file. 
            //StreamWriter sr = System.IO.File.CreateText(path);
            //if (dt.Rows.Count > 0)
            //{
            //    sr.WriteLine("'data':[");
            //    for (var i = 0; i < dt.Rows.Count; i++)
            //    {
            //        sr.WriteLine("{'id':'"+ i + "','word':'"+dt.Rows[i]["Word"].ToString().Trim()+"','description':''}");
            //    }
            //    sr.WriteLine("]");
            //}
            ////// 这里是f1的内容 
            ////// …… 

            //sr.Close();
            return Json(new { data = strJson }, JsonRequestBehavior.AllowGet);
        }


        public IEnumerable<wordModel> FindSearchsWord()
        {
            var cache = CacheHelper.GetCache("commonData_Search");//先读取
            if (cache == null)//如果没有该缓存
            {
                var dt = new PlasCommon.Common().Getsys_Autokey();
                var enumerable = ToolClass<wordModel>.ConvertDataTableToModel(dt);
                CacheHelper.SetCache("commonData_Search", enumerable);//添加缓存
                return enumerable;
            }
            var result = (List<wordModel>)cache;//有就直接返回该缓存
            return result;

        }




    }
}