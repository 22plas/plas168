﻿using PlasCommon;
using PlasCommon.SqlCommonQuery;
using PlasModel;
using PlasQueryWeb.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static PlasCommon.Enums;

namespace PlasQueryWeb.Controllers
{
    public class MemberCenterController : Controller
    {
        //获取当前登录的用户信息
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        private PlasBll.MemberCenterBll mbll = new PlasBll.MemberCenterBll();
        public void Sidebar(string name = "用户管理")
        {
            ViewBag.Sidebar = name;

        }
     
        //
        // GET: /会员中心首页/
        public ActionResult Index()
        {
            ViewBag.username = AccountData.UserName;
            ViewBag.userheadimage = AccountData.HeadImage;
            ViewBag.timestr = Common.GetTimeStr();
            DataTable usdt = mbll.GetUserInfo(AccountData.UserID);
            if (usdt.Rows.Count > 0)
            {
                ViewBag.balance = usdt.Rows[0]["Balance"].ToString();
                ViewBag.Intotal = usdt.Rows[0]["Intotal"].ToString();
                ViewBag.OutTotal = usdt.Rows[0]["OutTotal"].ToString();
            }
            else
            {
                ViewBag.balance = "0";
                ViewBag.Intotal = "0";
                ViewBag.OutTotal = "0";
            }
            Sidebar("个人中心");
            return View();
        }
        //登录
        public ActionResult Login()
        {
            return View();
        }
        //注册
        public ActionResult Register()
        {
            return View();
        }
        //用户信息
        //public ActionResult UserInfo()
        //{
        //    Sidebar("个人信息");
        //    return View();
        //}
        //公司资料
        public ActionResult CompanyInfo()
        {
            Sidebar("公司资料");
            return View();
        }
        //新增公司资料
        public ActionResult CompanyInfoCreate()
        {
            Sidebar("公司资料");
            var model = new cp_Company();
            return View(model);
        }
        //收货地址
        public ActionResult DeliveryAddress()
        {
            Sidebar("收货地址");
            return View();
        }
        //收货地址
        public ActionResult DeliveryAddressCreate()
        {
            Sidebar("收货地址");
            return View();
        }
        //物性收藏
        public ActionResult MaterialCollection()
        {
            Sidebar("物性收藏");
            return View();
        }
        //物性浏览记录
        public ActionResult MaterialLook()
        {
            Sidebar("物性浏览记录");
            return View();
        }
        //我要报价
        public ActionResult SellerOffer()
        {
            Sidebar("卖家报价");
            return View();
        }
        //新增卖家报价
        public ActionResult SellerOfferCreate()
        {
            Sidebar("卖家报价");
            return View();
        }

        //买家询价
        public ActionResult BuyerOffer()
        {
            Sidebar("买家询价");
            return View();
        }
        //新增买家询价
        public ActionResult BuyerOfferCreate()
        {
            Sidebar("买家询价");
            return View();
        }

        //收到的报价
        public ActionResult GetSellerOffer()
        {
            Sidebar("收到的报价");
            return View();
        }
        //收到的询价
        public ActionResult GetBuyerOffer()
        {
            Sidebar("收到的询价");
            return View();
        }

        //行情浏览记录
        public ActionResult QuotationRecord()
        {
            Sidebar("行情浏览记录");
            return View();
        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult SetCode(string phone)
        {
            string returncode = Common.GenerateCheckCodeNum(6);
            bool returnresult = Common.SmsSend(phone, returncode);
            if (returnresult)
            {
                var returnstr = new
                {
                    code = returncode
                };
                return Json(Common.ToJsonResult("Success", "成功", returnstr), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "失败"), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 保存注册数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult SaveRegister(cp_userview model)
        {
            string returnstr = mbll.SaveRegister(model);
            return Json(Common.ToJsonResult(returnstr, "返回结果"), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetLogin(string account, string password)
        {
            string returnstr = mbll.LoginBll(account, password);
            string[] resultstr = returnstr.Split(',');
            string tempstr = string.Empty;
            if (resultstr.Length > 0)
            {
                if (resultstr[0].Equals("Success"))
                {
                    //把重要的用户信息进行加密，存放到cookie
                    this.SetAccountData(new AccountData
                    {
                        UserID = resultstr[1],
                        UserName = resultstr[2],
                        HeadImage = resultstr[3]

                    });
                }
                tempstr = resultstr[0];
            }
            else
            {
                tempstr = returnstr;
            }
            return Json(Common.ToJsonResult(tempstr, "返回结果"), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 个人中心母版页获取用户信息
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult UserInfoPartial()
        {
            cp_userview um = new cp_userview();
            um.UserName = AccountData.UserName;
            return Json(Common.ToJsonResult("Success", "成功", um), JsonRequestBehavior.AllowGet);
        }
    }
}