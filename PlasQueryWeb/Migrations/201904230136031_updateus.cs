namespace PlasQueryWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateus : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.cp_user", "ErrorCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.cp_user", "ErrorCount", c => c.String());
        }
    }
}
