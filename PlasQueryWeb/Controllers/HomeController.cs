using PlasModel;
using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult SetJs()
        {
            string path = Server.MapPath("../Scripts/SearchKeywordsJs.js");

            // Delete the file if it exists. 
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            var dt = new PlasCommon.Common().Getsys_Autokey();
            // Create the file. 
            StreamWriter sr = System.IO.File.CreateText(path);
            if (dt.Rows.Count > 0)
            {
                sr.WriteLine("'data':[");
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    sr.WriteLine("{'id':'"+ i + "','word':'"+dt.Rows[i]["Word"].ToString().Trim()+"','description':''}");
                }
                sr.WriteLine("]");
            }
            //// 这里是f1的内容 
            //// …… 

            sr.Close();
            return View();
        }


    }
}