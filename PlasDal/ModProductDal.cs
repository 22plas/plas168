using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
    public class ModProductDal
    {
        /// <summary>
        /// 获取改新厂列表
        /// </summary>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">每页条数</param>
        /// <returns></returns>
        public DataTable GetModProductList(int pageindex, int pagesize)
        {
            int strnumber = ((pageindex - 1) * pagesize) - 1;
            int endnumber = pageindex * pagesize;
            string sql = string.Format(@"SELECT * FROM (SELECT row_number()over(order by Id) as rowNumber,* FROM Mod_Product) a WHERE a.rowNumber BETWEEN {0} AND {1}", strnumber, endnumber);
            return SqlHelper.GetSqlDataTable(sql);
        }

        /// <summary>
        /// 获取改新厂产品详情
        /// </summary>
        /// <param name="id">产品id</param>
        /// <returns></returns>
        public DataSet GetModProductDetail(int id)
        {
            string sql = string.Format(@"SELECT p.Id,p.ProModel,p.PictPath,p.WheelPath,pr.Price,pr.DiscountPrice,pm.Title,pr.MinQty,pm.DetailsMemo,pm.Unit,c.Name,c.MainPhoto,c.MainBusiness,
                                        pr.PayType,p.Color,pr.MaxQty/1000 AS MaxQty,pm.Native,case when pm.Brand='' then '无' else pm.Brand end as Brand,CASE WHEN pm.IsImported=1 THEN '是' ELSE '否' END AS IsImported,pm.Catgory,pr.Packing,pr.RateType,
                                        c.Category AS Ccategry,c.RegisteredCapital,c.Address,c.WebSite,CONVERT(DATE,c.RegisterTime) RegisterTime,p.ProductGuid,pm.Service,
                                        c.Salesperson,c.SalseMoble,c.SalseQQ,c.SalseWechat,c.SalseMail FROM dbo.Mod_Product p
                                        INNER JOIN dbo.Mod_Price pr ON p.Id=pr.ProductId
                                        INNER JOIN dbo.Mod_SaleMessage pm ON p.Id=pm.ProductId
                                        LEFT JOIN dbo.Mod_Customer c ON c.CustId=p.Custid where p.id={0};
                                        SELECT TOP 1 p.ProModel,p.PlaceOrigin FROM [dbo].[Mod_TargetList] mt
                                        INNER JOIN dbo.Product p ON p.ProductGuid=mt.TargetProductId WHERE mt.ModProuctId={0}", id);
            DataSet dset = SqlHelper.GetSqlDataSet(sql);
            return dset;
        }
        /// <summary>
        /// 查询改新厂替换详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetNewFactoryth(int id)
        {
            string sql = string.Format(@"SELECT p.ProModel,p.PlaceOrigin FROM [dbo].[Mod_TargetList] mt
                                        INNER JOIN dbo.Product p ON p.ProductGuid=mt.TargetProductId WHERE mt.ModProuctId={0}", id);
            return SqlHelper.GetSqlDataTable(sql);
        }
    }
}
