using PlasDal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
    public class ModProductBll
    {
        ModProductDal dal = new ModProductDal();
        //获取改新厂列表
        public DataTable GetModProductList(int pageindex, int pagesize)
        {
            return dal.GetModProductList(pageindex, pagesize);
        }
        //获取改新厂产品详情
        public DataSet GetModProductDetail(int id)
        {
            return dal.GetModProductDetail(id);
        }
        /// <summary>
        /// 查询改新厂替换详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetNewFactoryth(int id)
        {
            return dal.GetNewFactoryth(id);
        }
    }
}
