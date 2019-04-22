using System.Web;
using System.Web.Optimization;

namespace PlasQueryWeb
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            //@Styles.Render("~/GetContent/CommonCss")
            //@Scripts.Render("~/GetContent/CommonJS")
            bundles.Add(new ScriptBundle("~/GetContent/CommonJS").Include(
                "~/Content/js/Common.js",
                "~/Content/js/json2.js"
                ));
            bundles.Add(new StyleBundle("~/GetContent/CommonCss").Include(
                      "~/Content/css/base.css",
                      "~/Content/css/pageSwitch.min.css"
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


            BundleTable.EnableOptimizations = true;
        }
    }
}
