using Newtonsoft.Json;
using PlasBll;
using PlasCommon;
using PlasCommon.SqlCommonQuery;
using PlasModel;
using PlasModel.App_Start;
using PlasQueryWeb.App_Start;
using PlasQueryWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static PlasCommon.Enums;

namespace PlasModel.Controllers
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
        [UserAttribute]
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
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="filestr"></param>
        /// <param name="values"></param>
        /// <param name="usid"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        [UserAttribute]
        public ActionResult UpdateUserInfo(string filestr, string values, string usid)
        {
            string resultstr= mbll.UpdateUserInfobll(filestr, values, usid);
            return Json(Common.ToJsonResult(resultstr, "返回信息"), JsonRequestBehavior.AllowGet);
        }
        //用户信息
        //public ActionResult UserInfo()
        //{
        //    Sidebar("个人信息");
        //    return View();
        //}
        //公司资料
        [UserAttribute]
        public ActionResult CompanyInfo(string filter, int? page = 1)
        {
            Sidebar("公司资料");
            List<cp_Company> listcompany = new List<cp_Company>();
            DataSet dtset=mbll.GetCompanyList(AccountData.UserID, filter, page, 20);
            int pagetotal = 0;
            if (dtset.Tables.Count > 0)
            {
                DataTable dt = dtset.Tables[0];
                DataTable totaldt = dtset.Tables[1];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cp_Company m = new cp_Company();
                        m.AccountOpenBank = dt.Rows[i]["AccountOpenBank"].ToString();
                        m.AccountOpening = dt.Rows[i]["AccountOpening"].ToString();
                        m.Address = dt.Rows[i]["Address"].ToString();
                        m.Area = dt.Rows[i]["Area"].ToString();
                        m.City = dt.Rows[i]["City"].ToString();
                        m.Contacts = dt.Rows[i]["Contacts"].ToString();
                        m.Email = dt.Rows[i]["Email"].ToString();
                        m.Fax = dt.Rows[i]["Fax"].ToString();
                        m.image = dt.Rows[i]["image"].ToString();
                        m.Mobile = dt.Rows[i]["Mobile"].ToString();
                        m.Name = dt.Rows[i]["Name"].ToString();
                        m.OpenBank = dt.Rows[i]["OpenBank"].ToString();
                        m.Province = dt.Rows[i]["Province"].ToString();
                        m.TaxId = dt.Rows[i]["TaxId"].ToString();
                        m.Tel = dt.Rows[i]["Tel"].ToString();
                        m.WeChat = dt.Rows[i]["WeChat"].ToString();
                        m.Id = dt.Rows[i]["Id"].ToString();
                        listcompany.Add(m);
                    }
                }
                //计算页数
                if (totaldt.Rows.Count>0)
                {
                    int total = Convert.ToInt32(totaldt.Rows[0]["totals"]);
                    if (total > 0)
                    {
                        int temppagenumber = total / 20;
                        int qynumber = 0;
                        if (temppagenumber > 0)
                        {
                            qynumber = total - (temppagenumber * 20);
                            if (qynumber > 0)
                            {
                                pagetotal = temppagenumber + 1;
                            }
                            else
                            {
                                pagetotal = temppagenumber;
                            }
                        }
                        else
                        {
                            pagetotal = 1;
                        }
                    }
                }
                ViewBag.pagetotal = pagetotal;
            }


            return View(listcompany);
        }
        //新增公司资料
        [UserAttribute]
        public ActionResult CompanyInfoCreate()
        {
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("","");//省份
            ViewBag.City = abll.pliststrbll("北京市","1"); //城市
            ViewBag.District = abll.pliststrbll("北京市","2");//街道
            Sidebar("公司资料");
            var model = new cp_CompanyView();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserAttribute]
        public ActionResult CompanyInfoCreate(cp_CompanyView model)
        {
            ////var model = new cp_CompanyView();
            //return View(model);
            if (ModelState.IsValid)
            {
                cp_Company tempmodel = new cp_Company();
                tempmodel.AccountOpenBank = model.AccountOpenBank;
                tempmodel.AccountOpening = model.AccountOpening;
                tempmodel.Address = model.Address;
                tempmodel.Area = model.Area;
                tempmodel.City = model.City;
                tempmodel.Contacts = model.Contacts;
                tempmodel.CreateDate = model.CreateDate;
                tempmodel.Email = model.Email;
                tempmodel.Fax = model.Fax;
                tempmodel.Id = model.Id;
                tempmodel.isdefault = model.isdefault;
                tempmodel.logo = string.Empty;
                tempmodel.image = string.Join(",", model.image.Images) ;
                tempmodel.Mobile = model.Mobile;
                tempmodel.Name = model.Name;
                tempmodel.OpenBank = model.OpenBank;
                tempmodel.Province = model.Province;
                tempmodel.QQ = model.QQ;
                tempmodel.TaxId = model.TaxId;
                tempmodel.Tel = model.Tel;
                tempmodel.Trade = model.Trade;
                tempmodel.UserId = AccountData.UserID;
                tempmodel.WeChat = model.WeChat;
                mbll.EditCompanyInfoBll(tempmodel, Operation.Add);
                return RedirectToAction("CompanyInfo");
            }
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("", "");//省份
            ViewBag.City = abll.pliststrbll("北京市", "1"); //城市
            ViewBag.District = abll.pliststrbll("北京市", "2");//街道
            Sidebar("公司资料");
            return View(model);
        }
        //编辑公司资料
        [UserAttribute]
        public ActionResult CompanyInfoEdit(string Id)
        {

            cp_CompanyView tempmodel = new cp_CompanyView();
            var model= mbll.GetCompanyById(Id);
            tempmodel.AccountOpenBank = model.AccountOpenBank;
            tempmodel.AccountOpening = model.AccountOpening;
            tempmodel.Address = model.Address;
            tempmodel.Area = model.Area;
            tempmodel.City = model.City;
            tempmodel.Contacts = model.Contacts;
            tempmodel.CreateDate = model.CreateDate;
            tempmodel.Email = model.Email;
            tempmodel.Fax = model.Fax;
            tempmodel.Id = model.Id;
            tempmodel.isdefault = model.isdefault;
            tempmodel.logo = string.Empty;
            tempmodel.image.Images = model.image?.Split(',') ?? new string[0];
            tempmodel.Mobile = model.Mobile;
            tempmodel.Name = model.Name;
            tempmodel.OpenBank = model.OpenBank;
            tempmodel.Province = model.Province;
            tempmodel.QQ = model.QQ;
            tempmodel.TaxId = model.TaxId;
            tempmodel.Tel = model.Tel;
            tempmodel.Trade = model.Trade;
            tempmodel.UserId = AccountData.UserID;
            tempmodel.WeChat = model.WeChat;
            tempmodel.Id = model.Id;
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("", "");//省份
            ViewBag.City = abll.pliststrbll(model.Province, "1"); //城市
            ViewBag.District = abll.pliststrbll(model.City, "2");//街道
            Sidebar("公司资料");
            return View(tempmodel);
        }

        //保存编辑公司信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserAttribute]
        public ActionResult CompanyInfoEdit(cp_CompanyView model)
        {
            if (ModelState.IsValid)
            {
                cp_Company tempmodel = new cp_Company();
                tempmodel.AccountOpenBank = model.AccountOpenBank;
                tempmodel.AccountOpening = model.AccountOpening;
                tempmodel.Address = model.Address;
                tempmodel.Area = model.Area;
                tempmodel.City = model.City;
                tempmodel.Contacts = model.Contacts;
                tempmodel.CreateDate = model.CreateDate;
                tempmodel.Email = model.Email;
                tempmodel.Fax = model.Fax;
                tempmodel.Id = model.Id;
                tempmodel.isdefault = model.isdefault;
                tempmodel.logo = string.Empty;
                tempmodel.image = string.Join(",", model.image.Images);
                tempmodel.Mobile = model.Mobile;
                tempmodel.Name = model.Name;
                tempmodel.OpenBank = model.OpenBank;
                tempmodel.Province = model.Province;
                tempmodel.QQ = model.QQ;
                tempmodel.TaxId = model.TaxId;
                tempmodel.Tel = model.Tel;
                tempmodel.Trade = model.Trade;
                tempmodel.UserId = AccountData.UserID;
                tempmodel.WeChat = model.WeChat;
                tempmodel.Id = model.Id;
                mbll.EditCompanyInfoBll(tempmodel, Operation.Update);
                return RedirectToAction("CompanyInfo");
            }
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("", "");//省份
            ViewBag.City = abll.pliststrbll("北京市", "1"); //城市
            ViewBag.District = abll.pliststrbll("北京市", "2");//街道
            Sidebar("公司资料");
            return View(model);
        }
        //删除公司信息
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult DeleteCompanyInfo(string id)
        {
            bool returns = mbll.DeleteCommon(id, "cp_Company");
            if (returns)
            {
                return Json(Common.ToJsonResult("Success", "删除成功"), JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(Common.ToJsonResult("Fail", "删除失败"), JsonRequestBehavior.AllowGet);
            }
        }
        //收货地址
        [UserAttribute]
        public ActionResult DeliveryAddress(string filter, int? page = 1)
        {
            Sidebar("收货地址");
            List<DeliveryAddress> list = new List<DeliveryAddress>();
            DataSet dtset = mbll.GetDeliveryAddressList(AccountData.UserID, filter, page, 20);
            int pagetotal = 0;
            if (dtset.Tables.Count > 0)
            {
                DataTable dt = dtset.Tables[0];
                DataTable totaldt = dtset.Tables[1];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DeliveryAddress m = new DeliveryAddress();
                        m.Address = dt.Rows[i]["Address"].ToString();
                        m.Count = dt.Rows[i]["Count"].ToString();
                        m.City = dt.Rows[i]["City"].ToString();
                        m.Contacts = dt.Rows[i]["Contacts"].ToString();
                        m.ContactsMobile = dt.Rows[i]["ContactsMobile"].ToString();
                        m.Province = dt.Rows[i]["Province"].ToString();
                        m.Tel = dt.Rows[i]["Tel"].ToString();
                        m.Id = dt.Rows[i]["Id"].ToString();
                        m.IsDefaultStr= (YesOrNo)dt.Rows[i]["IsDefault"]== YesOrNo.No? YesOrNo.No.GetDisplayName(): YesOrNo.Yes.GetDisplayName();
                        list.Add(m);
                    }
                }
                //计算页数
                if (totaldt.Rows.Count > 0)
                {
                    int total = Convert.ToInt32(totaldt.Rows[0]["totals"]);
                    if (total > 0)
                    {
                        int temppagenumber = total / 20;
                        int qynumber = 0;
                        if (temppagenumber > 0)
                        {
                            qynumber = total - (temppagenumber * 20);
                            if (qynumber > 0)
                            {
                                pagetotal = temppagenumber + 1;
                            }
                            else
                            {
                                pagetotal = temppagenumber;
                            }
                        }
                        else
                        {
                            pagetotal = 1;
                        }
                    }
                }
                ViewBag.pagetotal = pagetotal;
            }
            return View(list);
        }
        //收货地址
        [UserAttribute]
        public ActionResult DeliveryAddressCreate()
        {
            Sidebar("收货地址");
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("", "");//省份
            ViewBag.City = abll.pliststrbll("北京市", "1"); //城市
            ViewBag.District = abll.pliststrbll("北京市", "2");//街道
            var model = new DeliveryAddress();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserAttribute]
        public ActionResult DeliveryAddressCreate(DeliveryAddress model)
        {
            if (ModelState.IsValid)
            {
                Sidebar("收货地址");
                model.Fax = string.Empty;
                model.QQ = string.Empty;
                model.WeChat = string.Empty;
                model.UserId = AccountData.UserID;
                mbll.EditDeliveryAddressInfoBll(model, Operation.Add);
                return RedirectToAction("DeliveryAddress");
            }
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("", "");//省份
            ViewBag.City = abll.pliststrbll("北京市", "1"); //城市
            ViewBag.District = abll.pliststrbll("北京市", "2");//街道
            return View(model);
        }

        //编辑公司资料
        [UserAttribute]
        public ActionResult DeliveryAddressEdit(string Id)
        {

            var model = mbll.GetDeliveryAddressById(Id);
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("", "");//省份
            ViewBag.City = abll.pliststrbll(model.Province, "1"); //城市
            ViewBag.District = abll.pliststrbll(model.City, "2");//街道
            Sidebar("收货地址");
            return View(model);
        }

        //保存编辑公司信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        [UserAttribute]
        public ActionResult DeliveryAddressEdit(DeliveryAddress model)
        {
            if (ModelState.IsValid)
            {
                model.UserId= AccountData.UserID;
                mbll.EditDeliveryAddressInfoBll(model, Operation.Update);
                return RedirectToAction("DeliveryAddress");
            }
            AreaBll abll = new AreaBll();
            ViewBag.Province = abll.pliststrbll("", "");//省份
            ViewBag.City = abll.pliststrbll("北京市", "1"); //城市
            ViewBag.District = abll.pliststrbll("北京市", "2");//街道
            Sidebar("收货地址");
            return View(model);
        }
        //删除收货地址
        [AllowCrossSiteJson]
        [HttpPost]
        [UserAttribute]
        public ActionResult DeleteDeliverAddress(string id)
        {
            bool returns = mbll.DeleteCommon(id, "cp_CompanyAddress");
            if (returns)
            {
                return Json(Common.ToJsonResult("Success", "删除成功"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "删除失败"), JsonRequestBehavior.AllowGet);
            }
        }
        //物性收藏
        [UserAttribute]
        public ActionResult MaterialCollection()
        {
            string note = string.Empty;
            var list = mbll.getProductAttr(AccountData.UserID, ref note);
            Sidebar("物性收藏");
            ViewBag.ProductList = list;
            return View();
        }

        [UserAttribute]
        public JsonResult GetMaterialCollection(string pageindex,string pagesize)
        {
            int beginPage = 1;
            int.TryParse(pageindex, out beginPage);
            int endPage = 10;
            int.TryParse(pagesize, out endPage);
            string smallid = string.Empty;
            int totalCount = 0;
            string note = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["smallid"]))
            {
                smallid = Request["smallid"].ToString();
            }
            var list = mbll.GetPhysics_Collection(AccountData.UserID, smallid, beginPage, endPage, ref totalCount, ref note);
            return Json(new { data = list, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult RomveMaterialCollection()
        {
            string arry = string.Empty;
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(Request["arry"]))
            {
                arry = Request["arry"].ToString();
                list = JsonConvert.DeserializeObject<List<string>>(arry);
            }
            string note = string.Empty;
            bool count = false;
            if (list.Count > 0)
            {
                count= mbll.RomvePhysics_Collection(list, ref note);
            }
            return Json(new { count = count ,message= note },JsonRequestBehavior.AllowGet);
        }

        //物性浏览记录
        [UserAttribute]
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
        
        [HttpGet]
        [AllowCrossSiteJson]
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
                    var returndata = new
                    {
                        usid = resultstr[1]
                    };
                    return Json(Common.ToJsonResult("Success", "登录成功", returndata), JsonRequestBehavior.AllowGet);
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
        /// <summary>
        /// 短信登录(只用手机号登录)
        /// </summary>
        /// <param name="account">手机号</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetAccountLogin(string account)
        {
            string returnstr = mbll.AccountLoginBll(account);
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
                    var returndata = new
                    {
                        usid = resultstr[1]
                    };
                    return Json(Common.ToJsonResult("Success", "登录成功", returndata), JsonRequestBehavior.AllowGet);
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
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="account">手机号</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult UpdateUserPwd(string phone, string newpwd)
        {
            string returnstr = mbll.UpdateUserPwd(phone, newpwd);
            string[] resultstr = returnstr.Split(',');
            string tempstr = string.Empty;
            if (returnstr == "NoFind")
            {
                return Json(Common.ToJsonResult("NoFind", "用户不存在"), JsonRequestBehavior.AllowGet);
            }
            else if (returnstr == "Fail")
            {
                return Json(Common.ToJsonResult("Fail", "修改失败"), JsonRequestBehavior.AllowGet);
            }
            else if (returnstr == "Success")
            {
                return Json(Common.ToJsonResult("Success", "修改成功"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "修改失败"), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetUserInfo(string usid)
        {
            DataTable usdt = mbll.GetUserInfo(usid);
            if (usdt.Rows.Count > 0)
            {
                var returndata = new
                {
                    usname = usdt.Rows[0]["Phone"].ToString(),
                    headimage = usdt.Rows[0]["HeadImage"].ToString()
                };
                return Json(Common.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "获取失败"), JsonRequestBehavior.AllowGet);
            }
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