using PlasDal;
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
        public DataTable GetSearchParam(int type, int showNum = 10)
        {
            return dal.GetSearchParam(type, showNum);
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

        //超级搜索
        public DataSet Sys_SuperSearch(string searchStr = "", int languageid = 2052, int pageCount = 0, int pageSize = 20, string guidstr = "",string isNavLink="")
        {
            return dal.Sys_SuperSearch(searchStr, languageid, pageCount, pageSize, guidstr, isNavLink);
        }

        #endregion

        //普通搜索
        public DataSet GetGeneralSearch(string key = "", int pageIndex = 1, int pageSize = 20, string strGuid = "")
        {
            return dal.GetGeneralSearch(key, pageIndex, pageSize, strGuid);
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
        #endregion

    }
}
