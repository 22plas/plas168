using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
   public class ContrastBll
    {

        private PlasDal.ContrastDal dal = new PlasDal.ContrastDal();
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public DataSet GetProductList(int pageSize = 10, int pageIndex = 1)
        {
            return dal.GetProductList(pageSize, pageIndex);
        }

    }
}
