namespace PlasQueryWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ss : DbMigration
    {
        public override void Up()
        {
            //DropPrimaryKey("dbo.cp_user");
            //AlterColumn("dbo.cp_user", "ID", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("dbo.cp_user", "ID");
        }
        
        public override void Down()
        {
            //DropPrimaryKey("dbo.cp_user");
            //AlterColumn("dbo.cp_user", "ID", c => c.String(nullable: false, maxLength: 128));
            //AddPrimaryKey("dbo.cp_user", "ID");
        }
    }
}
