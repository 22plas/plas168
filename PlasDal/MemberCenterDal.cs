﻿using PlasCommon;
using PlasCommon.SqlCommonQuery;
using PlasModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlasCommon.Enums;

namespace PlasDal
{
   public class MemberCenterDal
    {
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="parmstr">参数值</param>
        /// <param name="fieldname">字段值</param>
        /// <returns></returns>
        public DataTable GetUserDt(string parmstr,string fieldname)
        {
            string execsql = string.Format(@"select * from cp_user where {0} ='{1}'", fieldname, parmstr);
            DataTable dt = SqlHelper.GetSqlDataTable(execsql);
            return dt;
        }
        /// <summary>
        /// 保存用户注册信息
        /// </summary>
        /// <param name="model">用户信息实体</param>
        /// <param name="leaderuserid">上级用户id</param>
        /// <param name="parentid">上上级用户id</param>
        /// <returns></returns>
        public string SaveRegister(cp_userview model, string leaderuserid, string parentid)
        {
            string guid = Guid.NewGuid().ToString();
            string addsql = string.Format(@"INSERT dbo.cp_user
                ( UserName ,UserPwd ,Email ,Phone ,Address ,TestQQ ,CreateDate ,states ,ErrorDate ,ErrorCount ,WeChat ,ContentAddress ,LeaderUserName,HeadImage,InTotal,OutTotal,Balance,ID)
                VALUES  ( '{0}' ,'{1}' ,'{2}' ,'{3}' ,'{4}' ,N'' ,GETDATE() ,0 ,null ,0 ,N'' ,N'' ,'{5}','',100,0,100,'{6}')", model.UserName, ToolHelper.MD5_SET(model.UserPwd), string.Empty, model.Phone, string.Empty, leaderuserid, guid);

            SqlCommand sqlcmd = new SqlCommand();
            SqlConnection con = new SqlConnection(DataBase.ConnectionString);
            con.Open();
            SqlTransaction tra = con.BeginTransaction();
            sqlcmd.Connection = con;
            sqlcmd.Transaction = tra;
            int intype = UserInType.AddUser.GetHashCode();
            try
            {
                //保存注册用户信息
                sqlcmd.CommandText = addsql;
                sqlcmd.ExecuteNonQuery();
                //保存注册用户的积分流水信息
                sqlcmd.CommandText = string.Format(@"INSERT INTO dbo.Sys_UserPayDetails( UserId, InPay, InType, FromUserId )
                            VALUES  ( '{0}', {1}, {2},'{3}');", guid, 100, intype, "0");
                sqlcmd.ExecuteNonQuery();
                if (!string.IsNullOrWhiteSpace(leaderuserid))
                {
                    //计算修改上级用户的积分
                    sqlcmd.CommandText = string.Format(@"UPDATE dbo.cp_user SET InTotal=InTotal+50,Balance=Balance+50 WHERE ID='{0}'", leaderuserid);
                    sqlcmd.ExecuteNonQuery();
                    //保存上级用户积分流水信息
                    sqlcmd.CommandText = string.Format(@"INSERT INTO dbo.Sys_UserPayDetails( UserId, InPay, InType, FromUserId )
                            VALUES  ( '{0}', {1}, {2},'{3}');", leaderuserid, 50, intype, guid);
                    sqlcmd.ExecuteNonQuery();
                }
                if (!string.IsNullOrWhiteSpace(parentid))
                {
                    //计算修改上上级用户的积分
                    sqlcmd.CommandText = string.Format(@"UPDATE dbo.cp_user SET InTotal=InTotal+30,Balance=Balance+30 WHERE ID='{0}'", parentid);
                    sqlcmd.ExecuteNonQuery();
                    //保存上上级用户积分流水信息
                    sqlcmd.CommandText = string.Format(@"INSERT INTO dbo.Sys_UserPayDetails( UserId, InPay, InType, FromUserId )
                            VALUES  ( '{0}', {1}, {2},'{3}');", parentid, 30, intype, guid);
                    sqlcmd.ExecuteNonQuery();
                }
                tra.Commit();//提交
                return "Success";
            }
            catch (Exception ex)
            {
                tra.Rollback();//遇到错误，回滚
                return "Fail";
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        /// <summary>
        /// 执行登录方法
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="userpwd">密码</param>
        /// <returns></returns>
        public DataTable LgoinDal(string account, string userpwd)
        {
            string temppwd = ToolHelper.MD5_SET(userpwd);
            string execsql2 = string.Format(@"select * from cp_user where phone ='{0}' and UserPwd='{1}'", account, temppwd);
            DataTable usdt = SqlHelper.GetSqlDataTable(execsql2);
            return usdt;
        }
        //新增/修改公司信息
        public bool EditCompanyInfoDal(cp_Company model, Operation operation)
        {
            string sql = "";
            //新增
            if (operation == Operation.Add)
            {
                sql = string.Format(@"INSERT INTO dbo.cp_Company( Name ,Trade ,Province ,City ,Address ,Contacts ,Tel ,Fax ,QQ ,logo ,UserId ,WeChat ,Mobile ,Area ,Email ,AccountOpenBank ,AccountOpening ,
                                      OpenBank ,CreateDate ,isdefault ,TaxId,image)
                            VALUES  ( N'{0}' ,N'{1}' , N'{2}' ,N'{3}' , N'{4}' ,N'{5}' ,N'{6}' ,N'{7}' ,N'{8}' ,'{9}' ,'{10}' ,N'{11}' ,N'{12}' ,N'{13}' ,N'{14}' ,N'{15}' ,N'{16}' ,N'{17}' , '{18}',{19} ,'{20}','{21}')",
                            model.Name, string.Empty, model.Province, model.City, model.Address, model.Contacts, model.Tel, model.Fax, string.Empty, model.logo,
                                    model.UserId, model.WeChat, model.Mobile, model.Area, model.Email, model.AccountOpenBank, model.AccountOpening, model.OpenBank, DateTime.Now.ToString(), YesOrNo.No.GetHashCode(), model.TaxId,model.image);
            }
            //修改
            else
            {
                sql = string.Format(@"update dbo.cp_Company set Name='{0}' ,Trade='{1}' ,Province='{2}' ,City='{3}' ,Address='{4}' ,Contacts='{5}' ,Tel='{6}' ,Fax='{7}' ,QQ='{8}' ,logo='{9}' ,UserId='{10}' ,
                                    WeChat='{11}' ,Mobile='{12}' ,Area='{13}' ,Email='{14}' ,AccountOpenBank='{15}' ,AccountOpening='{16}' ,OpenBank='{17}' ,CreateDate='{18}' ,isdefault={19} ,TaxId='{20}',image='{21}' where Id='{22}'",
                           model.Name, string.Empty, model.Province, model.City, model.Address, model.Contacts, model.Tel, model.Fax, string.Empty, model.logo,
                                   model.UserId, model.WeChat, model.Mobile, model.Area, model.Email, model.AccountOpenBank, model.AccountOpening, model.OpenBank, DateTime.Now.ToString(), model.isdefault, model.TaxId, model.image, model.Id);
            }
            int rows = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql, null);
            return rows > 0 ? true : false;
        }
    }
}
