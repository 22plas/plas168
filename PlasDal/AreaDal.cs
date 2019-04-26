using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
   public class AreaDal
    {
        //获取区域信息
        public List<string> pliststrdal(string parentname, string level)
        {
            List<string> returnlist = new List<string>();
            try
            {
                string tempstr = "";
                if (string.IsNullOrWhiteSpace(parentname))
                {
                    tempstr = "IS NULL";
                }
                else
                {
                    string templevel = string.Empty;
                    if (!string.IsNullOrWhiteSpace(level))
                    {
                        templevel = string.Format(@"and level={0}", level);
                    }
                    string getparentsql = string.Format(@"select * from area where Name='{0}' {1}", parentname, templevel);
                    DataTable parentdt = SqlHelper.GetSqlDataTable(getparentsql);
                    if (parentdt.Rows.Count > 0)
                    {
                        tempstr = string.Format(@"={0}", parentdt.Rows[0]["AreaID"]);
                    }
                    else
                    {
                        tempstr = string.Format(@"={0}", "0");
                    }
                }
                string sql = string.Format(@"SELECT * FROM area WHERE ParentID {0}", tempstr);
                DataTable areadt = SqlHelper.GetSqlDataTable(sql);
                if (areadt.Rows.Count>0)
                {
                    for (int i = 0; i < areadt.Rows.Count; i++)
                    {
                        returnlist.Add(areadt.Rows[i]["Name"].ToString());
                    }
                }
                return returnlist;
            }
            catch (Exception)
            {
                return returnlist;
            }
        }
    }
}
