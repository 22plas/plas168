﻿using PlasDal;
using PlasModel;
using System;
using System.Collections.Generic;
using System.Data;
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
                    string parentid = string.Empty;//上上级用户id
                    string leaderuserid = string.Empty;//上级用户id
                    if (!string.IsNullOrWhiteSpace(model.RecommendPhone))
                    {
                        DataTable ldt = mdal.GetUserDt(model.RecommendPhone, "phone");
                        //如果有介绍人
                        if (ldt.Rows.Count > 0)
                        {
                            leaderuserid = ldt.Rows[0]["ID"].ToString();//上级用户id
                            parentid = ldt.Rows[0]["LeaderUserName"].ToString();//上上级用户id
                            //如果上上级用户id不为空
                            if (!string.IsNullOrWhiteSpace(parentid))
                            {
                                DataTable parentdt = mdal.GetUserDt(parentid, "ID");
                                //根据上上级用户ID检验上上级用户是否存在
                                if (parentdt.Rows.Count <= 0)
                                {
                                    parentid = string.Empty;
                                }
                            }
                        }
                    }
                    return mdal.SaveRegister(model, leaderuserid, parentid);
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
            return mdal.UpdateUserInfodal(filestr, values,usid);
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
        public bool DeleteCommon(string id,string tbname)
        {
            return mdal.DeleteCommon(id, tbname);
        }
    }
}
