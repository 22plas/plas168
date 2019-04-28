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

        private PlasBll.ReplaceBll plbll = new PlasBll.ReplaceBll();
        // GET: 替换

        public ActionResult Index()
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
            if (!string.IsNullOrEmpty(Rpt))
            {
                var ds = bll.GetModelInfo(Rpt);
                if (ds.Tables.Contains("ds") && ds.Tables[0].Rows.Count > 0)
                {
                    ProTitle = ds.Tables[0].Rows[0]["proModel"].ToString();
                    ProGuid = ds.Tables[0].Rows[0]["productid"].ToString();

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
            ViewBag.ProGuid = ProGuid;
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


        public JsonResult GetReplaceList()
        {
            // GetReplace(string SourceId, string ver, string UserId, string WhereString)
            //计算一个产品的相似度
            string SourceId = "088CF6D2-C231-499E-AD9F-82688EB6F9D5";// "2D1636B6-9548-4790-8F14-66DCD5879F2A";// "D161293C-4A8C-4267-B38A-D19A19B89682";// "2350C07B-D47C-46FD-88FF-00A903EF4594"; //TextProductId.Text.Trim();//

            //本次执行运算的唯一版本号
            string ver = Guid.NewGuid().ToString();
            //用户ID，应该从Session中取得
            string UserId = "张三或李四";
            //@WhereString: 要求参与相似度计算的所有属性及权重列表，这个是你前端用JS拼装出来的。
            //下面共有4个属性参与运算，每个属性用{}分开，每个{}中的最后一个是数值，代表这个属性的权重
            string WhereString = "{物理性能=)密度=>10}{机械性能=)伸长率=>10}{物理性能=)熔流率=>15}{可燃性=)阻燃等级=>15}";
            //采用多少个任务来处理本次相似度运算，我们的SQL服务器有32个逻辑内核，这里采用30个任务来处理，
            //当需要处理的目标物料较少时，由SQL存储过程会自动任务个数来保持执行效率
            var dt= plbll.GetReplace(SourceId, ver, UserId, WhereString);


            return null;

        }

    }
}