using PlasDal;
using PlasModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        string returns = string.Format(@"Success,{0},{1},{2}", usdt.Rows[0]["ID"].ToString(), usdt.Rows[0]["UserName"].ToString(),usdt.Rows[0]["HeadImage"]);
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
        /// <summary>
        /// 根据用户id获取用户信息
        /// </summary>
        /// <param name="usid"></param>
        /// <returns></returns>
        public DataTable GetUserInfo(string usid) {
            DataTable usdt = mdal.GetUserDt(usid, "ID");
            return usdt;
        }
    }
}
