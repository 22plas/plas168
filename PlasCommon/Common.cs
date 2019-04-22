using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasCommon
{
    public class Common
    {
        static String product = "Dysmsapi";//短信API产品名称
        static String domain = "dysmsapi.aliyuncs.com";//短信API产品域名
        //static String accessId = "LTAIhYsAcfD8MJ5V";//Access Key ID
        //static String accessSecret = "";//Access Key Secret
        //static String regionIdForPop = "huangyuanlin";
        static string singName = ConfigurationManager.AppSettings["SignName"];//你的签名
        static string tempCode = ConfigurationManager.AppSettings["TempCode"];//你的模板编号
        static string accetKey = ConfigurationManager.AppSettings["AccetKey"];//你的key
        static string accetSerct = ConfigurationManager.AppSettings["accetserct"];//你的accetserct
        static string regionIdForPop = ConfigurationManager.AppSettings["regionIdForPop"];//"cn-hangzhou";


        #region 发送短信
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="NumPhone"></param>
        /// <param name="validateNum"></param>

        public static bool SmsSend(string NumPhone, string validateNum)
        {
            IClientProfile profile = DefaultProfile.GetProfile(regionIdForPop, accetKey, accetSerct);
            DefaultProfile.AddEndpoint(regionIdForPop, regionIdForPop, product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                request.PhoneNumbers = NumPhone;//电话号码
                request.SignName = singName;//签名
                request.TemplateCode = tempCode;// "SMS_130950020";
                request.TemplateParam = "{\"code\":\"" + validateNum + "\"}";
                request.OutId = "";//模板code中定义的参数，可以定义多个，这里我只定义了一个code；validateNum是传进来的验证码，在后端随机生成，可自行处理
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                var status = sendSmsResponse.HttpResponse.Status;
                System.Console.WriteLine(sendSmsResponse.Message);
                //成功
                if (status == 200)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (ServerException ex)
            {
                string result = ex.Message;
                return false;
            }
            catch (ClientException ez)
            {
                string result = ez.Message;
                return false;
            }
        }
        #endregion


        #region 操作方法

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="UserName">用户</param>
        /// <param name="Title">大致标题</param>
        /// <param name="Message">操作信息</param>
        /// <param name="BusData">业务数据</param>
        //public static void AddLog(string UserName, string Title, string Message, string BusData)
        //{
        //    using (PlasConfig db = new PlasConfig())
        //    {
        //        systemLog log = new systemLog();
        //        log.BusinessData = BusData;
        //        log.Createdate = DateTime.Now;
        //        log.Message = Message;
        //        log.Title = Title;
        //        log.UserName = UserName;
        //        db.systemLog.Add(log);
        //        db.SaveChanges();
        //    }
        //}


        ///// <summary>
        ///// 产品点击数次累加
        ///// </summary>
        ///// <param name="proid"></param>
        //public static void ProductHit(int proid)
        //{
        //    string sql = "update NewProduct set HitCount=isnull(HitCount,0)+1 where ID=@proid";
        //    SqlParameter[] parm = {
        //        new SqlParameter("@proid",proid)
        //    };
        //    // ExectueNonQuery(sql, parm);
        //}
        ///// <summary>
        ///// 产品类目
        ///// </summary>
        ///// <param name="ClassID"></param>
        //public static void PrdcutClassDelete(int ClassID)
        //{
        //    string sql = "delete from  ProductType where (fid=@ClassID or id=@ClassID)";
        //    SqlParameter[] parm = {
        //        new SqlParameter("@ClassID",ClassID)
        //    };
        //    //  ExectueNonQuery(sql, parm);
        //}

        ///// <summary>
        ///// 新闻点击数次累加
        ///// </summary>
        ///// <param name="newsid"></param>
        //public static void NewsHit(int newsid)
        //{
        //    string sql = "update News set HIt=isnull(HIt,0)+1 where ID=@newsid";
        //    SqlParameter[] parm = {
        //        new SqlParameter("@newsid",newsid)
        //    };
        //    //  ExectueNonQuery(sql, parm);
        //}

        #endregion


        #region 其他操作方法

        /// <summary>
        /// 是否包含数字
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        //public static bool isShow(string message)
        //{
        //    for (int i = 0; i < message.Length; i++)
        //    {
        //        if (message[i] >= 48 && message[i] <= 57)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //    //bool isNum = false;
        //    //System.Text.RegularExpressions.Regex rex =new System.Text.RegularExpressions.Regex(@"^\d+$");
        //    //if (rex.IsMatch(message))
        //    //{
        //    //    //存在数字
        //    //    isNum = true;
        //    //}
        //    //return isNum;
        //}
        #endregion


        #region 产品操作

        /// <summary>
        /// sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        //public static string ProductUpdateClassID(string sql)
        //{
        //    string sErr = string.Empty;
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(sql))
        //        {
        //            //ExectueNonQuery(sql,null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sErr = ex.Message.ToString();

        //    }

        //    return sErr;

        //}


        #endregion


        #region 获取城市列表

        //获取城市列表
        //public DataTable GetCityList(int id)
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        string sql = "select * from REGION where ID=" + id;
        //        return GetSqlDataSet(sql.ToString()).Tables[0];
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}


        #endregion

        #region 随机字符串
        /// <summary>
        /// 生成指定数量长度的随机字符串
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public static string GenerateCheckCodeNum(int codeCount)
        {
            int rep = 0;
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }
        #endregion

        #region 生成返回结果
        /// <summary>
        /// 生成返回结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToJsonResult(string state, string message, object data = null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("State", state);
            result.Add("Message", message);
            result.Add("Result", data);
            //if (data != null)
            //{
            //    foreach (var item in data.GetType().GetProperties())
            //    {
            //        result.Add(item.Name, item.GetValue(data));
            //    }
            //}

            return result;
        }
        #endregion
    }
}
