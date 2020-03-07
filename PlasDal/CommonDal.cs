using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
   public class CommonDal
    {
        //获取微信/qq请求code用于匹配是否请求过
        public DataTable GetWxCodeCheck(string code, int type)
        {
            string sql = string.Format("SELECT * FROM Sys_WxCode WHERE WxCode='{0}' and Type={1}", code, type);
            return SqlHelper.GetSqlDataTable(sql);
        }
        //添加微信/qq请求code
        public void AddWxCode(string code, int type) {
            string sql = string.Format("INSERT INTO dbo.Sys_WxCode( WxCode, CreateTime,Type )VALUES  ( '{0}', GETDATE(),{1})", code,type);
            SqlHelper.ExecuteSqlNoQuery(sql);
        }
    }
}
