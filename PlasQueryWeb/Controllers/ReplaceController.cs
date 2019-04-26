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
        // GET: 替换

        public ActionResult Index()
        {
            //产品编号
            string Rpt = string.Empty;
            string ProTitle = "<a href='/PhysicalProducts/Index' style='color:#FF9933'>请选择产品</a>";
            if (!string.IsNullOrEmpty(Request["Rpt"]))
            {
                Rpt = Request["Rpt"].ToString();
            }
            var dt = new DataTable();
            if (!string.IsNullOrEmpty(Rpt))
            {
                var ds = bll.GetModelInfo(Rpt);
                if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                {
                    ProTitle = ds.Tables[0].Rows[0]["proModel"].ToString();
                }
                if (ds.Tables.Contains("ds1") && ds.Tables[1].Rows.Count > 0)
                {
                    dt = ds.Tables[1];///此数据要过滤
                   
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
            return View();
        }


        /// <summary>
        /// 返回列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ReplaceList()
        {
            return View();
        }

    }
}