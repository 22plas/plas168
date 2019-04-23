namespace PlasQueryWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sd : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.cp_user",
            //    c => new
            //        {
            //            ID = c.String(nullable: false, maxLength: 128),
            //            UserName = c.String(),
            //            UserPwd = c.String(),
            //            Email = c.String(),
            //            Phone = c.String(),
            //            Address = c.String(),
            //            TestQQ = c.String(),
            //            CreateDate = c.String(),
            //            states = c.Int(nullable: false),
            //            ErrorDate = c.String(),
            //            ErrorCount = c.String(),
            //            WeChat = c.String(),
            //            ContentAddress = c.String(),
            //            LeaderUserName = c.String(),
            //            HeadImage = c.String(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.cp_user");
        }
    }
}
