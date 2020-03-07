using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlasModel.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PlasModel.App_Start.Comm;

namespace PlasQueryWeb.App_Start
{
    public class QQApi
    {
        private QQIConfig _config;

        public QQApi(QQIConfig config)
        {
            _config = config;
        }
        public string AppID { get { return _config.qqAppID; } }

        public string Secret { get { return _config.qqAppSecret; } }

        /// <summary>
        /// 使用Code换取OpenID，UnionID，Token等
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public qqAccessTokenResult GetAccessTokenSns(string code)
        {

            string returnurl = "http://168plas.com/Account/LoginByQQ";// "http%3A%2F%2F168plas.com%2FAccount%2FLoginByQQ";// HttpUtility.UrlEncode("http://168plas.com/Account/LoginByQQ"); // Server.UrlEncode("http://168plas.com/Account/LoginByQQ");
            PlasCommon.Common.AddLog("", "qq获取AccessToken的url", "qq登录看返回值", returnurl);
            var p = new Dictionary<string, string>();
            p.Add("grant_type", "authorization_code");
            p.Add("client_id", _config.qqAppID);
            p.Add("client_secret",_config.qqAppSecret);
            p.Add("code", code);
            p.Add("redirect_uri", returnurl);
            string url = $"https://graph.qq.com/oauth2.0/token{p.ToParam("?")}";
            PlasCommon.Common.AddLog("", "qq获取AccessToken请求url", "qq登录看返回值", url);
            //Comm.WriteLog("GetAccessTokenSns", url, DebugLogLevel.Normal);
            var result = new CommonApi.BaseApi(url, "GET").CreateRequestReturnJson();
            PlasCommon.Common.AddLog("", "qq获取AccessToken", "qq登录看返回值", result.ToString());
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }
            _config.qqAccessToken = result["access_token"].Value<string>();
            _config.qqRefreshToken = result["refresh_token"].Value<string>();
            return new qqAccessTokenResult
            {
                qqAccessToken = _config.qqAccessToken,
                qqRefreshToken = _config.qqRefreshToken
            };
        }

        /// <summary>
        /// 刷新qq登录的Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public qqAccessTokenResult RefreshAccessTokenSns()
        {
            var p = new Dictionary<string, string>();
            p.Add("grant_type", "refresh_token");
            p.Add("client_id", _config.qqAppID);
            p.Add("client_secret", _config.qqAppSecret);
            p.Add("refresh_token", _config.qqRefreshToken);
            var result = new CommonApi.BaseApi($"https://graph.qq.com/oauth2.0/token{p.ToParam("?")}", "GET").CreateRequestReturnJson();
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }
            _config.qqAccessToken = result["access_token"].Value<string>();
            _config.qqRefreshToken = result["refresh_token"].Value<string>();
            return new qqAccessTokenResult
            {
                qqAccessToken = _config.qqAccessToken,
                qqRefreshToken = _config.qqRefreshToken
            };
        }

        /// <summary>
        /// 获取qq用户openID
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public qqAccessTokenResult GetUserOpenID()
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());
            try
            {
                var result = new CommonApi.BaseApi($"https://graph.qq.com/oauth2.0/me{p.ToParam("?")}", "GET")
                 .CreateRequestReturnJson();
                if (result["errcode"] != null)
                {
                    throw new Exception(JsonConvert.SerializeObject(result));
                }
                return new qqAccessTokenResult
                {
                    qqOpenID = result["openid"].Value<string>()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取qq用户基本信息，通过openID,Token内嵌
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public UserInfoResult GetUserInfoCgi(string openID)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());
            p.Add("oauth_consumer_key", _config.qqAppID);
            p.Add("openid", openID);
            try
            {
                var result = new CommonApi.BaseApi($"https://graph.qq.com/user/get_user_info{p.ToParam("?")}", "GET")
                 .CreateRequestReturnJson();
                if (result["errcode"] != null)
                {
                    throw new Exception(JsonConvert.SerializeObject(result));
                }
                return new UserInfoResult
                {
                    NickName = result["nickname"].Value<string>(),
                    HeadImgUrl = result["figureurl"].Value<string>(),
                };
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            if (_config.qqAccessToken == null || _config.qqAccessTokenEnd <= DateTime.Now)
            {
                qqRefreshToken();
            }
            return _config.qqAccessToken;
        }
        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <returns></returns>
        public void qqRefreshToken()
        {
            var p = new Dictionary<string, string>();
            p.Add("grant_type", "refresh_token");
            p.Add("client_id", _config.qqAppID);
            p.Add("client_secret", _config.qqAppSecret);
            p.Add("refresh_token", _config.qqRefreshToken);
            var result = new CommonApi.BaseApi($"https://graph.qq.com/oauth2.0/token{p.ToParam("?")}", "GET").CreateRequestReturnJson();
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }
            _config.qqAccessToken = result["access_token"].Value<string>();
            _config.qqAccessTokenEnd = DateTime.Now.AddSeconds(3500);
        }
    }
    public class qqAccessTokenResult
    {
        public string qqOpenID { get; set; }

        public string qqAccessToken { get; set; }

        public string qqUnionID { get; set; }

        public string qqRefreshToken { get; set; }
    }
}