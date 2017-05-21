namespace DashDocs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customer_name_not_null : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "Name", c => c.String());
        }
    }
}
