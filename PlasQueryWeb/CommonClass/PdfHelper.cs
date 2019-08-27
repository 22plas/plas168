using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PlasQueryWeb.CommonClass
{
    public class PdfHelper
    {
        //type 标识是否是黄卡UL生成pdf 0：否  1：是
        public static bool HtmlToPdf(string url, string path,string pmodel, string placeorigin, string brand,string type,string icopath)
        {
            Encoding utf8 = Encoding.UTF8;
            string MainHost = System.Web.Configuration.WebConfigurationManager.AppSettings["MainHost"];
            path = HttpContext.Current.Server.MapPath("~/") + path;
            string cookie = "cookieKey cookieValue";//改为为你自己的
            string headerUrl = MainHost + "/pdfHeader.html?pmodel=" + Microsoft.JScript.GlobalObject.escape(pmodel) + "&placeorigin=" + Microsoft.JScript.GlobalObject.escape(placeorigin) + "&brand=" + Microsoft.JScript.GlobalObject.escape(brand)+ "&icopath="+ Microsoft.JScript.GlobalObject.escape(icopath);//页头内容
            string footerUrl = MainHost + "/pdfFooter.html";//页脚内容页面
            string footerLeft = "本页面信息资料来自厂商，文档提供者不承担任何法律责任，强烈建议在最终选择材料前，就数据值与材料供应商进行验证。版权归原作者所有，如有侵权请立即与我们联系。";
            //string Arguments = "-q  -B 0 -L 0 -R 0 -T 0 -s A4 --no-background --disable-smart-shrinking --cookie " + cookie + " " + url + " " + path; //参数可以根据自己的需要进行修改
            string Arguments = "-q  -B 10 -L 0 -R 0 -T 5 --header-html " + headerUrl + " --footer-html " + footerUrl + " --header-spacing 35 --footer-spacing 10 --margin-bottom 45 --margin-top 50 --disable-smart-shrinking " + url + " " + path; //参数可以根据自己的需要进行修改
            if (type=="1")
            {
                string urlgooterurl = MainHost+"/urlpdfFooter.html";
                Arguments= "-q  -B 10 -L 0 -R 0 -T 5 --footer-html " + urlgooterurl + " --footer-spacing 10 --margin-bottom 45 --disable-smart-shrinking " + url + " " + path;
            }
            try
            {
                PlasCommon.Common.AddLog("", "开始生成pdf", url+ "+" + path, "");
                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(path))
                    return false;
                var p = new Process();
                string str = HttpContext.Current.Server.MapPath("~/") + @"bin/wkhtmltopdf.exe";
                if (!File.Exists(str))
                    return false;
                p.StartInfo.FileName = str;
                p.StartInfo.Arguments = Arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = false;
                var istrue=p.Start();
                p.WaitForExit();
                //System.Threading.Thread.Sleep(1000);
                PlasCommon.Common.AddLog("", "开始生成pdf", istrue.ToString() + "exe生成器" + "+" + str, "");
                return true;
            }
            catch (Exception ex)
            {
                PlasCommon.Common.AddLog("", "生成pdf错误",ex.Message,"");
                throw ex;
            }
            //return false;
        }

        public static bool ContrastHtmlToPdf(string url, string path,string title1,string title2,string title3,string factory1, string factory2, string factory3, string class1, string class2, string class3)
        {
            Encoding utf8 = Encoding.UTF8;
            string MainHost = System.Web.Configuration.WebConfigurationManager.AppSettings["MainHost"];
            path = HttpContext.Current.Server.MapPath("~/") + path;
            string cookie = "cookieKey cookieValue";//改为为你自己的
            string headerUrl = MainHost + "/pdfContrastHeader.html?titleo=" + Microsoft.JScript.GlobalObject.escape(title1) + "&titlet=" + Microsoft.JScript.GlobalObject.escape(title2) + "&titleth=" + Microsoft.JScript.GlobalObject.escape(title3)+
                "&factory1=" + Microsoft.JScript.GlobalObject.escape(factory1) + "&factory2=" + Microsoft.JScript.GlobalObject.escape(factory2) + "&factory3=" + Microsoft.JScript.GlobalObject.escape(factory3) +
                "&class1=" + Microsoft.JScript.GlobalObject.escape(class1) + "&class2=" + Microsoft.JScript.GlobalObject.escape(class2) + "&class3=" + Microsoft.JScript.GlobalObject.escape(class3);
            string footerUrl = MainHost + "/pdfFooter.html";//页脚内容页面
            //string Arguments = "-q  -B 0 -L 0 -R 0 -T 0 -s A4 --no-background --disable-smart-shrinking --cookie " + cookie + " " + url + " " + path; //参数可以根据自己的需要进行修改
            string Arguments = "-q  -B 10 -L 0 -R 0 -T 5 --header-html " + headerUrl + " --footer-html " + footerUrl + " --header-spacing 5 --footer-spacing 10 --margin-bottom 45 --margin-top 50 --disable-smart-shrinking " + url + " " + path; //参数可以根据自己的需要进行修改
            try
            {
                PlasCommon.Common.AddLog("", "开始生成pdf", url + "+" + path, "");
                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(path))
                    return false;
                var p = new Process();
                string str = HttpContext.Current.Server.MapPath("~/") + @"bin/wkhtmltopdf.exe";
                if (!File.Exists(str))
                    return false;
                p.StartInfo.FileName = str;
                p.StartInfo.Arguments = Arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = false;
                var istrue = p.Start();
                p.WaitForExit();
                //System.Threading.Thread.Sleep(1000);
                PlasCommon.Common.AddLog("", "开始生成pdf", istrue.ToString() + "exe生成器" + "+" + str, "");
                return true;
            }
            catch (Exception ex)
            {
                PlasCommon.Common.AddLog("", "生成pdf错误", ex.Message, "");
                throw ex;
            }
        }

    }
}