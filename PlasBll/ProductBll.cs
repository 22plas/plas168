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
        public DataSet GetModelInfo(string pguid)
        {
            return dal.GetModelInfo(pguid);
        }
        public DataSet NewGetModelInfo(string pguid)
        {
            return dal.NewGetModelInfo(pguid);
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
        //app获取搜索属性值
        public DataTable Sys_GetSuperSearchParamForApp(int parentID = -1)
        {
            return dal.Sys_GetSuperSearchParamForApp(parentID);
        }

        //超级搜索
        public DataSet Sys_SuperSearch(string searchStr = "", int languageid = 2052, int pageCount = 0, int pageSize = 20, string guidstr = "", string isNavLink = "")
        {
            return dal.Sys_SuperSearch(searchStr, languageid, pageCount, pageSize, guidstr, isNavLink);
        }

        #endregion

        //普通搜索
        public DataSet GetGeneralSearch(string key = "", int pageIndex = 1, int pageSize = 20, string strGuid = "", int? isapp = 0)
        {
            return dal.GetGeneralSearch(key, pageIndex, pageSize, strGuid, isapp);
        }
        //二次检索
        public DataSet GetTwoSearch(int pageIndex, int pageSize, string ver, string Characteristic, string Used, string Kind, string Method, string Factory, string Additive, string AddingMaterial)
        {
            return dal.GetTwoSearch(pageIndex, pageSize, ver, Characteristic, Used, Kind, Method, Factory, Additive, AddingMaterial);
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

        //价格趋势分类列表
        public DataSet GetPriceType(int numtop = 10)
        {
            return dal.GetPriceType(numtop);
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

        //获取超级搜索填料属性
        public DataTable GetSysfiller(string parentname, int pageindex = 1, int pagesize = 8)
        {
            return dal.GetSysfiller(parentname, pageindex, pagesize);
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
                    list= PlasCommon.ToolClass<Pri_DayAvgPriceModel>.ConvertDataTableToModel(query);
                }
            }
            return list;
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
                if (query != null)
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
        public List<Ul_bodyModel> GetUl_body(string ProductId)
        {
            var list = new List<Ul_bodyModel>();
            if (!string.IsNullOrWhiteSpace(ProductId))
            {
                var query = dal.GetUl_body(ProductId);
                if (query != null)
                {
                    list = PlasCommon.ToolClass<Ul_bodyModel>.ConvertDataTableToModel(query);
                }
            }
            return list;
        }


        #endregion

    }
}
