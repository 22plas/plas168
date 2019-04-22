using PlasCommon;
using PlasModel;
using PlasQueryWeb.App_Start;
using PlasQueryWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class MemberCenterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Sidebar(string name = "用户管理")
        {
            ViewBag.Sidebar = name;

        }
        //
        // GET: /会员中心首页/
        public ActionResult Index()
        {
            Sidebar("概览");
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
        public ActionResult UserInfo()
        {
            Sidebar("个人信息");
            return View();
        }
        //公司资料
        public ActionResult CompanyInfo()
        {
            Sidebar("公司资料");
            return View();
        }
        //新增公司资料
        public ActionResult CompanyInfoCreate()
        {
            return View();
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
        public ActionResult SaveRegister(cp_user model)
        {
            if (db.cp_user.Any(s => s.Phone == model.Phone))
            {
                return Json(Common.ToJsonResult("Error", "用户已存在"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.LeaderUserName = string.Empty;
                db.cp_user.Add(model);
                db.SaveChanges();
            }
            return Json(Common.ToJsonResult("Fail", "失败"), JsonRequestBehavior.AllowGet);
        }
    }
}