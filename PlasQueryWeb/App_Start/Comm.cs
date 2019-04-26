using PlasQueryWeb.App_Start.Qiniu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using static PlasCommon.Enums;

namespace PlasQueryWeb.App_Start
{
    public static class Comm
    {

        #region ResizeImage图片地址生成
        /// <summary>
        /// ResizeImage图片地址生成
        /// </summary>
        /// <param name="url">图片地址</param>
        /// <param name="w">最大宽度</param>
        /// <param name="h">最大高度</param>
        /// <param name="quality">质量0~100</param>
        /// <param name="image">占位图类别</param>
        /// <returns>地址为空返回null</returns>
        public static string ResizeImage(string url, int? w = null, int? h = null,
            int? quality = null,
            DummyImage? image = DummyImage.Default,
            ResizerMode? mode = null,
            ReszieScale? scale = null
            )
        {
            var Url = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);

            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else
            {
                if (Url.IsLocalUrl(url))
                {
                    var t = new Uri(HttpContext.Current.Request.Url, Url.Content(url)).AbsoluteUri;
                    Dictionary<string, string> p = new Dictionary<string, string>();
                    if (w.HasValue)
                    {
                        p.Add("w", w.ToString());
                    }
                    if (h.HasValue)
                    {
                        p.Add("h", h.ToString());
                    }
                    if (scale.HasValue)
                    {
                        p.Add("scale", scale.Value.ToString());
                    }
                    if (quality.HasValue)
                    {
                        p.Add("quality", quality.ToString());
                    }
                    if (image.HasValue)
                    {
                        p.Add("404", image.ToString());
                    }
                    if (mode.HasValue)
                    {
                        p.Add("mode", mode.ToString());
                    }
                    return t + p.ToParam("?");
                }
                else if (url.Contains(QinQiuApi.ServerLink))
                {
                    var fileType = System.IO.Path.GetExtension(url);

                    StringBuilder sbUrl = new StringBuilder(url);
                    if (fileType == ".mp4")
                    {
                        sbUrl.Append("?vframe/jpg/offset/1");
                        if (w.HasValue)
                        {
                            sbUrl.Append($"/w/{w}");
                        }
                        if (h.HasValue)
                        {
                            sbUrl.Append($"/h/{h}");
                        }
                        return sbUrl.ToString();
                    }
                    else
                    {
                        sbUrl.Append("?imageView2");
                        switch (mode)
                        {
                            case ResizerMode.Pad:
                            default:
                            case ResizerMode.Crop:
                                sbUrl.Append("/1");
                                break;
                            case ResizerMode.Max:
                                sbUrl.Append("/0");
                                break;
                        }
                        if (w.HasValue)
                        {
                            sbUrl.Append($"/w/{w}");
                        }
                        if (h.HasValue)
                        {
                            sbUrl.Append($"/h/{h}");
                        }
                        quality = quality ?? 100;
                        sbUrl.Append($"/q/{quality}");
                        return sbUrl.ToString();
                    }

                }
                else
                {
                    return url;
                }
            }
        }
        #endregion
    }
}