﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlasCommon.SqlCommonQuery
{
    public class SqlHelper
    {

        public static readonly string ConnectionStrings = DataBase.ConnectionString;

        public static SqlConnection GetConnection(string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            return con;
        }



        public static int ExecuteScalar(string connectionStrings, string sql, SqlParameter[] values)
        {
            using (SqlConnection con = GetConnection(connectionStrings))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                if (values != null) cmd.Parameters.AddRange(values);
                object obj = cmd.ExecuteScalar();
                int result = Convert.ToInt32(obj);
                con.Close();
                con.Dispose();
                return result;

            }

        }


        public static int ExectueNonQuery(string connectionStrings, string sql, SqlParameter[] values)
        {
            using (SqlConnection con = GetConnection(connectionStrings))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    if (values != null)
                    {
                        foreach (SqlParameter parm in values)
                        {
                            if (parm.Value == null)
                            {
                                parm.Value = DBNull.Value;

                            }
                            cmd.Parameters.Add(parm);
                        }

                    }
                    // cmd.Parameters.AddRange(values);
                    int result = cmd.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    return result;

                }
                catch (Exception ex)
                {
                    con.Close();
                    con.Dispose();
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();

                }
            }
        }

        public static int ExectueNonQuery(SqlConnection conn, string sql, SqlParameter[] values, SqlTransaction trans)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (values != null)
            {


                cmd.Parameters.AddRange(values);
            }
            cmd.Transaction = trans;
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
            return result;

        }

        /// <summary>
        /// 查询单行单列的值
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="sql">查询语句</param>
        /// <param name="values">参数</param>
        /// <returns>对象</returns>
        public static object GetScalar(string connectionString, string sql, SqlParameter[] values)
        {
            using (SqlConnection con = GetConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                if (values != null) cmd.Parameters.AddRange(values);
                object result = cmd.ExecuteScalar();
                
                con.Close();
                con.Dispose();
                return result;
            }
        }

        public static object GetScalar(SqlConnection connection, SqlTransaction trans, string cmdText, SqlParameter[] parameter)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, trans, CommandType.Text, cmdText, parameter);
            object val = cmd.ExecuteScalar();
           
            connection.Close();
            connection.Dispose();
            return val;
        }

        public static SqlDataReader Reader(string connectionStrings, string sql, SqlParameter[] values)
        {
            SqlConnection con = GetConnection(connectionStrings);

            SqlCommand cmd = new SqlCommand(sql, con);
            if (values != null) cmd.Parameters.AddRange(values);
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            //清除参数
            cmd.Parameters.Clear();
            
            con.Close();
            con.Dispose();
            return reader;
        }

        /// <summary>
        /// 事务方式执行sql语句
        /// </summary>
        /// <param name="connection">sqlconnection对象</param>
        /// <param name="commd"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool ExecuteTransaction(SqlConnection connection, SqlCommand commd, SqlTransaction trans, string cmdText, SqlParameter[] parameters)
        {
            try
            {
                CommandType cmdType = CommandType.Text;
                PrepareCommand(commd, connection, trans, cmdType, cmdText, parameters);
                int result = commd.ExecuteNonQuery();
                if (result > 0)
                {
                    trans.Commit();
                    return true;
                }
                else
                {
                    trans.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    
                    connection.Close();
                    connection.Dispose();
                }
                  
            }

        }

        public static bool ExecuteTransaction(SqlConnection connection, SqlCommand commd, SqlTransaction trans, List<Tuple<string, SqlParameter[]>> list)
        {
            CommandType cmdType = CommandType.Text;
            int count = 0;
            try
            {
                foreach (var item in list)
                {
                    string cmdText = item.Item1;
                    var parameters = item.Item2;
                    PrepareCommand(commd, connection, trans, cmdType, cmdText, parameters);
                    count += commd.ExecuteNonQuery();
                    commd.Parameters.Clear();
                }
                if (count >= 0)
                {
                    trans.Commit();
                    return true;
                }
                else
                {
                    trans.Rollback();
                    return false;
                }

            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    
                    connection.Close();
                    connection.Dispose();
                }
            }
        }


        public static int ExecuteTrans(SqlConnection connection, SqlTransaction trans, string cmdText, SqlParameter[] parameter)
        {
            int val;
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, trans, CommandType.Text, cmdText, parameter);
                val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch
            {
                connection.Close();
                throw;
            }
            finally
            {
                
                connection.Close();
                connection.Dispose();
            }
            return val;
        }
        /// <summary>
        /// 用现有的数据库连接执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="conn">一个现有的数据库连接</param>
        /// <param name="commandType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery_Cmd(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }


        /// <summary>
        /// 用执行的数据库连接执行一个返回数据集的sql命令
        /// </summary>
        /// <remarks>
        /// 举例:  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="commandType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>包含结果的读取器</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            //创建一个SqlCommand对象
            SqlCommand cmd = new SqlCommand();
            //创建一个SqlConnection对象
            SqlConnection conn = new SqlConnection(connectionString);

            //在这里我们用一个try/catch结构执行sql文本命令/存储过程，因为如果这个方法产生一个异常我们要关闭连接，因为没有读取器存在，
            //因此commandBehaviour.CloseConnection 就不会执行
            try
            {
                //调用 PrepareCommand 方法，对 SqlCommand 对象设置参数
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                //调用 SqlCommand  的 ExecuteReader 方法
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //清除参数
                cmd.Parameters.Clear();
                
                conn.Close();
                conn.Dispose();
                return reader;
            }
            catch
            {
                //关闭连接，抛出异常
                conn.Dispose();
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 准备执行一个命令
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="conn">Sql连接</param>
        /// <param name="trans">Sql事务</param>
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param>
        /// <param name="cmdText">命令文本,例如：Select * from Products</param>
        /// <param name="cmdParms">执行命令的参数</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if (parm.Value == null)
                    {
                        parm.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parm);
                }
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, ref SqlCommand sqlCommand)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet);
                SqlParameter[] OutParameters = new SqlParameter[parameters.Length];
                sqlCommand = sqlDA.SelectCommand;
                //   connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }


        


        #region 返回DataTable
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataTable GetSqlDataTable(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    DataSet ds = new DataSet();

                    cmd.CommandTimeout = 100;
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds, "ds");
                    connection.Close();
                    connection.Dispose();
                    return ds.Tables[0];
                }

            }
        }

        /// <summary>
        /// 返回dataset
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public static DataSet GetSqlDataSet(string SQLString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings))
                {
                    using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                    {
                        DataSet ds = new DataSet();

                        cmd.CommandTimeout = 60000;
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds, "ds");
                        connection.Close();
                        connection.Dispose();
                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region 执行SQL语句 无返回值
        /// <summary>
        /// 执行SQL语句 无返回值
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static void ExecuteSqlNoQuery(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        cmd.CommandTimeout = 10;
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        connection.Dispose();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    
                    connection.Close();
                    connection.Dispose();
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }



        /// <summary>
        /// 执行存储过程 [带参数] 返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecProcSqlQuery_Param(string ProcName, SqlParameter[] parms)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = 100;
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = ProcName;
                        cmd.Parameters.AddRange(parms);  //添加参数集
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds.Tables[0];


                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        connection.Dispose();
                        throw new Exception(ex.Message);
                    }
                }
            }
        }


        #region 20200306获取list转换list
        #region 返回DataTable [带参数]
        /// <summary>
        /// 返回DataTable [带参数]
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataTable GetSqlDataTable_Param(string SQLString, SqlParameter[] param = null)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    DataSet ds = new DataSet();

                    cmd.CommandTimeout = 10;
                    connection.Open();
                    if (param != null)
                        cmd.Parameters.AddRange(param); //添加参数集
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds, "ds");
                    adapter.Dispose();
                    return ds.Tables[0];
                }

            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlString"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> GetQueryList<T>(string sqlString, SqlParameter[] param = null)
        {
            var dt = GetSqlDataTable_Param(sqlString, param);
            var list = ConvertToList<T>(dt);
            return list;
        }

        /// <summary>
        /// 获取单行列表
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T GetQuery<T>(string sql, SqlParameter[] param = null)
        {
            DataTable dt = GetSqlDataTable_Param(sql, param);
            var list = ConvertToList<T>(dt);
            if (list.Count > 0)
            {
                return list[0];
            }
            return default(T);
        }
        #endregion


        #region 返回DataTable
        /// <summary>
        /// datatable转换成List
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="table">表</param>
        /// <returns></returns>
        public static List<T> ConvertToList<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertToList<T>(rows);
        }

        public static List<T> ConvertToList<T>(List<DataRow> rows)
        {
            List<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {  //You can log something here     
                       //throw;    
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet GetDataSet(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }


        #endregion
        #endregion




    }
}
