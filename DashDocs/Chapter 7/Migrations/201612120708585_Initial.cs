namespace DashDocs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DocumentName = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        BlobPath = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.Users", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Documents", new[] { "OwnerId" });
            DropIndex("dbo.Users", new[] { "CustomerId" });
            DropTable("dbo.Documents");
            DropTable("dbo.Users");
            DropTable("dbo.Customers");
        }
    }
}
