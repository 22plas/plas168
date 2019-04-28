using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
    public class ReplaceDal
    {

        /// <summary>
        /// 返回替换产品
        /// </summary>
        /// <param name="SourceId"></param>
        /// <param name="ver"></param>
        /// <param name="UserId"></param>
        /// <param name="WhereString"></param>
        /// <returns></returns>
        public DataSet GetReplace(string SourceId,string ver,string UserId,string WhereString)
        {
            //计算一个产品的相似度
            //string SourceId = "088CF6D2-C231-499E-AD9F-82688EB6F9D5";// "2D1636B6-9548-4790-8F14-66DCD5879F2A";// "D161293C-4A8C-4267-B38A-D19A19B89682";// "2350C07B-D47C-46FD-88FF-00A903EF4594"; //TextProductId.Text.Trim();//

            ////本次执行运算的唯一版本号
            //string ver = Guid.NewGuid().ToString();
            ////用户ID，应该从Session中取得
            //string UserId = "张三或李四";
            ////@WhereString: 要求参与相似度计算的所有属性及权重列表，这个是你前端用JS拼装出来的。
            ////下面共有4个属性参与运算，每个属性用{}分开，每个{}中的最后一个是数值，代表这个属性的权重
            //string WhereString = "{物理性能=)密度=>10}{机械性能=)伸长率=>10}{物理性能=)熔流率=>15}{可燃性=)阻燃等级=>15}";
            ////采用多少个任务来处理本次相似度运算，我们的SQL服务器有32个逻辑内核，这里采用30个任务来处理，
            ////当需要处理的目标物料较少时，由SQL存储过程会自动任务个数来保持执行效率
            int tasks = 30;//Environment.ProcessorCount;
            double bg = DateTime.Now.Ticks;
            string parm1 = string.Format("exec AlikeCountPara_User '{0}','{1}','{2}','{3}',{4}", ver, UserId, SourceId, WhereString, tasks);
            //SqlParameter[] parm1 = {
            //    new SqlParameter("@ver", ver),
            //    new SqlParameter("@UserId", UserId),
            //    new SqlParameter("@SourceId", SourceId),
            //    new SqlParameter("@WhereString", WhereString),
            //    new SqlParameter("@tasks", tasks)};
            //SqlHelper.ExecProcSqlQuery_Param("AlikeCountPara_User", parm1);
            SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings,parm1,null);

            //以上代码是执行整个运算的第一步：解析出参与相似度运算的目标物料，并根据目标物料的个数多少分配出每个任务要运算哪些目标物料。


            //以下代码是执行整个运算的第二步：由前端调用多任务来计算每个任务中列出目标物料属性的相似度得分并写入到表ProductAlikeDetails_User中

            //先找出存储过程将本次参与运算的实际任务个数
            string sql = "select max(tasks) from ProductAlikeTargetList_User where ver=@ver";
            SqlParameter[] parm2 = { new SqlParameter("@ver", ver) };
            tasks = SqlHelper.ExecuteScalar(SqlHelper.ConnectionStrings, sql, parm2);


            if (tasks > 1) //采用多任务执行 
            {
                TaskFactory taskfactory = new TaskFactory();
                List<Task> taskList = new List<Task>();
                while (tasks > 0)
                {
                    taskList.Add(taskfactory.StartNew(() =>
                    {
                        string sqlparm = string.Format("exec AlikeCount_User '{0}',{1}", ver, tasks);
                        SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sqlparm, null);
                        //SqlParameter[] parm3 = { new SqlParameter("@ver", ver), new SqlParameter("@task", tasks) };
                        //SqlHelper.ExecProcSqlQuery_Param("AlikeCountPara_User", parm3);
                    }));
                    tasks -= 1;
                }
                Task.WaitAll(taskList.ToArray());

            }
            else
            {
                //SqlParameter[] parm4 = { new SqlParameter("@ver", ver), new SqlParameter("@task", tasks) };
                //SqlHelper.ExecProcSqlQuery_Param("AlikeCountPara_User", parm4);
                string sqlparm4 = string.Format("exec AlikeCount_User '{0}',{1}", ver, tasks);
                SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sqlparm4, null);
            }
            //以上代码执行完成第二步,分多任务（目标物料较少时采用单线程执行）完成所有目标物料的相似度运算明细内容

            //以下代码实现第三步：最终相似度得分与排序，并实现分页显示 存储过程名称为：AlikeMerge_User
            int pageno = 1;  //分页显示时的页码编号
            int pagesize = 20;//分页显示时每页要求显示的记录条数
            string sqlparm5 = string.Format("exec AlikeMerge_User '{0}',{1},{2}", ver, pageno, pagesize);
            var ds = SqlHelper.GetSqlDataSet(sqlparm5);
           // SqlParameter[] parm5 = { new SqlParameter("@ver", ver), new SqlParameter("@pageno", pageno), new SqlParameter("@pagesize", pagesize) };
           // DataTable dt = SqlHelper.ExecProcSqlQuery_Param("AlikeMerge_User", parm5);
           // string sErr = string.Empty;
           // Rad.Text = string.Format("启用{0}个线程计算完成:耗时{1}秒", tasks, Convert.ToString((DateTime.Now.Ticks - bg) / 10000000.00));
            return ds;
        }

    }
}
