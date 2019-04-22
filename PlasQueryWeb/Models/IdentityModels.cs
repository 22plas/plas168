using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PlasModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace PlasQueryWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        //用户充值信息表
        public DbSet<cp_user> cp_user { get; set; }
        ////用户信息表
        //public DbSet<tb_userinfo> tb_userinfos { get; set; }
        ////支付订单表
        //public DbSet<PayOrder> PayOrders { get; set; }
        ////产品分类表
        //public DbSet<tb_goodssort> tb_goodssort { get; set; }
        ////选品库信息
        //public DbSet<tb_Favorites> tb_Favorites { get; set; }
        ////商品信息
        //public DbSet<tb_goods> tb_goods { get; set; }
        ////淘宝客信息
        //public DbSet<tb_TKInfo> tb_TKInfo { get; set; }
        ////商品分类等级
        //public DbSet<GoodsSortGrade> GoodsSortGrade { get; set; }
        ////用户订单表
        //public DbSet<Tborder> Tborder { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}