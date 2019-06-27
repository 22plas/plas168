using PlasDal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasBll
{
    public class NewsBll
    {
        NewsDal dal = new NewsDal();
        //获取案例
        public DataTable GetNews(int pageindex, int pagesize)
        {
            return dal.GetNews(pageindex, pagesize);
        }
    }
}
