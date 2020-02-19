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
        //获取微信请求code用于匹配是否请求过
        public DataTable GetWxCodeCheck(string code)
        {
            string sql = string.Format("SELECT * FROM Sys_WxCode WHERE WxCode='{0}'", code);
            return SqlHelper.GetSqlDataTable(sql);
        }
        //添加微信请求code
        public void AddWxCode(string code) {
            string sql = string.Format("INSERT INTO dbo.Sys_WxCode( WxCode, CreateTime )VALUES  ( '{0}', GETDATE())", code);
            SqlHelper.ExecuteSqlNoQuery(sql);
        }
    }
}
