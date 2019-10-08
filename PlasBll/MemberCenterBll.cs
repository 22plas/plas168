using PlasDal;
using PlasModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasCommon.Enums;

namespace PlasBll
{
    public class MemberCenterBll
    {
        MemberCenterDal mdal = new MemberCenterDal();
        //保存用户注册
        public string SaveRegister(cp_userview model)
        {
            try
            {
                DataTable dt = mdal.GetUserDt(model.Phone, "phone");
                if (dt.Rows.Count > 0)
                {
                    return "AlreadyExist";
                }
                else
                {
                    DataTable dt2 = GetUserByUserCode(model.ParentUserCode);
                    string parentid = string.Empty;//上上级用户id
                    string leaderuserid = string.Empty;//上级用户id
                    if (dt2.Rows.Count > 0)
                    {
                        leaderuserid = dt2.Rows[0]["ID"].ToString();
                        parentid = dt2.Rows[0]["LeaderUserName"].ToString();
                    }
                    int tempcode = Convert.ToInt32(model.Phone.Substring(7, 4));
                    string usercode = getusebycode(tempcode);
                    model.usercode = usercode;
                    return mdal.SaveRegister(model, leaderuserid, parentid, "");
                }
            }
            catch (Exception)
            {
                return "Fail";
            }
        }

        public string getusebycode(int tempcode)
        {
            DataTable dt = mdal.GetUserByUserCode(tempcode.ToString());
            int usercount = dt.Rows.Count;
            string returncode = string.Empty;
            if (usercount > 0)
            {
                getusebycode(tempcode + 1);
            }
            else
            {
                returncode = tempcode.ToString();
            }
            return returncode;
        }
        //根据用户邀请码获取用户信息
        public DataTable GetUserByUserCode(string usercode)
        {
            return mdal.GetUserByUserCode(usercode);
        }
        //微信或者qq用户注册登录
        public string WxOrQQSaveRegister(cp_userview model, string type)
        {
            try
            {
                DataTable dt = new DataTable();
                if (type == "0")
                {
                    dt = mdal.GetUserDt(model.wxopenid, "wxopenid");
                    return AddUserInfo(dt, model, type);
                }
                else if (type == "1")
                {
                    dt = mdal.GetUserDt(model.qqopenid, "qqopenid");
                    return AddUserInfo(dt, model, type);
                }
                else
                {
                    return "Fail";
                }
            }
            catch (Exception ex)
            {
                return "Fail";
            }
        }
        private string AddUserInfo(DataTable dt, cp_userview model, string type)
        {
            if (dt.Rows.Count > 0)
            {
                return "AlreadyExist";
            }
            else
            {
                string parentid = string.Empty;//上上级用户id
                string leaderuserid = string.Empty;//上级用户id
                return mdal.SaveRegister(model, leaderuserid, parentid, type);
            }
        }
        /// <summary>
        /// 微信或者qq登录
        /// </summary>
        /// <param name="openid">微信或者QQopenid</param>
        /// <param name="type">登录类型 0：微信登录 1：qq登录</param>
        /// <returns></returns>
        public string WxOrQQLoginBll(string openid, string type)
        {
            try
            {
                DataTable dt = new DataTable();
                if (type == "0")
                {
                    dt = mdal.GetUserDt(openid, "wxopenid");
                }
                else
                {
                    dt = mdal.GetUserDt(openid, "qqopenid");
                }
                if (dt.Rows.Count > 0)
                {
                    string returns = string.Format(@"Success,{0},{1},{2},{3}", dt.Rows[0]["ID"].ToString(), dt.Rows[0]["UserName"].ToString(), dt.Rows[0]["HeadImage"], dt.Rows[0]["Phone"]);
                    return returns;
                }
                else
                {
                    return "Fail";
                }
            }
            catch (Exception)
            {
                return "Fail";
            }
        }
        //登录
        public string LoginBll(string account, string userpwd)
        {
            try
            {
                //检测用户手机号是否被注册
                DataTable usdt1 = mdal.GetUserDt(account, "phone");
                if (usdt1.Rows.Count <= 0)
                {
                    return "NoFind";
                }
                else
                {
                    //根据手机号和密码查询用户信息
                    DataTable usdt = mdal.LgoinDal(account, userpwd);
                    if (usdt.Rows.Count > 0)
                    {
                        string returns = string.Format(@"Success,{0},{1},{2}", usdt.Rows[0]["ID"].ToString(), usdt.Rows[0]["UserName"].ToString(), usdt.Rows[0]["HeadImage"]);
                        return returns;
                    }
                    else
                    {
                        return "Fail";
                    }
                }
            }
            catch (Exception)
            {
                return "Fail";
            }
        }
        //短信登录
        public string AccountLoginBll(string account)
        {
            try
            {
                //检测用户手机号是否被注册
                DataTable usdt1 = mdal.GetUserDt(account, "phone");
                if (usdt1.Rows.Count <= 0)
                {
                    return "NoFind";
                }
                else
                {
                    //根据手机号和密码查询用户信息
                    DataTable usdt = mdal.AccountLgoinDal(account);
                    if (usdt.Rows.Count > 0)
                    {
                        string returns = string.Format(@"Success,{0},{1},{2}", usdt.Rows[0]["ID"].ToString(), usdt.Rows[0]["UserName"].ToString(), usdt.Rows[0]["HeadImage"]);
                        return returns;
                    }
                    else
                    {
                        return "Fail";
                    }
                }
            }
            catch (Exception)
            {
                return "Fail";
            }
        }
        //修改密码
        public string UpdateUserPwd(string phone, string newpwd)
        {
            try
            {
                //检测用户手机号是否被注册
                DataTable usdt1 = mdal.GetUserDt(phone, "phone");
                if (usdt1.Rows.Count <= 0)
                {
                    return "NoFind";
                }
                else
                {
                    //根据手机号和密码查询用户信息
                    string returnstr = mdal.UpdateUserPwd(phone, newpwd);
                    if (returnstr == "Success")
                    {
                        return "Success";
                    }
                    else
                    {
                        return "Fail";
                    }
                }
            }
            catch (Exception)
            {
                return "Fail";
            }
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="filestr">字段名</param>
        /// <param name="values">字段值</param>
        /// <param name="usid">用户id</param>
        /// <returns></returns>
        public string UpdateUserInfobll(string filestr, string values, string usid)
        {
            return mdal.UpdateUserInfodal(filestr, values, usid);
        }
        //绑定手机号
        public string binduserphone(string phone, string usid)
        {
            int tempcode = Convert.ToInt32(phone.Substring(7, 4));
            string usercode = getusebycode(tempcode);
            return mdal.BindUserPhone(phone, usercode, usid);
        }
        /// <summary>
        /// 根据用户id获取用户信息
        /// </summary>
        /// <param name="usid"></param>
        /// <returns></returns>
        public DataTable GetUserInfo(string usid)
        {
            DataTable usdt = mdal.GetUserDt(usid, "ID");
            return usdt;
        }
        /// <summary>
        /// 获取用户积分收入明细
        /// </summary>
        /// <param name="userid">我的用户id</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页获取数量</param>
        /// <returns>用户信息datatable集合</returns>
        public DataTable GetUserIntegralDetail(string userid, int pageindex, int pagesize)
        {
            return mdal.GetUserIntegralDetail(userid, pageindex, pagesize);
        }

        //根据要查询的字段查询用户信息
        public DataTable GetUserInfoByFile(string filename, string value)
        {
            DataTable usdt = mdal.GetUserDt(value, filename);
            return usdt;
        }

        //新增公司信息
        public bool EditCompanyInfoBll(cp_Company model, Operation operation)
        {
            return mdal.EditCompanyInfoDal(model, operation);
        }
        /// <summary>
        /// 获取公司列表信息
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public DataSet GetCompanyList(string userid, string filter, int? page = 1, int? pageSize = 20)
        {
            DataSet dtset = new DataSet();
            try
            {
                dtset = mdal.GetCompanyList(userid, filter, page, pageSize);
                return dtset;
            }
            catch (Exception ex)
            {
                return dtset;
            }
        }
        //根据id获取公司信息
        public cp_Company GetCompanyById(string id)
        {
            return mdal.GetCompanyById(id);
        }

        /// <summary>
        /// 获取收货地址列表信息
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public DataSet GetDeliveryAddressList(string userid, string filter, int? page = 1, int? pageSize = 20)
        {
            return mdal.GetDeliveryAddressList(userid, filter, page, pageSize);
        }
        //编辑收货地址信息
        public bool EditDeliveryAddressInfoBll(DeliveryAddress model, Operation operation)
        {
            return mdal.EditDeliveryAddressInfoDal(model, operation);
        }
        //根据id获取收货地址
        public DeliveryAddress GetDeliveryAddressById(string id)
        {
            return mdal.GetDeliveryAddressById(id);
        }
        //删除收货地址信息
        public bool DeleteCommon(string id, string tbname)
        {
            return mdal.DeleteCommon(id, tbname);
        }

        #region 我的物性



        #region 收藏数据

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool AddPhysics_Collection(Physics_CollectionModel model, ref string errMsg)
        {
            return mdal.AddPhysics_Collection(model, ref errMsg);
        }

        /// <summary>
        /// 获取我的收藏
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SmallClassID"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagecout"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<Physics_CollectionModel> GetPhysics_Collection(string userId, string SmallClassID, int pageindex, int pagesize, ref int pagecout, ref string errMsg)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var dt = mdal.GetPhysics_Collection(userId, SmallClassID, pageindex, pagesize, ref pagecout, ref errMsg);
                return PlasCommon.ToolClass<Physics_CollectionModel>.ConvertDataTableToModel(dt);
            }
            return null;
        }

        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="CollID">批量删除ID</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool RomvePhysics_Collection(List<string> CollID, ref string errMsg)
        {
            return mdal.RomvePhysics_Collection(CollID, ref errMsg);
        }


        /// <summary>
        /// 获取类别
        /// </summary>
        /// <param name="userId">用户信息</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public List<ProductAttr> getProductAttr(string userId, ref string errMsg)
        {
            var attr = new List<ProductAttr>();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var dt = mdal.getProductAttr(userId, ref errMsg);
                var list = PlasCommon.ToolClass<ProductAttr>.ConvertDataTableToModel(dt);
                if (list != null)
                {
                    attr = list;
                }
                return attr;
            }
            return attr;
        }
        #endregion



        #region 浏览数据


        /// <summary>
        /// 删除浏览数据
        /// </summary>
        /// <param name="browsId">浏览编号</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool RomvePhysics_Browse(List<string> browsId, ref string errMsg)
        {
            return mdal.RomvePhysics_Browse(browsId, ref errMsg);
        }

        /// <summary>
        /// 浏览分页查询
        /// </summary>
        public List<Physics_BrowseModel> GetPhysics_Browse(string userId, int pageindex, int pagesize, ref int pagecout, ref string errMsg, string wheresql = "")
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var dt = mdal.GetPhysics_Browse(userId, pageindex, pagesize, ref pagecout, ref errMsg, wheresql);
                return PlasCommon.ToolClass<Physics_BrowseModel>.ConvertDataTableToModel(dt);
            }
            return null;
        }
        //获取我的查看物性数量
        public DataTable GetMyLookMaterial(string userid)
        {
            return mdal.GetMyLookMaterial(userid);
        }

        /// <summary>
        /// 添加浏览
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool AddPhysics_Browse(Physics_BrowseModel model, ref string errMsg)
        {
            return mdal.AddPhysics_Browse(model, ref errMsg);
        }
        #endregion

        //增加用户操作积分奖励
        public void AddOperationPay(string type, string userid)
        {
            mdal.AddOperationPay(type, userid);
        }
        //获取任务完成次数
        public int GetTaskNumber(string userid, string type)
        {
            return mdal.GetTaskNumber(userid, type);
        }

        #region 添加对比
        /// <summary>
        /// 添加对比
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<PlasModel.Physics_ContrastModel> GetPhysics_Contrast(string userId, ref string errMsg)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var dt = mdal.GetPhysics_Contrast(userId, ref errMsg);
                var list = PlasCommon.ToolClass<PlasModel.Physics_ContrastModel>.ConvertDataTableToModel(dt);
                return list;
            }
            return null;
        }
        //手机端获取订阅的行情列表
        public DataTable AppGetPhyices_Contrast(string userId, int pageindex, int pagesize)
        {
            return mdal.AppGetPhyices_Contrast(userId, pageindex, pagesize);
        }

        /// <summary>
        /// 添加对比
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool AddPhysics_Contrast(Physics_ContrastModel model, ref string errMsg)
        {
            return mdal.AddPhysics_Contrast(model, ref errMsg);
        }

        /// <summary>
        /// 删除对比
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool RomvePhysics_Contrast(Physics_ContrastModel model, ref string errMsg)
        {
            return mdal.RomvePhysics_Contrast(model, ref errMsg);
        }

        /// <summary>
        /// 删除对比
        /// </summary>
        /// <param name="browsId">对比编号</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool RomvePhysics_Contrast(List<string> browsId, ref string errMsg)
        {
            return mdal.RomvePhysics_Contrast(browsId, ref errMsg);
        }

        #endregion



        #region 行情数据

        /// <summary>
        /// 添加行情
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool AddPhysics_Quotation(Physics_QuotationModel model, ref string errMsg)
        {
            return mdal.AddPhysics_Quotation(model, ref errMsg);
        }

        /// <summary>
        /// 获取我的行情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagecout"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<Physics_QuotationModel> GetPhysics_Quotation(string userId, int pageindex, int pagesize, ref int pagecout, ref string errMsg)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var dt = mdal.GetPhysics_Quotation(userId, pageindex, pagesize, ref pagecout, ref errMsg);
                return PlasCommon.ToolClass<Physics_QuotationModel>.ConvertDataTableToModel(dt);
            }
            return null;
        }
        //手机端获取订阅的行情列表
        public DataTable AppGetPhyices_Quotation(string userId, int pageindex, int pagesize)
        {
            return mdal.AppGetPhyices_Quotation(userId, pageindex, pagesize);
        }
        /// <summary>
        /// 删除行情
        /// </summary>
        /// <param name="CollID">批量删除ID</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool RomvePhysics_Quotation(List<string> CollID, ref string errMsg)
        {
            return mdal.RomvePhysics_Quotation(CollID, ref errMsg);
        }



        #endregion


        #endregion

        /// <summary>
        /// 获取我的下级用户
        /// </summary>
        /// <param name="userid">我的用户id</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页获取数量</param>
        /// <returns>用户信息datatable集合</returns>
        public DataTable GetMyUserInfo(string userid, int pageindex, int pagesize)
        {
            return mdal.GetMyUserInfo(userid, pageindex, pagesize);
        }
    }
}
