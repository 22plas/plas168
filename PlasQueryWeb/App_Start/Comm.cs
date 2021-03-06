﻿using PlasCommon;
using PlasModel.App_Start.Qiniu;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
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
        public static IEnumerable<wordModel> FindSearchsWord()
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
        /// <summary>
        /// 大写转小写，其他都保留
        /// </summary>
        /// <param name="key">要转小写的字符串</param>
        /// <returns>返回新的字符串</returns>
        public static string ToLower(string key)
        {
            //string str = "Aa Bb 123!@#";     //测试字符串
            string newStr = string.Empty;    //用于存放新字符串
            //循环字符串
            foreach (char item in key)
            {
                //if (item >= 'a' && item <= 'z')
                //{
                //    //小写字母转大写
                //    newStr += item.ToString().ToUpper();
                //}
                if (item >= 'A' && item <= 'Z')
                {
                    //大写字母转小写
                    newStr += item.ToString().ToLower();
                }
                else
                {
                    //不变
                    newStr += item.ToString();
                }
            }
            return newStr;
        }

        public interface QQIConfig {
            string qqAppID { get; }

            string qqAppSecret { get; }

            string qqAccessToken { get; set; }

            string qqRefreshToken { get; set; }

            DateTime qqAccessTokenEnd { get; set; }
        }
        public interface IConfig
        {
            string AppID { get; }

            string AppSecret { get; }

            string AccessToken { get; set; }

            string RefreshToken { get; set; }

            DateTime AccessTokenEnd { get; set; }

        }

        public class ConfigWeChatWork : IConfig
        {
            /// <summary>
            /// 网页开放平台
            /// </summary>
            private static string appID = "wx10e2e44e6fb65f66";
            //public static string AppID = "1000004";

            /// <summary>
            /// 网页开放平台
            /// </summary>
            private static string appSecret = "317e95dd1568a3d73e5f236cda828456";
            //public static string AppSecret = "-1k1RzNA3Z1lRsOnXF-fYPBxHv4m6fN8zgU_BOA8Y98";


            /// <summary>
            /// AppID对应的公共AccessToken
            /// </summary>
            private static string accessToken = null;

            /// <summary>
            /// AppID对应的公共RefreshToken
            /// </summary>
            private static string refreshToken = null;

            private static DateTime accessTokenEnd;

            public string AccessToken { get { return accessToken; } set { accessToken = value; } }

            public string AppID { get { return appID; } }

            public string AppSecret { get { return appSecret; } }

            public string RefreshToken { get { return refreshToken; } set { refreshToken = value; } }

            public DateTime AccessTokenEnd { get { return accessTokenEnd; } set { accessTokenEnd = value; } }



        }

        public class ConfigPc : IConfig
        {
            private const string appID = "wx10e2e44e6fb65f66";

            private const string appSecret = "317e95dd1568a3d73e5f236cda828456";

            private static string accessToken = null;

            private static string refreshToken = null;

            private static DateTime accessTokenEnd;

            public string AccessToken { get { return accessToken; } set { accessToken = value; } }

            public DateTime AccessTokenEnd { get { return accessTokenEnd; } set { accessTokenEnd = value; } }

            public string AppID { get { return appID; } }

            public string AppSecret { get { return appSecret; } }

            public string RefreshToken { get { return refreshToken; } set { refreshToken = value; } }
        }


        public class ConfigQQtWork : QQIConfig
        {
            /// <summary>
            /// 网页开放平台
            /// </summary>
            private static string qqappID = "101844740";

            /// <summary>
            /// 网页开放平台
            /// </summary>
            private static string qqappSecret = "0de90d3859ad7d5bf22dfe8d73a041d2";


            /// <summary>
            /// AppID对应的公共AccessToken
            /// </summary>
            private static string qqaccessToken = null;

            /// <summary>
            /// AppID对应的公共RefreshToken
            /// </summary>
            private static string qqrefreshToken = null;

            private static DateTime qqaccessTokenEnd;

            public string qqAccessToken { get { return qqaccessToken; } set { qqaccessToken = value; } }

            public string qqAppID { get { return qqappID; } }

            public string qqAppSecret { get { return qqappSecret; } }

            public string qqRefreshToken { get { return qqrefreshToken; } set { qqrefreshToken = value; } }

            public DateTime qqAccessTokenEnd { get { return qqaccessTokenEnd; } set { qqaccessTokenEnd = value; } }
        }

    }
}