using PlasCommon;
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
        #region 罗志强--会员中心模块
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="parmstr">参数值</param>
        /// <param name="fieldname">字段值</param>
        /// <returns></returns>
        public DataTable GetUserDt(string parmstr, string fieldname)
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
        public string SaveRegister(cp_userview model, string leaderuserid, string parentid,string type)
        {
            string guid = Guid.NewGuid().ToString();
            string addsql = "";
            if (type == "0")
            {
                addsql = string.Format(@"INSERT dbo.cp_user
                ( UserName ,UserPwd ,Email ,Phone ,Address ,TestQQ ,CreateDate ,states ,ErrorDate ,ErrorCount ,WeChat ,ContentAddress ,LeaderUserName,HeadImage,InTotal,OutTotal,Balance,ID,wxopenid)
                VALUES  ( '{0}' ,'{1}' ,'{2}' ,'{3}' ,'{4}' ,N'' ,GETDATE() ,0 ,null ,0 ,N'' ,N'' ,'{5}','{8}',100,0,100,'{6}','{7}')", model.UserName, "", string.Empty, "", string.Empty, leaderuserid, guid,model.wxopenid,model.HeadImage);
            }
            else if (type == "1")
            {
                addsql = string.Format(@"INSERT dbo.cp_user
                ( UserName ,UserPwd ,Email ,Phone ,Address ,TestQQ ,CreateDate ,states ,ErrorDate ,ErrorCount ,WeChat ,ContentAddress ,LeaderUserName,HeadImage,InTotal,OutTotal,Balance,ID,qqopenid)
                VALUES  ( '{0}' ,'{1}' ,'{2}' ,'{3}' ,'{4}' ,N'' ,GETDATE() ,0 ,null ,0 ,N'' ,N'' ,'{5}','{8}',100,0,100,'{6}','{7}')", model.UserName, "", string.Empty,"", string.Empty, leaderuserid, guid, model.wxopenid, model.HeadImage);
            }
            else
            {
                addsql = string.Format(@"INSERT dbo.cp_user
                ( UserName ,UserPwd ,Email ,Phone ,Address ,TestQQ ,CreateDate ,states ,ErrorDate ,ErrorCount ,WeChat ,ContentAddress ,LeaderUserName,HeadImage,InTotal,OutTotal,Balance,ID)
                VALUES  ( '{0}' ,'{1}' ,'{2}' ,'{3}' ,'{4}' ,N'' ,GETDATE() ,0 ,null ,0 ,N'' ,N'' ,'{5}','',100,0,100,'{6}')", model.UserName, ToolHelper.MD5_SET(model.UserPwd), string.Empty, model.Phone, string.Empty, leaderuserid, guid);
            }

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
        /// 修改用户信息
        /// </summary>
        /// <param name="filestr">字段名</param>
        /// <param name="values">值</param>
        /// <param name="usid">用户id</param>
        /// <returns></returns>
        public string UpdateUserInfodal(string filestr, string values, string usid)
        {
            try
            {
                string sqlstr = string.Format(@"update cp_user set {0}='{1}' where ID='{2}'", filestr, values, usid);
                int row = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sqlstr, null);
                return "Success";
            }
            catch (Exception)
            {
                return "Fail";
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
        /// <summary>
        /// 执行登录方法
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns></returns>
        public DataTable AccountLgoinDal(string account)
        {
            string execsql2 = string.Format(@"select * from cp_user where phone ='{0}'", account);
            DataTable usdt = SqlHelper.GetSqlDataTable(execsql2);
            return usdt;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string UpdateUserPwd(string phone, string newpwd)
        {
            try
            {
                string temppwd = ToolHelper.MD5_SET(newpwd);
                string execsql2 = string.Format(@"UPDATE dbo.cp_user SET UserPwd='{0}' WHERE Phone='{1}'", temppwd, phone);
                int row = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, execsql2, null);
                return "Success";
            }
            catch (Exception)
            {
                return "Fail";
            }
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
            DataSet returnds = new DataSet();
            try
            {
                string tempstr = string.IsNullOrWhiteSpace(filter) == false ? string.Format(@" and g.Name like '%{0}%'", filter) : "";
                int starpagesize = page.Value * pageSize.Value - pageSize.Value;
                int endpagesize = page.Value * pageSize.Value;
                //拼接参数
                //SqlParameter[] parameters = {
                //                new SqlParameter("@userid", SqlDbType.NVarChar),
                //                new SqlParameter("@starpagesize", SqlDbType.Int),
                //                new SqlParameter("@endpagesize", SqlDbType.Int),
                //                new SqlParameter("@wherestr",SqlDbType.NVarChar)
                //            };
                //parameters[0].Value = userid;
                //parameters[1].Value = starpagesize;
                //parameters[2].Value = endpagesize;
                //parameters[3].Value = tempstr;
                string execsql2 = string.Format(@"SELECT * FROM (
                                                SELECT CAST(ROW_NUMBER() over(order by COUNT(g.Id) DESC) AS INTEGER) AS Ornumber,g.Name,g.Province,g.City,g.Area,g.[Address],g.Contacts,g.Tel,g.Fax,g.[image],g.WeChat,
                                                g.Mobile,g.Email,g.AccountOpenBank,g.AccountOpening,g.OpenBank,g.TaxId,g.Id
                                                FROM dbo.cp_Company g WHERE g.UserId='{0}' {1}
                                                GROUP BY g.Name,g.Province,g.City,g.Area,g.[Address],g.Contacts,g.Tel,g.Fax,g.[image],g.WeChat,
                                                g.Mobile,g.Email,g.AccountOpenBank,g.AccountOpening,g.OpenBank,g.TaxId,g.Id) t WHERE t.Ornumber > {2} AND t.Ornumber<={3}", userid, tempstr, starpagesize, endpagesize);
                execsql2 = execsql2 + string.Format(@" SELECT COUNT(*) as totals FROM dbo.cp_Company g WHERE g.UserId='{0}' {1}", userid, tempstr);
                //DataTable returndt = SqlHelper.GetSqlDataTable_Param(execsql2, parameters);

                //returnds.Tables.Add(returndt);
                returnds = SqlHelper.GetSqlDataSet(execsql2);
                return returnds;
            }
            catch (Exception ex)
            {
                return returnds;
            }
        }
        /// <summary>
        /// 根据id获取公司信息
        /// </summary>
        /// <param name="Id">公司ID</param>
        /// <returns></returns>
        public cp_Company GetCompanyById(string Id)
        {
            cp_Company m = new cp_Company();
            try
            {
                string sql = string.Format(@"select * from cp_company where Id='{0}'", Id);
                DataTable dt = SqlHelper.GetSqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    m.AccountOpenBank = dt.Rows[0]["AccountOpenBank"].ToString();
                    m.AccountOpening = dt.Rows[0]["AccountOpening"].ToString();
                    m.Address = dt.Rows[0]["Address"].ToString();
                    m.Area = dt.Rows[0]["Area"].ToString();
                    m.City = dt.Rows[0]["City"].ToString();
                    m.Contacts = dt.Rows[0]["Contacts"].ToString();
                    m.Email = dt.Rows[0]["Email"].ToString();
                    m.Fax = dt.Rows[0]["Fax"].ToString();
                    m.image = dt.Rows[0]["image"].ToString();
                    m.Mobile = dt.Rows[0]["Mobile"].ToString();
                    m.Name = dt.Rows[0]["Name"].ToString();
                    m.OpenBank = dt.Rows[0]["OpenBank"].ToString();
                    m.Province = dt.Rows[0]["Province"].ToString();
                    m.TaxId = dt.Rows[0]["TaxId"].ToString();
                    m.Tel = dt.Rows[0]["Tel"].ToString();
                    m.WeChat = dt.Rows[0]["WeChat"].ToString();
                    m.Id = dt.Rows[0]["Id"].ToString();
                }
                return m;
            }
            catch (Exception)
            {
                return m;
            }
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
                                    model.UserId, model.WeChat, model.Mobile, model.Area, model.Email, model.AccountOpenBank, model.AccountOpening, model.OpenBank, DateTime.Now.ToString(), YesOrNo.No.GetHashCode(), model.TaxId, model.image);
            }
            //修改
            else
            {
                sql = string.Format(@"update dbo.cp_Company set Name='{0}' ,Trade='{1}' ,Province='{2}' ,City='{3}' ,Address='{4}' ,Contacts='{5}' ,Tel='{6}' ,Fax='{7}' ,QQ='{8}' ,logo='{9}' ,UserId='{10}' ,
                                    WeChat='{11}' ,Mobile='{12}' ,Area='{13}' ,Email='{14}' ,AccountOpenBank='{15}' ,AccountOpening='{16}' ,OpenBank='{17}' ,CreateDate='{18}' ,isdefault={19} ,TaxId='{20}',image='{21}' where Id='{22}'",
                           model.Name, string.Empty, model.Province, model.City, model.Address, model.Contacts, model.Tel, model.Fax, string.Empty, model.logo,
                                   model.UserId, model.WeChat, model.Mobile, model.Area, model.Email, model.AccountOpenBank, model.AccountOpening, model.OpenBank, DateTime.Now.ToString(), model.isdefault.GetHashCode(), model.TaxId, model.image, model.Id);
            }
            int rows = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql, null);
            return rows > 0 ? true : false;
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
            DataSet returnds = new DataSet();
            try
            {
                string tempstr = string.IsNullOrWhiteSpace(filter) == false ? string.Format(@" and g.Contacts like '%{0}%'", filter) : "";
                int starpagesize = page.Value * pageSize.Value - pageSize.Value;
                int endpagesize = page.Value * pageSize.Value;
                string execsql2 = string.Format(@"SELECT * FROM (
                                                SELECT CAST(ROW_NUMBER() over(order by COUNT(g.Id) DESC) AS INTEGER) AS Ornumber,g.Id,g.Tel,g.Contacts,g.ContactsMobile,g.Province,g.City,g.[Count],g.[Address],g.IsDefault FROM dbo.cp_CompanyAddress g
                                                WHERE g.UserId='{0}' {1}
                                                GROUP BY g.Id,g.Tel,g.Contacts,g.ContactsMobile,g.Province,g.City,g.[Count],g.[Address],g.IsDefault) t WHERE t.Ornumber > {2} AND t.Ornumber<={3}", userid, tempstr, starpagesize, endpagesize);
                execsql2 = execsql2 + string.Format(@" SELECT COUNT(*) as totals FROM dbo.cp_CompanyAddress g WHERE g.UserId='{0}' {1}", userid, tempstr);
                returnds = SqlHelper.GetSqlDataSet(execsql2);
                return returnds;
            }
            catch (Exception ex)
            {
                return returnds;
            }
        }
        /// <summary>
        /// 根据id获取公司信息
        /// </summary>
        /// <param name="Id">公司ID</param>
        /// <returns></returns>
        public DeliveryAddress GetDeliveryAddressById(string Id)
        {
            DeliveryAddress m = new DeliveryAddress();
            try
            {
                string sql = string.Format(@"select * from cp_CompanyAddress where Id='{0}'", Id);
                DataTable dt = SqlHelper.GetSqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    m.Address = dt.Rows[0]["Address"].ToString();
                    m.Count = dt.Rows[0]["Count"].ToString();
                    m.City = dt.Rows[0]["City"].ToString();
                    m.Contacts = dt.Rows[0]["Contacts"].ToString();
                    m.ContactsMobile = dt.Rows[0]["ContactsMobile"].ToString();
                    m.Fax = dt.Rows[0]["Fax"].ToString();
                    m.Province = dt.Rows[0]["Province"].ToString();
                    m.Tel = dt.Rows[0]["Tel"].ToString();
                    m.WeChat = dt.Rows[0]["WeChat"].ToString();
                    m.Id = dt.Rows[0]["Id"].ToString();
                    m.IsDefault = Convert.ToInt32(dt.Rows[0]["IsDefault"]) == 0 ? false : true;
                }
                return m;
            }
            catch (Exception)
            {
                return m;
            }
        }
        //新增/修改收货地址信息
        public bool EditDeliveryAddressInfoDal(DeliveryAddress model, Operation operation)
        {
            string sql = "";
            //如果当前保存的收货地址为默认的话则修改原来的默认的收货地址为不默认
            if (model.IsDefault)
            {
                string updatesql = string.Format(@"update cp_CompanyAddress set IsDefault=0 where IsDefault=1 and UserId='{0}'", model.UserId);
                SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, updatesql, null);
            }
            //新增
            if (operation == Operation.Add)
            {
                sql = string.Format(@"INSERT INTO dbo.cp_CompanyAddress( UserId ,Tel ,Fax ,Contacts ,ContactsMobile ,QQ ,WeChat ,Province ,City ,Count ,Address ,IsDefault)
                                    VALUES  ( '{0}' ,N'{1}' ,N'{2}' ,N'{3}' ,'{4}' ,N'{5}' ,N'{6}' ,N'{7}' ,N'{8}' ,N'{9}' ,N'{10}' ,{11})",
                            model.UserId, model.Tel, model.Fax, model.Contacts, model.ContactsMobile, model.QQ, model.WeChat, model.Province, model.City, model.Count, model.Address, model.IsDefault == false ? 0 : 1);
            }
            //修改
            else
            {
                sql = string.Format(@"update dbo.cp_CompanyAddress set UserId='{0}' ,Tel='{1}' ,Fax='{2}' ,Contacts='{3}' ,ContactsMobile='{4}' ,QQ='{5}' ,WeChat='{6}' ,Province='{7}' ,City='{8}' ,Count='{9}' ,Address='{10}' ,
                                    IsDefault='{11}'  where Id='{12}'",
                           model.UserId, model.Tel, model.Fax, model.Contacts, model.ContactsMobile, model.QQ, model.WeChat, model.Province, model.City, model.Count, model.Address, model.IsDefault == false ? 0 : 1, model.Id);

            }
            int rows = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql, null);
            return rows > 0 ? true : false;
        }
        //公共删除方法
        public bool DeleteCommon(string id, string tbname)
        {
            try
            {
                string sql = string.Format(@"delete from {1} where Id='{0}'", id, tbname);
                SqlHelper.ExecuteScalar(SqlHelper.ConnectionStrings, sql, null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 黄远林--我的物性


        #region 收藏
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool AddPhysics_Collection(Physics_CollectionModel model, ref string errMsg)
        {
            bool isAdd = false;
            try
            {
                string savesql = string.Format("select id from Physics_Collection where ProductGuid='{0}' and UserId='{1}'", model.ProductGuid, model.UserId);
                var dt = SqlHelper.GetSqlDataTable(savesql.ToString());
                if (dt.Rows.Count > 0)
                {
                    errMsg = "此产品已经收藏！";
                    dt.Dispose();
                }
                else
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into Physics_Collection(ProductGuid,UserId,CreateDate)");
                    sql.Append(" values(@ProductGuid,@UserId,@CreateDate)");
                    SqlParameter[] parm = {
                   new SqlParameter("@ProductGuid",model.ProductGuid),
                   new SqlParameter("@UserId",model.UserId),
                   new SqlParameter("@CreateDate",DateTime.Now)
                    };
                    isAdd = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql.ToString(), parm) > 0;
                }

            }
            catch (Exception ex)
            {

                errMsg = ex.Message.ToString();
            }
            return isAdd;
        }


        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="CollID">批量删除ID</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public bool RomvePhysics_Collection(List<string> CollID, ref string errMsg)
        {
            bool isdel = false;
            try
            {
                if (CollID.Count > 0)
                {
                    StringBuilder sql = new StringBuilder();
                    for (var i = 0; i < CollID.Count; i++)
                    {
                        string sqlstr = string.Format(" delete from Physics_Collection where Id={0}", CollID[i]);
                        sql.Append(sqlstr);
                    }
                    isdel = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql.ToString(), null) > 0;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
            }
            return isdel;
        }

        /// <summary>
        /// 获取类别
        /// </summary>
        /// <param name="userId">用户信息</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public DataTable getProductAttr(string userId, ref string errMsg)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select b.SmallClassId as attribute,c.Name as attributevalue from Physics_Collection a");
                sql.Append(" left join product  b on a.ProductGuid=b.ProductGuid ");
                sql.Append(" left join Prd_SmallClass_l c on c.parentguid=b.SmallClassId");
                sql.Append(" where 1=1 and a.UserId='" + userId + "'");
                sql.Append(" group by b.SmallClassId,c.Name");
                return SqlHelper.GetSqlDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                errMsg = ex.Message.ToString();
            }
            return null;
        }

        /// <summary>
        /// 收藏
        /// </summary>
        public DataTable GetPhysics_Collection(string userId, string SmallClassID, int pageindex, int pagesize, ref int pagecout, ref string errMsg)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select a.Id,a.ProductGuid,a.UserId,a.CreateDate,b.ProModel,b.PlaceOrigin,c.ProUse,c.characteristic,d.Name from Physics_Collection as a ");
                sql.Append(" left join Product as b on a.ProductGuid=b.ProductGuid");
                sql.Append(" left join Product_l as c on c.ParentGuid=a.ProductGuid");
                sql.Append(" left join Prd_SmallClass_l as d on d.parentguid=b.SmallClassId");
                sql.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(SmallClassID))
                {
                    sql.Append(" and b.SmallClassId='" + SmallClassID + "'");
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    sql.Append(" and a.UserId='" + userId + "'");
                }
                var dt = GetPhysicsAttr(sql.ToString(), "id desc", pageindex, pagesize, ref pagecout, ref errMsg);
                return dt;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 浏览


        /// <summary>
        /// 添加浏览
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool AddPhysics_Browse(Physics_BrowseModel model,ref string errMsg)
        {
            bool isadd = false;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into Physics_Browse(ProductGuid,UserId,Btype,BrowsCount,CreateDate)");
                sql.Append("values(@ProductGuid,@UserId,@Btype,@BrowsCount,@CreateDate)");
                SqlParameter[] parm = {
                    new SqlParameter("@ProductGuid",model.ProductGuid),
                    new SqlParameter("@UserId",model.UserId),
                    new SqlParameter("@Btype",model.Btype),
                    new SqlParameter("@BrowsCount",model.BrowsCount),
                    new SqlParameter("@CreateDate",DateTime.Now)
                };
                isadd = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql.ToString(), parm) > 0;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
            }
            return isadd;
        }

        /// <summary>
        /// 浏览分页查询
        /// </summary>
        public DataTable GetPhysics_Browse(string userId, int pageindex, int pagesize, ref int pagecout, ref string errMsg)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select a.Id,a.ProductGuid,a.UserId,a.CreateDate,b.ProModel,b.PlaceOrigin,c.ProUse,c.characteristic,d.Name,isnull(e.Id,0) as isColl from Physics_Browse as a ");
                sql.Append(" left join Product as b on a.ProductGuid=b.ProductGuid");
                sql.Append(" left join Product_l as c on c.ParentGuid=a.ProductGuid");
                sql.Append(" left join Prd_SmallClass_l as d on d.parentguid=b.SmallClassId");
                sql.Append(" left join Physics_Collection e on a.ProductGuid=e.ProductGuid and a.UserId=e.UserId");
                sql.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(userId))
                {
                    sql.Append(" and a.UserId='" + userId + "'");
                 }
                var dt = GetPhysicsAttr(sql.ToString(), "id desc", pageindex, pagesize, ref pagecout, ref errMsg);
                return dt;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
            }
            return null;
        }

        /// <summary>
        /// 删除浏览数据
        /// </summary>
        /// <param name="browsId">浏览编号</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool RomvePhysics_Browse(List<string> browsId, ref string errMsg)
        {
            bool isdel = false;
            try
            {
                if (browsId.Count > 0)
                {
                    StringBuilder sql = new StringBuilder();
                    for (var i = 0; i < browsId.Count; i++)
                    {
                        string sqlstr = string.Format(" delete from Physics_Browse where Id={0}", browsId[i]);
                        sql.Append(sqlstr);
                    }
                    isdel = SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql.ToString(), null) > 0;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
            }
            return isdel;
        }

        #endregion

        /// <summary>
        /// 对比
        /// </summary>
        public DataTable GetPhysics_Contrast(string userId, int pageindex, int pagesize, ref int pagecout, ref string errMsg)
        {
            return null;
        }

        /// <summary>
        /// 公用方法类
        /// </summary>
        /// <returns></returns>
        public DataTable GetPhysicsAttr(string sql,string orderstr, int pageindex, int pagesize, ref int pagecout, ref string errMsg)
        {
            try
            {
                int pageBegin = 1;
                int pageEnd = 1;
                if (pageindex == 0)
                {
                    pageindex = 1;
                }
                if (pagesize == 0)
                {
                    pagesize = 1;
                }
                pageBegin = (pageindex - 1) * pagesize;
                pageEnd = (pagesize * pageindex);
                StringBuilder bigsql = new StringBuilder();
                bigsql.Append("select * from (");
                bigsql.Append(" select row_number()over(order by " + orderstr + ")as rn,* from ");
                bigsql.Append(" (" + sql + ") t");
                bigsql.Append(" ) tn");
                bigsql.Append(" where tn.rn between " + pageBegin + " and " + pageEnd);

                string sqlcout = string.Format("select count(1) as counts from ({0}) as t", sql.ToString());
                var dt = SqlHelper.GetSqlDataTable(sqlcout);
                if (dt.Rows.Count > 0)
                {
                    int.TryParse(dt.Rows[0]["counts"].ToString(), out pagecout);
                    dt.Dispose();
                }

                return SqlHelper.GetSqlDataTable(bigsql.ToString());
            }
            catch (Exception ex)
            {
                errMsg = ex.Message.ToString();
                return null;
            }
         
        }

        #endregion

    }
}
