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
using System.Text;
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
            int count = 0;
            string errMsg = string.Empty;
            ///浏览记录
            var BrowseList = mbll.GetPhysics_Browse(AccountData.UserID, 1, 8, ref count, ref errMsg);
            ///比较
            var ContrastList = mbll.GetPhysics_Contrast(AccountData.UserID, ref errMsg);

            ViewBag.BrowseList = BrowseList;
            ViewBag.ContrastList = ContrastList;
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
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut()
        {
            HttpCookie aCookie;
            string cookieName;
            int limit = Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                cookieName = Request.Cookies[i].Name;
                aCookie = new HttpCookie(cookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }
            return RedirectToAction("index", "Home");
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
        /// 删除收藏
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


        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <returns></returns>
        public JsonResult AddMaterialColl()
        {
            string errMsg = string.Empty;
            bool isadd = false;
            //var list = arrylist;
            string arry = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["arry"]))
            {
                arry = Request["arry"].ToString();
            }
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(arry) && arry.Length > 0)
            {
                list = JsonConvert.DeserializeObject<List<string>>(arry);
            }
            var userName = string.Empty;
            if (AccountData != null)
            {
                userName = AccountData.UserID;
            }

            PlasBll.MemberCenterBll mbll = new MemberCenterBll();
            var counttrue = 0;
            var countfalse = 0;
            if (list.Count > 0)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    PlasModel.Physics_CollectionModel model = new Physics_CollectionModel();
                    model.ProductGuid = list[i];
                    model.UserId = userName;
                    isadd = mbll.AddPhysics_Collection(model, ref errMsg);
                    if (isadd)
                    {
                        counttrue++;
                    }
                    else
                    {
                        countfalse++;
                    }
                }
                errMsg = "收藏成功 " + counttrue + " 条 ,失败 " + countfalse + " 条" + errMsg;
            }

            return Json(new { isadd = isadd , errMsg = errMsg },JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加对比
        /// </summary>
        /// <returns></returns>
        public JsonResult AddContrast(string ProductId)
        {
            string errmsg = string.Empty;
            PlasBll.MemberCenterBll mbll = new MemberCenterBll();
            var userName = string.Empty;
            bool isContonl = false;
            if (AccountData != null)
            {
                userName = AccountData.UserID;
                if (!string.IsNullOrWhiteSpace(ProductId))
                {
                    PlasModel.Physics_ContrastModel model = new Physics_ContrastModel();
                    model.UserId = userName;
                    model.ProductGuid = ProductId;
                    isContonl = mbll.AddPhysics_Contrast(model, ref errmsg);
                }
                else
                {
                    errmsg = "请选中需要对比的物料！";
                }
            }
            else
            {
                errmsg = "请登录才能添加对比！";
            }
           
            return Json(new { isContonl = isContonl , errmsg = errmsg },JsonRequestBehavior.AllowGet);
        }

        //物性浏览记录
        [UserAttribute]
        public ActionResult MaterialLook()
        {
            Sidebar("物性浏览记录");
            return View();
        }

        /// <summary>
        /// 浏览分页
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public JsonResult GetPhysics_Browse(string pageindex, string pagesize)
        {
            int beginPage = 1;
            int.TryParse(pageindex, out beginPage);
            int endPage = 10;
            int.TryParse(pagesize, out endPage);
            string seletvalue = string.Empty;
            int totalCount = 0;
            string note = string.Empty;
            //seletvalue pulicevalue
            //类型1（型号），2（供应商），3（用途）
            StringBuilder wheresql = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Request["seletvalue"]))
            {
                seletvalue = Request["seletvalue"].ToString();
            }

            string pulicevalue = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["pulicevalue"]))
            {
                pulicevalue = Request["pulicevalue"].ToString();
            }

            if (!string.IsNullOrWhiteSpace(seletvalue) && !string.IsNullOrWhiteSpace(pulicevalue))
            {
                switch (seletvalue)
                {
                    case "1":
                        wheresql.Append(" and d.Name like '%" + pulicevalue + "%'");
                        break;
                    case "2":
                        wheresql.Append(" and b.ProModel like '%" + pulicevalue + "%'");
                        break;
                    case "3":
                        wheresql.Append(" and c.ProUse like '%" + pulicevalue + "%'");
                        break;
                    default:
                        break;
                }
            }
            


            var list = mbll.GetPhysics_Browse(AccountData.UserID, beginPage, endPage, ref totalCount, ref note, wheresql.ToString());
            return Json(new { data = list, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除浏览
        /// </summary>
        /// <returns></returns>
        public JsonResult DelteMaterialColl()
        {
            string errMsg = string.Empty;
            bool isadd = false;
            //var list = arrylist;
            string arry = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["arry"]))
            {
                arry = Request["arry"].ToString();
            }
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(arry) && arry.Length > 0)
            {
                list = JsonConvert.DeserializeObject<List<string>>(arry);
            }
            var userName = string.Empty;
            if (AccountData != null)
            {
                userName = AccountData.UserID;
            }

            PlasBll.MemberCenterBll mbll = new MemberCenterBll();
            if (list.Count > 0)
            {
                isadd= mbll.RomvePhysics_Browse(list, ref errMsg);
            }

            return Json(new { isadd = isadd, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 对比记录
        /// </summary>
        /// <returns></returns>
        public ActionResult Physics_ContrastList()
        {
            Sidebar("物性对比记录");
            var userName = string.Empty;
            string errMsg = string.Empty;
            if (AccountData != null)
            {
                userName = AccountData.UserID;
            }
            var ContrastList = new List<PlasModel.Physics_ContrastModel>();
            if (!string.IsNullOrWhiteSpace(userName))
            {
                ContrastList = mbll.GetPhysics_Contrast(userName, ref errMsg);
            }
            ViewBag.ContrastList = ContrastList;
            return View();
        }

        //删除对比
        public ActionResult DelteMaterialContrast()
        {
            string errMsg = string.Empty;
            bool isadd = false;
            //var list = arrylist;
            string arry = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["arry"]))
            {
                arry = Request["arry"].ToString();
            }
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(arry) && arry.Length > 0)
            {
                list = JsonConvert.DeserializeObject<List<string>>(arry);
            }
            var userName = string.Empty;
            if (AccountData != null)
            {
                userName = AccountData.UserID;
            }

            PlasBll.MemberCenterBll mbll = new MemberCenterBll();
            if (list.Count > 0)
            {
                isadd = mbll.RomvePhysics_Contrast(list, ref errMsg);
            }

            return Json(new { isadd = isadd, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
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
        //获取行情记录
        public JsonResult GetMaterialQuotation(string pageindex, string pagesize)
        {
            int beginPage = 1;
            int.TryParse(pageindex, out beginPage);
            int endPage = 10;
            int.TryParse(pagesize, out endPage);
            string seletvalue = string.Empty;
            int totalCount = 0;
            string note = string.Empty;
            //seletvalue pulicevalue
            //类型1（型号），2（供应商），3（用途）
            StringBuilder wheresql = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Request["seletvalue"]))
            {
                seletvalue = Request["seletvalue"].ToString();
            }

            string pulicevalue = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["pulicevalue"]))
            {
                pulicevalue = Request["pulicevalue"].ToString();
            }

            if (!string.IsNullOrWhiteSpace(seletvalue) && !string.IsNullOrWhiteSpace(pulicevalue))
            {
                switch (seletvalue)
                {
                    case "1":
                        wheresql.Append(" and d.Name like '%" + pulicevalue + "%'");
                        break;
                    case "2":
                        wheresql.Append(" and b.ProModel like '%" + pulicevalue + "%'");
                        break;
                    case "3":
                        wheresql.Append(" and c.ProUse like '%" + pulicevalue + "%'");
                        break;
                    default:
                        break;
                }
            }



            var list = mbll.GetPhysics_Quotation(AccountData.UserID, beginPage, endPage, ref totalCount, ref note);
            return Json(new { data = list, totalCount = totalCount }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 取消行情订阅
        /// </summary>
        /// <returns></returns>
        public JsonResult RomveMaterialQuotation()
        {
            string errMsg = string.Empty;
            bool isadd = false;
            //var list = arrylist;
            string arry = string.Empty;
            if (!string.IsNullOrWhiteSpace(Request["arry"]))
            {
                arry = Request["arry"].ToString();
            }
            var list = new List<string>();
            if (!string.IsNullOrWhiteSpace(arry) && arry.Length > 0)
            {
                list = JsonConvert.DeserializeObject<List<string>>(arry);
            }
            var userName = string.Empty;
            if (AccountData != null)
            {
                userName = AccountData.UserID;
            }

            PlasBll.MemberCenterBll mbll = new MemberCenterBll();
            if (list.Count > 0)
            {
                isadd = mbll.RomvePhysics_Quotation(list, ref errMsg);
            }

            return Json(new { isadd = isadd, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 显示详情
        /// </summary>
        /// <returns></returns>
        public ActionResult QuotationDafaut(string ProductGuid)
        {
            //QuotationDafaut
            PlasBll.ProductBll pbll = new ProductBll();
            var QuotationList = new List<Pri_DayAvgPriceModel>();
            if (!string.IsNullOrWhiteSpace(ProductGuid))
            {
                QuotationList = pbll.GetPri_DayAvgPrice(ProductGuid);
            }
            ViewBag.QuotationList = QuotationList;
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
        /// 保存微信/qq注册数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult WxOrQQSaveRegister(cp_userview model,string type)
        {
            string returnstr = mbll.WxOrQQSaveRegister(model, type);
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
        /// 微信或者qq登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult GetWxOrQQLogin(cp_userview model, string type)
        {
            string tempopenid = "";
            //微信登录
            if (type == "0")
            {
                tempopenid = model.wxopenid;
            }
            //qq登录
            else
            {
                tempopenid = model.qqopenid;
            }
            string returnstr = mbll.WxOrQQLoginBll(tempopenid, type);
            if (returnstr!= "Fail")
            {
                string[] resultstr = returnstr.Split(',');
                //如果微信或者qq已经存在则登录
                if (resultstr.Length > 0)
                {
                    if (resultstr[0].Equals("Success"))
                    {
                        var returndata = new
                        {
                            usid = resultstr[1],
                            phone=resultstr[4]
                        };
                        return Json(Common.ToJsonResult("Success", "登录成功", returndata), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                    }
                }
                else {
                    return Json(Common.ToJsonResult("Fail", "登录失败"), JsonRequestBehavior.AllowGet);
                }
            }
            //不存在则注册,注册成功则返回用户id，注册失败则返回失败状态
            else
            {
                string registerresultstr = mbll.WxOrQQSaveRegister(model, type);
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
                    string returnstr2 = mbll.WxOrQQLoginBll(tempopenid, type);
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
                            return Json(Common.ToJsonResult("Success", "登录成功", returndata2), JsonRequestBehavior.AllowGet);
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
        /// 绑定手机号
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="usid">用户id</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpPost]
        public ActionResult BindUserPhone(string phone, string usid)
        {
            string resultstr = mbll.AccountLoginBll(phone);
            //手机号已存在
            if (resultstr != "NoFind" && resultstr != "Fail")
            {
                return Json(Common.ToJsonResult("Exists", "手机号已绑定其他账号"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                string bindresult = mbll.UpdateUserInfobll("phone", phone, usid);
                if (bindresult == "Success")
                {
                    return Json(Common.ToJsonResult("Success", "绑定成功"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Common.ToJsonResult("Fail", "绑定失败"), JsonRequestBehavior.AllowGet);
                }
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
                    usname = usdt.Rows[0]["UserName"].ToString(),
                    phone= usdt.Rows[0]["phone"].ToString(),
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