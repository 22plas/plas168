using System.Web;
using System.Web.Optimization;

namespace PlasModel
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/bgcss").Include(
            "~/Content/css/bootstrap.css",
            "~/Content/css/site.css"));
            //@Styles.Render("~/GetContent/CommonCss")
            //@Scripts.Render("~/GetContent/CommonJS")
            bundles.Add(new ScriptBundle("~/GetContent/CommonJS").Include(
                "~/Content/js/Common.js",
                "~/Content/js/json2.js"
                ));
            bundles.Add(new StyleBundle("~/GetContent/CommonCss").Include(
                      "~/Content/css/base.css",
                      "~/Content/css/pageSwitch.min.css",
                      "~/Content/css/iconfont/iconfont.css"
                      ));

            bundles.Add(new ScriptBundle("~/GetButtom/FootJS").Include(
                "~/Content/js/pageSwitch.min.js",
                "~/Content/js/index.js",
                "~/Content/js/loadingoverlay.js"
                ));
            //超级搜索
            bundles.Add(new ScriptBundle("~/PhysicalJS/BesJS").Include(
                "~/Content/js/business/PhysicalJS.js"
                ));
            //普通搜索
            bundles.Add(new ScriptBundle("~/ProductJS/BesPdtJS").Include(
              "~/Content/js/business/ProductJS.js"
              ));

            //价格趋势
            bundles.Add(new ScriptBundle("~/PriceJS/PrcJS").Include(
             "~/Content/js/echarts.min.js",
             "~/Content/js/business/priceJS.js"
             ));
            //个人中心用到的js
            bundles.Add(new ScriptBundle("~/RegisterJS/BesRJS").Include(
                "~/Content/js/jquery-2.1.1.js",
              "~/Scripts/comm.js",
              "~/Content/js/jsEx.js",
              "~/Content/js/jQueryEx.js",
              "~/Content/js/amazeui.min.js",
              "~/Content/js/iscroll.js",
              "~/Content/js/app.js",
              "~/Content/js/vue.js",
              "~/Content/js/jquery-ui.js",
              "~/Content/js/uploadfile.js",
              "~/Content/js/Area.js",
              "~/Content/js/business/Register.js"
              ));
                 BundleTable.EnableOptimizations = true;
        }
    }
}
