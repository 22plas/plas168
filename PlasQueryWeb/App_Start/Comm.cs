using PlasModel.App_Start.Qiniu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using static PlasCommon.Enums;

namespace PlasModel.App_Start
{
    public static class Comm
    {
        private static Random _random;
        /// <summary>
        /// 系统唯一随机
        /// </summary>
        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }
                return _random;
            }
        }
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

        /// <summary>
        /// DataTable转成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToDataList<T>(this DataTable dt)
        {
            var list = new List<T>();
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            foreach (DataRow item in dt.Rows)
            {
                T s = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        try
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object v = null;
                                if (info.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    v = Convert.ChangeType(item[i], Nullable.GetUnderlyingType(info.PropertyType));
                                }
                                else
                                {
                                    v = Convert.ChangeType(item[i], info.PropertyType);
                                }
                                info.SetValue(s, v, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("字段[" + info.Name + "]转换出错," + ex.Message);
                        }
                    }
                }
                list.Add(s);
            }
            return list;
        }
    }
}