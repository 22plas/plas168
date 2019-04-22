using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasCommon.SqlCommonQuery
{
   public class DataBase
    {

        public static string ConnectionString
        {
            get
            {

                string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ConnectionString;
                return _connectionString;
            }
        }

        /// <summary>
        /// 得到web.config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string configName)
        {
            string connectionString = ConfigurationManager.AppSettings[configName];
            return connectionString;
        }

    }
}
