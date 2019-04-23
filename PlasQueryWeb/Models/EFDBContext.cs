using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasModel
{
    public class EFDBContext : DbContext
    {
        public EFDBContext() : base("ConnectionStrings") //数据库链接的Web.config中结点名字
        {

        }
        public DbSet<cp_user> cp_user { get; set; }

    }
}
