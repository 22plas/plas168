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
    public class ProductDal
    {

        #region 产品详情：查询型号名称及物性
        public DataSet GetModelInfo(string pguid)
        {
            var ds = SqlHelper.GetSqlDataSet(string.Format(@"exec PROC_GetInfoByPguid '{0}' ", pguid));
            return ds;
        }
        public DataSet NewGetModelInfo(string pguid)
        {
            var ds = SqlHelper.GetSqlDataSet(string.Format(@"exec PROC_GetInfoByPguid '{0}' ", pguid));
            //var ds = SqlHelper.GetSqlDataSet(string.Format(@"exec readproduct '{0}' ", pguid));
            return ds;
        }
        #endregion


        #region 热门产品
        /// <summary>
        /// 按点击次数，获取指定数量热门产品，默认5条
        /// </summary>
        /// <param name="showNum"></param>
        /// <returns></returns>
        public DataTable HotProducts(int showNum = 5)
        {

            var ds = SqlHelper.GetSqlDataTable(string.Format(@"select a.*,(select top 1 ProductID from dbo.ProductLevel1 b where a.SmallClassId=b.SmallClassID) as ProductID from
  (SELECT TOP {0} BigClassId,SmallClassId,ProModel,PlaceOrigin,ProductGuid,HitCount,Brand,Edition
  FROM dbo.Product where isShow=1 order by HitCount desc) a ", showNum));
            return ds;

        }
        #endregion


        #region 感兴趣的产品和同一个厂家产出的产品

        public DataSet GetCompanyAndLiveProduct(int topnum, string productID)
        {
            //---同一个厂家
            //---感兴趣的产品，同类产品
            string sql = string.Format(@"select top {0} a.ProModel,a.ProductGuid from Product as a 
                inner join Product as b on a.PlaceOrigin=b.PlaceOrigin
                where b.productguid='{1}'
                order by NewID()
                select top {2} a.ProModel,a.ProductGuid from Product as a 
                inner join Product as b on a.SmallClassID=b.SmallClassID
                where b.productguid='{3}'
                order by NewID()", topnum, productID, topnum, productID);
            var ds = SqlHelper.GetSqlDataSet(sql.ToString());
            return ds;
        }

        #endregion


        #region 超级搜索查询数据
        /// <summary>
        /// 数据库中根据中英文根据首字母排序
        /// </summary>
        /// <param name="type"></param>
        /// <param name="showNum"></param>
        /// <returns></returns>
        public DataTable GetSearchParam(int type, string wherekey, int showNum = 10, int? pageindex = 1)
        {
            string tempwherekey = string.Empty;
            var ds = new DataSet();
            switch (type)
            {
                case 1://产品种类
                    ds = Sys_GetSmallClassList(showNum, pageindex.Value);
                    break;
                case 2://特性
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and name like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Sys_character", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,*,'''' as SmallGuid", " and name<>'''' and name<>''--''" + tempwherekey, "weight desc,  substring(dbo.[fn_ChineseToSpell](name),1,1),name ", pageindex.Value);
                    break;
                case 3://阻燃等级
                    ds = Sys_GetSearchParam("Prd_ZRDJSort", showNum, "  substring(dbo.[fn_ChineseToSpell](KeyWord),1,1) as fw,KeyWord as Name,*,'''' as SmallGuid ", "", " value desc ,substring(dbo.[fn_ChineseToSpell](KeyWord),1,1),KeyWord ", pageindex.Value);
                    break;
                case 4://厂家
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and AliasName like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Sys_Manufacturer", showNum, "  substring(dbo.[fn_ChineseToSpell](AliasName),1,1) as fw,AliasName as Name,*,[Guid] as SmallGuid", " and AliasName<>'''' and AliasName<>''--''" + tempwherekey, "weight desc, substring(dbo.[fn_ChineseToSpell](AliasName),1,1),AliasName ", pageindex.Value);
                    break;
                case 5://加工方法
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and name like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Sys_Method", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,*,'''' as SmallGuid,'''' as AliasName ", " and name<>'''' and name<>''--''" + tempwherekey, "[weight] desc, substring(dbo.[fn_ChineseToSpell](name),1,1),name ", pageindex.Value);
                    break;
                case 6://安全级别(阻燃等级的二级属性)
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and name like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Prd_SmallClass", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,*,'''' as SmallGuid,'''' as AliasName ", " and name<>'''' and name<>''--''" + tempwherekey, "[weight] desc, substring(dbo.[fn_ChineseToSpell](name),1,1),name  ", pageindex.Value);//Weigth字段加在了Prd_SmallClass_l表
                    break;
                case 7://用途
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and name like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Sys_ForUse", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,*,'''' as SmallGuid ", " and name<>'''' and name<>''--''" + tempwherekey, "[weight] desc,substring(dbo.[fn_ChineseToSpell](name),1,1),name  ", pageindex.Value);
                    break;
                case 8://填料
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and name like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Sys_filler", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,*,'''' as SmallGuid ", " and name<>'''' and name<>''--''" + tempwherekey, "[weight] desc,substring(dbo.[fn_ChineseToSpell](name),1,1),name ", pageindex.Value);
                    break;
                case 9://添加剂
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and name like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Sys_Additive", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,*,'''' as SmallGuid,'''' as AliasName ", " and name<>'''' and name<>''--''" + tempwherekey, "[weight] desc,substring(dbo.[fn_ChineseToSpell](name),1,1),name ", pageindex.Value);
                    break;
                case 11://app高级搜类别
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " aliasname LIKE''" + wherekey.Trim() + "'' OR name like ''%" + wherekey.Trim() + "%''";
                    ds = Sys_SuperSearchGetSmallClassList(showNum, pageindex.Value, tempwherekey);
                    break;
                case 111://app助剂类别
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " aliasname LIKE''" + wherekey.Trim() + "'' OR name like ''%" + wherekey.Trim() + "%''";
                    ds = Sys_SuperSearchGetSmallClassList(showNum, pageindex.Value, tempwherekey);
                    break;
                default:
                    tempwherekey = string.IsNullOrWhiteSpace(wherekey) ? "" : " and name like ''%" + wherekey + "%''";
                    ds = Sys_GetSearchParam("Prd_SmallClass", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ,'''' as SmallGuid,'''' as AliasName ", " and name<>'''' and name<>''--''" + tempwherekey, "[weight] desc,substring(dbo.[fn_ChineseToSpell](name),1,1),name ", pageindex.Value);
                    break;
            }
            return ds.Tables[0];
        }

        public DataSet Sys_GetSearchParam(string tableName, int pageSize = 10, string filterColumns = "", string whereStr = "", string orderBy = "weight desc", int pageIndex = 1)
        {
            string sql = string.Format("exec pr_common_page '{0}','{1}','{2}','{3}',{4},{5}", tableName, filterColumns, whereStr, orderBy, pageIndex, pageSize);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        public DataSet Sys_GetSmallClassList(int pageSize = 10, int pageIndex = 1)
        {
            string sql = string.Format("exec PROC_GetSmallClassList {0},{1}", pageIndex, pageSize);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }
        //超级搜索获取产品类别
        public DataSet Sys_SuperSearchGetSmallClassList(int pageSize, int pageIndex, string key)
        {
            string sql = string.Format("exec PROC_SuperSearchGetSmallClassList {0},{1},'{2}'", pageIndex, pageSize, key);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }
        /// <summary>
        /// 获取产品类别
        /// </summary>
        /// <param name="parentid">上级分类id</param>
        /// <param name="type">获取类型 0：获取一级分类  1：获取二级分类 2：获取三级分类</param>
        /// <param name="middlename">二级分类名称</param>
        /// <returns></returns>
        public DataTable GetClass(string parentid, string middlename, string type)
        {
            string sql = "";
            if (type == "0")
            {
                sql = string.Format("select Name,parentguid from Prd_BigClass_l");
            }
            else if (type == "1")
            {
                sql = string.Format(@"select distinct a.MidClassName as Name,'' as parentguid from Prd_SmallClass_l a inner join Prd_SmallClass b on a.parentguid=b.guid 
                                      inner join Prd_BigClass c on c.guid = b.parentguid where c.guid = '{0}' and a.LanguageId = 2052 order by a.MidClassName", parentid);
            }
            else
            {
                sql = string.Format(@"select Name,parentguid as parentguid from Prd_SmallClass_l where MidClassName='{0}' and LanguageId=2052 order by Name", middlename);
            }
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
        //属性值
        public DataSet Sys_GetSuperSearchParam(int parentID = -1)
        {
            var ds = new DataSet();
            string sql = string.Format("exec proc_supersearch_getParam {0}", parentID);
            var dt = SqlHelper.GetSqlDataTable(sql);
            #region 旧处理方式
            //dt.TableName = "dt1n";
            //DataTable tblDatas = new DataTable("dt2n");
            //DataColumn dc = null;
            //dc = tblDatas.Columns.Add("id", Type.GetType("System.String"));
            //dc = tblDatas.Columns.Add("Name", Type.GetType("System.String"));
            //DataRow newRow;
            //if (dt.Rows.Count > 0)
            //{
            //    for (var i = 0; i < dt.Rows.Count; i++)
            //    {
            //        if (!string.IsNullOrWhiteSpace(dt.Rows[i]["RealKey"].ToString()))
            //        {
            //            string rsql = string.Format("select distinct unit as Name from ProductAttribute where RealKey in (select * from f_split('{0}',';'))", dt.Rows[i]["RealKey"].ToString().Replace("'",""));
            //            var ts = SqlHelper.GetSqlDataTable(rsql);
            //            if (ts.Rows.Count > 0)
            //            {
            //                for (var j = 0; j < ts.Rows.Count; j++)
            //                {
            //                    newRow = tblDatas.NewRow();
            //                    newRow["id"] = dt.Rows[i]["id"].ToString();
            //                    newRow["Name"] = ts.Rows[j]["Name"].ToString();
            //                    tblDatas.Rows.Add(newRow);
            //                }
            //            }

            //        }
            //    }
            //}
            #endregion
            dt.TableName = "dt1n";
            ds.Tables.Add(dt.Copy());
           // ds.Tables.Add(tblDatas);
            return ds;
        }
        //app获取搜索属性值
        public DataTable Sys_GetSuperSearchParamForApp(string type, string keyname)
        {
            //string sql = string.Format("exec proc_supersearch_getParam_forapp {0}", parentID);
            //DataTable dt = SqlHelper.GetSqlDataTable(sql);
            string sql = string.Format(@"SELECT * FROM V_RealKey");
            //一级
            if (type == "0")
            {
                sql = "SELECT DISTINCT parentkey,parentweight FROM V_RealKey WHERE parentkey<>'总体' ORDER BY parentweight ASC";
            }
            //二级
            else if (type == "1")
            {
                sql = string.Format("SELECT DISTINCT realkeygroup AS FaceKey,weight FROM V_RealKey WHERE parentkey='{0}' ORDER BY weight desc", keyname);
            }
            //三级
            else if (type == "2")
            {
                sql = string.Format("SELECT  FaceKey,RealKey,Guid,UnitFaceKey FROM V_RealKey WHERE realkeygroup='{0}'", keyname);
            }
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }


        /// <summary>
        /// 获取单位面积
        /// </summary>
        /// <param name="bigClassName">大类编号</param>
        /// <param name="samllClassName">小类编号0</param>
        /// <returns></returns>
        public List<PlasModel.unitModels> GetUnitModels(string bigClassName, string samllClassName)
        {
            string sql = string.Format(@"select '' as unit union select unit from ProductUnit where attribute1='{0}' and realkey='{1}' and unit<>'' and unit is not null order by unit", bigClassName, samllClassName);
            var models = new List<PlasModel.unitModels>();
            var dt = SqlHelper.GetSqlDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                models = PlasCommon.ToolClass<PlasModel.unitModels>.ConvertDataTableToModel(dt);
            }
            return models;
        }


        //超级搜索
        public DataSet Sys_SuperSearch(string searchStr = "", int languageid = 2052, int pageCount = 0, int pageSize = 20, string guidstr = "", string isNavLink = "")
        {
            //{产品种类=>''尼龙 6 弹性体;MMS;EA'',0,0}{产品特性=>''Broad Seal Range ;半结晶'',0,0}	
            // { 机械性能 =)拉伸模量 => '''',0,10600}
            TaskFactory taskfactory = new TaskFactory();
            List<Task> taskList = new List<Task>();
            //为每个条件开启一个线程
            int taskts = searchStr.Length - searchStr.Replace("}", "").Length;//Environment.ProcessorCount
            string ver = guidstr;//Guid.NewGuid().ToString();
            if (string.IsNullOrWhiteSpace(isNavLink) && isNavLink != "1")//分页的时候，不执行数据查询
            {
                for (int i = 1; i <= taskts; i++)
                {
                    //产生一个查询条件
                    string onesql = searchStr.Substring(1, searchStr.IndexOf("}") - 1);
                    string splitsql = onesql;
                    if (onesql.IndexOf("=)") > -1)
                    {
                        splitsql = "{" + onesql + "}";
                    }
                    //一个线程执行一个查询条件
                    string execsql = string.Format("exec suppersearchone '{0}','{1}',{2},{3}", ver, splitsql, languageid, i);
                    taskList.Add(taskfactory.StartNew(() =>
                    {
                        SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, execsql, null);
                    }));
                    searchStr = searchStr.Replace(string.Format("{0}{1}{2}", "{", onesql, "}"), "");
                }
            }
            //等待所有线程执行完成
            Task.WaitAll(taskList.ToArray());
            //所有线程执行完成后再将全部结果求交集
            string sql = string.Format("exec suppersearchmerge '{0}',{1},{2},{3}", ver, taskts, pageCount, pageSize);
            PlasCommon.Common.AddLog("system", "执行超级搜索", sql.ToString(), "");
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        /// <summary>
        /// 获取关键词
        /// </summary>
        /// <param name="productGuid"></param>
        /// <returns></returns>
        public DataTable GetPorcutKeyWord(string productGuid)
        {
            string sql = string.Format(@"select * from Product a 
                                        left join sys_Manufacturer b on a.PlaceOrigin=b.EnglishName
                                        where a.ProductGuid='{0}'", productGuid);
            return SqlHelper.GetSqlDataTable(sql.ToString());

        }

        #endregion

        #region 普通搜索

        /// <summary>
        /// DataTable dt2 = dv.ToTable(true, "name,age,hobby");去重
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filedNames"></param>
        /// <returns></returns>
        public static DataTable Distinct(DataTable dt, string[] filedNames)
        {
            DataView dv = dt.DefaultView;
            DataTable DistTable = dv.ToTable("Dist", true, filedNames);
            return DistTable;
        }

        //普通搜索
        public DataSet GetGeneralSearch(string key = "", int pageIndex = 1, int pageSize = 20, string strGuid = "", int? isapp = 0)
        {
            string sql = string.Format("exec headsearch '{0}','{1}',{2},{3},{4},{5}", strGuid, key, 2052, pageIndex, pageSize, isapp);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        //       @pageIndex int,
        //@pageSize int,
        //@ver varchar(100),
        //@Characteristic varchar(300),--特性
        //@Used varchar(300),--用途
        //@Kind varchar(300),--种类
        //@Method varchar(300),--方法
        //@Factory varchar(300),--厂家
        //@Additive varchar(300),--添加剂
        //@AddingMaterial varchar(300)--增料
        public DataSet GetTwoSearch(int pageIndex, int pageSize, string ver, string Characteristic, string Used, string Kind, string Method, string Factory, string Additive, string AddingMaterial,string addghdq)
        {
            string sql = string.Format("exec Get_TwoHeadSearchTmpData {0},{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}'", pageIndex, pageSize, ver, Characteristic, Used, Kind, Method, Factory, Additive, AddingMaterial, addghdq);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        #endregion

        #region 价格趋势

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSet getPriceFile(string SmallClass, string Manufacturer, string Model, int pageindex = 1, int pagesize = 8)
        {
            string sql = string.Format("exec QueryPriceProc {0},{1},'{2}','{3}','{4}',''", pageindex, pagesize, SmallClass, Manufacturer, Model);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        /// <summary>
        /// 查询价格信息
        /// </summary>
        /// <param name="strwhere"></param>
        /// <returns></returns>
        public DataTable GetPriceLineDt(string strwhere)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select PriDate,Price from Pri_DayAvgPrice where 1=1");
            if (!string.IsNullOrWhiteSpace(strwhere))
            {
                sql.Append(" " + strwhere);
            }
            sql.Append(" order by PriDate");
            var dt = SqlHelper.GetSqlDataTable(sql.ToString());
            return dt;
        }

        //获取分类
        public DataSet GetPriceType(int numtop = 10)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            if (numtop > 0)
            {
                sql.Append(" top " + numtop + " ");
            }
            sql.Append("SmallClass as Name from Pri_DayAvgPrice group by SmallClass ");
            sql.Append("select ");
            if (numtop > 0)
            {
                sql.Append(" top " + numtop + " ");
            }
            sql.Append("Manufacturer as Name from Pri_DayAvgPrice group by Manufacturer ");
            var ds = SqlHelper.GetSqlDataSet(sql.ToString());
            return ds;
        }

        //获取行情走势中的分类\厂家
        public DataSet GetPagePriceTypeOrFactory(string key, string type, int? pageindex = 1, int? pagesize = 10)
        {
            string sql = string.Format("exec QueryPriceProcClassAndFactory {0},{1},'{2}',{3}", pageindex.Value, pagesize.Value, key, Convert.ToInt32(type));
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }
        //获取产品所属厂家等信息
        public DataTable GetProductFactoryInfo(string pid)
        {
            string sql = string.Format(@"select b.AliasName,c.Name,b.EnglishName from Product a inner join Sys_Manufacturer b on a.PlaceOrigin=b.EnglishName inner join Prd_SmallClass_l c on a.SmallClassId=c.parentguid
                                        where a.ProductGuid = '{0}' and c.LanguageId = 2052", pid);
            var dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }

        #endregion

        #region 点击次数累增
        /// <summary>
        /// 产品点击数次累加
        /// </summary>
        /// <param name="proid"></param>
        public void ProductHit(string proid)
        {
            string sql = "update Product set HitCount=isnull(HitCount,0)+1 where ProductGuid=@ProductGuid";
            SqlParameter[] parm = {
                new SqlParameter("@ProductGuid",proid)
            };
            SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, sql, parm);
        }
        #endregion

        #region 产品属性

        /// <summary>
        /// 查询产品下面的PDF
        /// </summary>
        /// <param name="proguid"></param>
        /// <returns></returns>
        public DataTable GetProductPdf(string proguid)
        {
            DataTable dt = new DataTable();
            string sql = string.Format(@"select a.*,b.* from Product_TestPdf as a 
                                        left join Product_TypePdf as b on a.testtype = b.id
                                        where a.ProductGuid = '{0}'", proguid);
            dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }


        //获取产品信息
        public DataTable GetProductMessage(string guid)
        {
            string sql = string.Format("select * from product where ProductGuid='{0}'", guid);
            var dt = SqlHelper.GetSqlDataTable(sql.ToString());
            return dt;
        }
        /// <summary>
        /// 获取产品特性
        /// </summary>
        /// <param name="ver">版本号</param>
        /// <param name="tx">特性名称</param>
        /// <returns></returns>
        public DataTable GetProductAttribute(string ver, string tx)
        {
            string sql = string.Format("exec headsearch_app '{0}','{1}'", ver, tx);
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }
        /// <summary>
        /// 获取超级搜索属性填料
        /// </summary>
        /// <param name="parentname">上级填料名称</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        public DataTable GetSysfiller(string type, string parentname, int pageindex = 1, int pagesize = 8)
        {
            string sql = "";
            int startpage = (pageindex - 1) * pagesize + 1;
            int endpage = pageindex * pagesize;
            if (type=="0")
            {
                //sql = string.Format("SELECT b.Tl AS Name FROM(SELECT row_number() over(order by Tl desc)as rownum,a.Tl FROM(SELECT DISTINCT Tl FROM Sys_filler) a) b WHERE b.rownum BETWEEN {0} AND {1}", startpage, endpage);
                sql = string.Format("exec proc_supersearch_getfiller {0},{1},'{2}'", startpage, endpage, parentname);
            }
            else
            {
                sql = string.Format("SELECT Name FROM dbo.Sys_filler WHERE Tl='{0}' ORDER BY Weight DESC", parentname);
            }
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            return dt;
        }



        /// <summary>
        /// 获取价格
        /// </summary>
        /// <param name="ProductGuid"></param>
        /// <returns></returns>
        public DataTable GetPri_DayAvgPrice(string ProductGuid)
        {
            string sql = "select * from Pri_DayAvgPrice where PriceProductGuid=@ProductGuid order by priDate";
            SqlParameter[] parm = { new SqlParameter("@ProductGuid", ProductGuid) };
            return SqlHelper.GetSqlDataTable_Param(sql, parm);
        }

        /// <summary>
        /// 根据厂家名称获取对应的厂家id
        /// </summary>
        /// <param name="name">厂家名称</param>
        /// <returns></returns>
        public DataTable AppGetFactoryByName(string name)
        {
            string sql = string.Format(@"select a.AliasName,a.Guid from Sys_Manufacturer a where '{0}' in (select * from f_split(a.AliasName,';'))", name);
            return SqlHelper.GetSqlDataTable(sql);
        }
        #endregion

        #region 产品UL

        /// <summary>
        /// Ul头部
        /// </summary>
        /// <param name="FileNumber"></param>
        /// <returns></returns>
        public DataTable GetUl_Head(string ProductId)
        {
            string sql = string.Format(@"select b.* from Ul_Product a 
                                        left join Ul_Head b on a.FileNumber=b.FileNumber
                                        where a.ProductId=@ProductId
                                        order by b.[Category] , b.[FileNumber] ,b.[Manufactory],   b.[Field8] ,b.[Serires]   ,b.[Field10] , b.[Field11]
                                       ");
            SqlParameter[] parm = { new SqlParameter("@ProductId", ProductId) };
            return SqlHelper.GetSqlDataTable_Param(sql.ToString(), parm);
        }

        /// <summary>
        /// 根据编号
        /// </summary>
        /// <param name="NumberId"></param>
        /// <returns></returns>
        public DataTable GetUl_HeadNumber(string NumberId)
        {
            string sql = string.Format("select top 1* from Ul_Head where FileNumber=@NumberId");
            SqlParameter[] parm = { new SqlParameter("@NumberId", NumberId) };
            DataTable dt = SqlHelper.GetSqlDataTable_Param(sql.ToString(), parm);
            return dt;
        }

        /// <summary>
        /// 获取Ul头部
        /// </summary>
        /// <param name="NumberId"></param>
        /// <returns></returns>
        public DataTable GetUl_body(string ProductId)
        {
            string sql = string.Format(@"select * from dbo.UL_Body where FileNumber=@ProductId");
            SqlParameter[] parm = { new SqlParameter("@ProductId", ProductId) };
            return SqlHelper.GetSqlDataTable_Param(sql.ToString(), parm);
        }




        #endregion

        //获取产品助剂列表
        public DataTable GetAnnotationList(int pagesize, int pageindex, string key,int? type=0)
        {
            string sql = string.Format("exec Fi_FillerList {0},{1},{2},{3},{4},'{5}'", pagesize, pageindex, 2052, 1, type,key);
            return SqlHelper.GetSqlDataTable(sql);
        }
        /// <summary>
        /// 获取助剂类别或者厂家
        /// </summary>
        /// <param name="typestr">获取类别</param>
        /// <param name="key">关键词</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns>返回助剂类别或者厂家</returns>
        public DataTable GetAnnotationClassOrFactory(string typestr, string key, int? pageindex = 1, int? pagesize = 10)
        {
            string sql = string.Format(@"exec Fi_FillerProductTypeOrFactory {0},{1},{2},{3},'{4}'", pagesize, pageindex,2052, typestr, key);
            return SqlHelper.GetSqlDataTable(sql);
        }

        //获取助剂详情
        public DataSet GetAnnotationDetail(int id)
        {
            //根据id查询产品助剂热度
            string gethitsql =string.Format("select * from Fi_FillterHitCount where ParentId={0}",id);
            DataTable hitdt = SqlHelper.GetSqlDataTable(gethitsql);
            int hitcount = 0;
            if (hitdt.Rows.Count > 0)
            {
                hitcount = Convert.ToInt32(hitdt.Rows[0]["HitCount"]) + 1;
                string updatehitsql = string.Format(@"update Fi_FillterHitCount set HitCount={0} where ParentId={1} ", hitcount, id);
                SqlHelper.ExecuteSqlNoQuery(updatehitsql);
            }
            string sql = string.Format("EXEC Fi_ReadHead {0},{1}", id, 2052);
            string sql2 = string.Format(@"SELECT a.Property,a.ValueUnit,a.TestType,a.TestCondition FROM dbo.Fi_Body a 
                                            INNER JOIN dbo.Fi_Fillter b ON a.Parentid=b.id
                                            INNER JOIN dbo.Fi_Fillter_l c ON c.ParentId=a.Parentid
                                            WHERE c.languageid={1} AND a.Parentid={0}", id, 2052);
            DataTable dt2 = SqlHelper.GetSqlDataTable(sql2);
            DataTable dt = SqlHelper.GetSqlDataTable(sql);
            dt.TableName = "dt";
            dt2.TableName = "dt2";
            DataSet dset = new DataSet();
            dset.Tables.Add(dt.Copy());
            dset.Tables.Add(dt2.Copy());
            return dset;
        }
    }
}
