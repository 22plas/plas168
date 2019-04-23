using PlasCommon;
using PlasCommon.SqlCommonQuery;
using PlasModel;
using PlasQueryWeb.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class MemberCenterController : Controller
    {
        EFDBContext db = new EFDBContext();
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
            try
            {
                if (db.cp_user.Any(s => s.Phone == model.Phone))
                {
                    return Json(Common.ToJsonResult("AlreadyExist", "用户已存在"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.LeaderUserName = "";
                    model.HeadImage = "";
                    model.CreateDate = DateTime.Now;
                    model.states = 0;
                    model.WeChat = "";
                    model.ErrorCount = 0;
                    model.ErrorDate = "1990-01-01";
                    model.Email = "";
                    model.UserPwd = ToolHelper.MD5_SET(model.UserPwd);
                    db.cp_user.Add(model);
                    int row = db.SaveChanges();
                    if (row > 0)
                    {
                        return Json(Common.ToJsonResult("Success", "注册成功"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(Common.ToJsonResult("Fail", "失败"), JsonRequestBehavior.AllowGet);
                    }
                }

                //        string execsql = string.Format(@"select * from cp_user where phone ='{0}'", model.Phone);
                //        int haveusercount = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, execsql, null);
                //        if (haveusercount > 0)
                //        {
                //            return Json(Common.ToJsonResult("Error", "用户已存在"), JsonRequestBehavior.AllowGet);
                //        }
                //        else
                //        {

                //            //拼接参数
                //            SqlParameter[] parameters = {
                //                            new SqlParameter("@UserName", SqlDbType.NVarChar),
                //                            new SqlParameter("@UserPwd", SqlDbType.NVarChar),
                //                            new SqlParameter("@Email", SqlDbType.NVarChar),
                //                            new SqlParameter("@Phone", SqlDbType.NVarChar),
                //                            new SqlParameter("@Address", SqlDbType.NVarChar)
                //                        };
                //            parameters[0].Value = model.UserName;
                //            parameters[1].Value = model.UserPwd;
                //            parameters[2].Value = model.Email==null?"":model.Email;
                //            parameters[3].Value = model.Phone;
                //            parameters[4].Value = model.Address == null ? "" : model.Address;
                //            string sqlstr = string.Empty;
                //            string addsql = string.Format(@"INSERT dbo.cp_user
                //( UserName ,UserPwd ,Email ,Phone ,Address ,TestQQ ,CreateDate ,states ,ErrorDate ,ErrorCount ,WeChat ,ContentAddress ,LeaderUserName,HeadImage)
                //VALUES  ( @UserName ,@UserPwd ,@Email ,@Phone ,@Address ,N'' ,GETDATE() ,0 ,null ,0 ,N'' ,N'' ,'','')");
                //            int addrow = SqlHelper.ExecuteScalar(SqlHelper.ConnectionStrings, addsql, parameters);
                //            if (addrow > 0)
                //            {
                //                return Json(Common.ToJsonResult("Success", "注册成功"), JsonRequestBehavior.AllowGet);
                //            }
                //            else
                //            {
                //                return Json(Common.ToJsonResult("Fail", "失败"), JsonRequestBehavior.AllowGet);
                //            }
                //        }
            }
            catch (Exception ex)
            {
                return Json(Common.ToJsonResult("Fail", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}