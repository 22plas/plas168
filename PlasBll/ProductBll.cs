﻿using PlasCommon;
using PlasDal;
using PlasModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
    public class ProductBll
    {

        ProductDal dal = new ProductDal();
        //产品详情
        public DataSet GetModelInfo(string pguid,string userid,string ipaddress)
        {
            return dal.GetModelInfo(pguid, userid, ipaddress);
        }
        public DataSet NewGetModelInfo(string pguid,string userid)
        {
            return dal.NewGetModelInfo(pguid, userid);
        }

        /// <summary>
        /// 热门产品
        /// </summary>
        /// <param name="showNum"></param>
        /// <returns></returns>
        public DataTable HotProducts(int showNum = 5)
        {
            return dal.HotProducts(showNum);
        }

        /// <summary>
        /// 感兴趣产品和同厂家产品
        /// </summary>
        /// <returns></returns>
        public DataSet GetCompanyAndLiveProduct(int topnum, string productID)
        {
            return dal.GetCompanyAndLiveProduct(topnum, productID);
        }


        #region 超级搜索用到类

        /// <summary>
        /// 数据库中根据中英文根据首字母排序
        /// </summary>
        /// <param name="type"></param>
        /// <param name="showNum"></param>
        /// <returns></returns>
        public DataTable GetSearchParam(int type, string wherekey, int showNum = 10, int? pageindex = 1)
        {
            return dal.GetSearchParam(type, wherekey, showNum, pageindex.Value);
        }

        public DataSet Sys_GetSearchParam(string tableName, int pageSize = 10, string filterColumns = "", string whereStr = "", string orderBy = "weight desc", int pageIndex = 1)
        {
            return dal.Sys_GetSearchParam(tableName, pageSize, filterColumns, whereStr, orderBy, pageIndex);
        }

        public DataSet Sys_GetSmallClassList(int pageSize = 10, int pageIndex = 1)
        {
            return dal.Sys_GetSmallClassList(pageSize, pageIndex);
        }
        //属性值
        public DataSet Sys_GetSuperSearchParam(int parentID = -1)
        {
            return dal.Sys_GetSuperSearchParam(parentID);
        }
        //属性值新方法
        public DataSet Sys_GetSuperSearchParamnew(int parentID = -1)
        {
            return dal.Sys_GetSuperSearchParamnew(parentID);
        }
        //app获取搜索属性值
        public DataTable Sys_GetSuperSearchParamForApp(string type, string keyname)
        {
            return dal.Sys_GetSuperSearchParamForApp(type, keyname);
        }

        //超级搜索页面加载首次加载数据方法
        public DataTable SuperSearchOneLoad()
        {
            return dal.SuperSearchOneLoad();
        }
        //超级搜索
        public DataSet Sys_SuperSearch(string searchStr = "", int languageid = 2052, int pageCount = 0, int? pageSize = 10, string guidstr = "", string isNavLink = "")
        {
            return dal.Sys_SuperSearch(searchStr, languageid, pageCount, pageSize, guidstr, isNavLink);
        }

        /// <summary>
        /// 获取单位面积
        /// </summary>
        /// <param name="bigClassName">大类编号</param>
        /// <param name="samllClassName">小类编号0</param>
        /// <returns></returns>
        public List<PlasModel.unitModels> GetUnitModels(string bigClassName, string samllClassName)
        {
            return dal.GetUnitModels(bigClassName, samllClassName);
        }
        #endregion

        //普通搜索
        public DataSet GetGeneralSearch(string key = "", int pageIndex = 1, int pageSize = 20, string strGuid = "", int? isapp = 0, int? searchtype = 1)
        {
            return dal.GetGeneralSearch(key, pageIndex, pageSize, strGuid, isapp, searchtype);
        }
        //二次检索
        public DataSet GetTwoSearch(int pageIndex, int pageSize, string ver, string Characteristic, string Used, string Kind, string Method, string Factory, string Additive, string AddingMaterial, string addghdq)
        {
            return dal.GetTwoSearch(pageIndex, pageSize, ver, Characteristic, Used, Kind, Method, Factory, Additive, AddingMaterial, addghdq);
        }


        #region 价格趋势

        public DataSet getPriceFile(string SmallClass, string Manufacturer, string Model, int pageindex = 1, int pagesize = 8)
        {
            return dal.getPriceFile(SmallClass, Manufacturer, Model, pageindex, pagesize);
        }

        public DataTable GetPriceLineDt(string strwhere)
        {
            return dal.GetPriceLineDt(strwhere);
        }

        /// <summary>
        /// 最近方法查询价格报价信息表
        /// </summary>
        /// <returns></returns>
        public DataTable GetPriceNewList(Dictionary<string, string> dic, ref string errMsg)
        {
            return dal.GetPriceNewList(dic, ref errMsg);
        }

        //价格趋势分类列表
        public DataSet GetPriceType(int numtop = 10)
        {
            return dal.GetPriceType(numtop);
        }
        //头部一级条件
        public DataTable GetPriceParentParm()
        {
            return dal.GetPriceParentParm();
        }
        //获取头部二级条件
        public DataTable GetPriceTopChildParm(string parentname)
        {
            return dal.GetPriceTopChildParm(parentname);
        }
        //价格趋势分类\厂家列表
        public DataSet GetPagePriceTypeOrFactory(string key, string type, int? pageindex = 1, int? pagesize = 10)
        {
            return dal.GetPagePriceTypeOrFactory(key, type, pageindex, pagesize);
        }
        //获取产品所属厂家等信息
        public DataTable GetProductFactoryInfo(string pid)
        {
            return dal.GetProductFactoryInfo(pid);
        }
        #endregion

        #region 点击次数累增
        public void ProductHit(string proid)
        {
            dal.ProductHit(proid);
        }
        #endregion


        //查询产品下面
        public DataTable GetProductPdf(string proguid)
        {
            return dal.GetProductPdf(proguid);
        }

        //获取产品信息
        public DataTable GetProductMessage(string guid)
        {
            return dal.GetProductMessage(guid);
        }

        /// <summary>
        /// 获取产品特性
        /// </summary>
        /// <param name="ver">版本号</param>
        /// <param name="tx">特性名称</param>
        /// <returns></returns>
        public DataTable GetProductAttribute(string ver, string tx)
        {
            return dal.GetProductAttribute(ver, tx);
        }
        public DataTable GetClass(string parentid, string middlename, string type)
        {
            return dal.GetClass(parentid, middlename, type);
        }
        //最新读取分类方法(采用缓存机制)
        public List<parminfo> GetClassNew(string parentid, string middlename, string type)
        {
            string tempname = string.Empty;
            //如果是读取二级分类则传一级分类的id作为缓存值标识
            if (type=="1")
            {
                tempname = parentid;
            }
            //如果是读取三级级分类则传二级分类的名称作为缓存值标识
            else if (type=="2")
            {
                tempname = middlename;
            }
            //直接读取一级分类
            else
            {
                tempname = "onelevel";
            }
            var cache=CacheHelper.GetCache(tempname);//先读取
            if (cache == null)//如果没有该缓存
            {
                var dt = dal.GetClass(parentid, middlename, type);
                var enumerable = ToolClass<parminfo>.ConvertDataTableToModel(dt);
                CacheHelper.SetCache(tempname, enumerable);//添加缓存
                return enumerable;
            }
            else
            {
                var result = (List<parminfo>)cache;//有就直接返回该缓存
                return result;
            }            
        }

        //获取超级搜索填料属性
        public DataTable GetSysfiller(string type, string parentname, int pageindex = 1, int pagesize = 8)
        {
            return dal.GetSysfiller(type, parentname, pageindex, pagesize);
        }

        /// <summary>
        /// 获取关键词
        /// </summary>
        /// <param name="productGuid"></param>
        /// <returns></returns>
        public DataTable GetPorcutKeyWord(string productGuid)
        {
            return dal.GetPorcutKeyWord(productGuid);
        }

        /// <summary>
        /// 获取价格
        /// </summary>
        /// <param name="ProductGuid"></param>
        /// <returns></returns>
        public List<Pri_DayAvgPriceModel> GetPri_DayAvgPrice(string ProductGuid)
        {
            var list = new List<Pri_DayAvgPriceModel>();
            if (!string.IsNullOrWhiteSpace(ProductGuid))
            {
                var query = dal.GetPri_DayAvgPrice(ProductGuid);
                if (query != null)
                {
                    list = PlasCommon.ToolClass<Pri_DayAvgPriceModel>.ConvertDataTableToModel(query);
                }
            }
            return list;
        }
        /// <summary>
        /// 根据厂家名称获取对应的厂家id
        /// </summary>
        /// <param name="name">厂家名称</param>
        /// <returns></returns>
        public DataTable AppGetFactoryByName(string name)
        {
            return dal.AppGetFactoryByName(name);
        }
        #region 产品UL


        /// <summary>
        /// Ul头部
        /// </summary>
        /// <param name="FileNumber"></param>
        /// <returns></returns>
        public List<Ul_HeadModel> GetUl_Head(string ProductId)
        {
            var list = new List<Ul_HeadModel>();
            if (!string.IsNullOrWhiteSpace(ProductId))
            {
                var query = dal.GetUl_Head(ProductId);
                if (query != null && query.Rows.Count > 0)
                {
                    list = PlasCommon.ToolClass<Ul_HeadModel>.ConvertDataTableToModel(query);
                }
            }
            return list;
        }

        /// <summary>
        ///根据 编号查询
        /// </summary>
        /// <param name="FileNumber"></param>
        /// <returns></returns>
        public List<Ul_HeadModel> GetUl_HeadNumber(string NumberId)
        {
            var list = new List<Ul_HeadModel>();
            if (!string.IsNullOrWhiteSpace(NumberId))
            {
                var query = dal.GetUl_HeadNumber(NumberId);
                if (query != null && query.Rows.Count > 0)
                {
                    list = PlasCommon.ToolClass<Ul_HeadModel>.ConvertDataTableToModel(query);
                }
            }
            return list;
        }


        /// <summary>
        /// Ul详情
        /// </summary>
        /// <param name="NumberId"></param>
        /// <returns></returns>
        public List<Ul_bodyModel> GetUl_body(string NumberId)
        {
            var list = new List<Ul_bodyModel>();
            if (!string.IsNullOrWhiteSpace(NumberId))
            {
                var query = dal.GetUl_body(NumberId);
                if (query != null && query.Rows.Count > 0)
                {
                    list = PlasCommon.ToolClass<Ul_bodyModel>.ConvertDataTableToModel(query);
                }
            }
            return list;
        }


        #endregion

        /// <summary>
        /// 获取产品助剂列表
        /// </summary>
        /// <param name="name">厂家名称</param>
        /// <returns></returns>
        public DataTable GetAnnotationList(int pagesize, int pageindex, string key, int? type = 0)
        {
            return dal.GetAnnotationList(pagesize, pageindex, key, type);
        }
        /// <summary>
        /// 获取助剂类别或者厂家
        /// </summary>
        /// <param name="typestr">获取类别</param>
        /// <param name="key">关键词</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页数量</param>
        /// <returns></returns>
        public DataTable GetAnnotationClassOrFactory(string typestr, string key, int? pageindex = 1, int? pagesize = 10)
        {
            return dal.GetAnnotationClassOrFactory(typestr, key, pageindex, pagesize);
        }
        /// <summary>
        /// 获取助剂详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataSet GetAnnotationDetail(int id)
        {
            return dal.GetAnnotationDetail(id);
        }

        //获取厂家
        public DataTable GetManufacturer()
        {
            return dal.GetManufacturer();
        }
        //获取轮播pdf数据
        public DataTable GetListPDF(int ? pagesize=30)
        {
            return dal.GetListPDF(pagesize);
        }
        /// <summary>
        /// 获取产品信息
        /// </summary>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public DataTable GetProductList(int? pagesize = 10)
        {
            return dal.GetProductList(pagesize);
        }

        #region 获取对比项

        /// <summary>
        /// 获取对比项
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<Physics_ContrastModel> getContrastList(string userId,ref int contrasts, ref string errMsg)
        {
            return dal.getContrastList(userId, ref contrasts, ref errMsg);
        }

        /// <summary>
        /// 删除对比项
        /// </summary>
        /// <param name="physics"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool deleteContrastList(List<Physics_ContrastModel> physics, ref string errMsg)
        {
            return dal.deleteContrastList(physics, ref errMsg);
        }

        #endregion

    }
}
