namespace PlasQueryWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upus : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.cp_user", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.cp_user", "CreateDate", c => c.String());
        }
    }
}
