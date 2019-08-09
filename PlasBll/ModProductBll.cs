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
    }
}
