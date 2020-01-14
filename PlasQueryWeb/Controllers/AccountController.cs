using PlasCommon;
using PlasModel;
using PlasModel.App_Start;
using PlasQueryWeb.App_Start;
using PlasQueryWeb.App_Start.CommonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PlasModel.App_Start.Comm;

namespace PlasQueryWeb.Controllers
{
    public class AccountController : Controller
    {
        private PlasBll.MemberCenterBll mbll = new PlasBll.MemberCenterBll();
        //获取当前登录的用户信息
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        #region 微信对接

        [HttpGet]
        public ActionResult LoginByWeiXinSilence(string state)
        {
            IConfig config = new ConfigWeChatWork();
            var p = new Dictionary<string, string>();
            string tempurl = Server.UrlEncode("https://168plas.com/Account/LoginByWeiXin");
            p.Add("appid", config.AppID);
            p.Add("redirect_uri", tempurl);
            p.Add("response_type", "code");
            p.Add("scope", "snsapi_login");
            p.Add("state", Server.UrlEncode(state));
            //return Redirect($"https://open.weixin.qq.com/connect/oauth2/authorize{p.ToParam("?")}#wechat_redirect");
            string tempurls = $"https://open.weixin.qq.com/connect/qrconnect{p.ToParam("?")}#wechat_redirect";
            return Redirect(tempurls);
        }

        /// <summary>
        /// 微信授权登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state">扩展参数</param>
        /// <param name="type">端口类别（Web App Mini）</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult LoginByWeiXin(string code, string state = null)
        {

            Func<string, string, ActionResult> error = (content, detail) =>
            {
                return this.ToError("错误", detail, Url.Action("Login", "Account"));
            };
            if (string.IsNullOrWhiteSpace(code))
            {
                return error("请求有误", "Code不能为空");
            }

            IConfig config = new ConfigWeChatWork();
            WeChatApi wechat = new WeChatApi(config);
            AccessTokenResult result;
            try
            {
                result = wechat.GetAccessTokenSns(code);
                var openID = result.OpenID;
                if (state == "openid")
                {
                    config.AccessToken = result.AccessToken;
                    //Response.Cookies["WeChatOpenID"].Value = openID;
                    AccountData.WeChatOpenID = openID;
                    var userInfo = wechat.GetUserInfoSns(openID);
                    string returnstr = mbll.WxOrQQLoginBll(openID, "0");
                    if (returnstr != "Fail")
                    {
                        string[] resultstr = returnstr.Split(',');
                        //如果微信已经存在则登录
                        if (resultstr.Length > 0)
                        {
                            
                            mbll.UpdateUserInfobll("HeadImage", userInfo.HeadImgUrl, resultstr[1]);
                            mbll.UpdateUserInfobll("UserName", userInfo.NickName, resultstr[1]);
                            if (resultstr[0].Equals("Success"))
                            {
                                var returndata = new
                                {
                                    usid = resultstr[1],
                                    phone = resultstr[4]
                                };
                                PlasCommon.Common.AddLog("", "微信登录成功", "登录成功" + AccountData.WeChatOpenID, "");
                                return RedirectToAction("Home", "Index");
                            }
                            else
                            {
                                return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                        }
                    }
                    //不存在则注册,注册成功则返回用户id，注册失败则返回失败状态
                    else
                    {
                        cp_userview model = new cp_userview();
                        model.UserName = userInfo.NickName;
                        model.wxopenid = openID;
                        model.HeadImage = userInfo.HeadImgUrl;
                        string registerresultstr = mbll.WxOrQQSaveRegister(model,"0");
                        //已经存在
                        if (registerresultstr == "AlreadyExist")
                        {
                            return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                        }
                        else if (registerresultstr == "Fail")
                        {
                            return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                        }
                        else if (registerresultstr == "Success")
                        {
                            string returnstr2 = mbll.WxOrQQLoginBll(openID, "0");
                            string[] resultstr2 = returnstr2.Split(',');
                            if (resultstr2.Length > 0)
                            {
                                if (resultstr2[0].Equals("Success"))
                                {
                                    var returndata2 = new
                                    {
                                        usid = resultstr2[1],
                                        phone = resultstr2[4]
                                    };
                                    PlasCommon.Common.AddLog("", "微信登录成功", "登录成功" + AccountData.WeChatOpenID, "");
                                    return RedirectToAction("Home", "Index");
                                }
                                else
                                {
                                    return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(Common.ToJsonResult("Error", "登录成功"));
                }
            }
            catch (Exception ex)
            {
                return error("请求有误", ex.Message);
            }
        }
        #endregion

        #region qq授权登录回调方法
        public ActionResult LoginByQQ(string code, string state = null)
        {
            Func<string, string, ActionResult> error = (content, detail) =>
            {
                return this.ToError("错误", detail, Url.Action("Home", "Index"));
            };
            if (string.IsNullOrWhiteSpace(code))
            {
                return error("请求有误", "Code不能为空");
            }
            try
            {
                string tempurl = Server.UrlEncode("https://168plas.com/Account/LoginByQQ");
                QQIConfig config = new ConfigQQtWork();
                QQApi qqapi = new QQApi(config);
                qqAccessTokenResult result;
                result = qqapi.GetAccessTokenSns(code, tempurl);
                var openID = result.qqOpenID;

                if (state == "suyiqqlogin")
                {
                    config.qqAccessToken = result.qqAccessToken;
                    //Response.Cookies["WeChatOpenID"].Value = openID;
                    AccountData.QQOpenID = openID;
                    //return Json(Common.ToJsonResult("Success", "成功", new { OpenID = openID }));
                    PlasCommon.Common.AddLog("", "qq登录成功", "登录成功" + AccountData.WeChatOpenID, "");
                    return RedirectToAction("Home", "Index");
                }
                else
                {
                    return Json(Common.ToJsonResult("Error", "登录成功"));
                }
            }
            catch (Exception ex)
            {
                return error("请求有误", ex.Message);
            }
        }
        #endregion
    }
}