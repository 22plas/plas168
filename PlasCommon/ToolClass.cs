using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace PlasCommon
{
    public class ToolClass<T> where T : new()
    {

        /// <summary>
        /// 解析为list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ListToJson(IList<T> obj)
        {
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
                string szJson = string.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    json.WriteObject(ms, obj);
                    szJson = Encoding.UTF8.GetString(ms.ToArray());
                }
                return szJson;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Table 转换成List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> ConvertDataTableToModel(DataTable dt)
        {
            // 定义集合    
            IList<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

    }
}
