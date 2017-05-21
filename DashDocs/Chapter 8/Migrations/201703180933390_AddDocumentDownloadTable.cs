namespace DashDocs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentDownloadTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentDownloads",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DocumentId = c.Guid(nullable: false),
                        DownloadedOn = c.DateTime(nullable: false),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.DocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentDownloads", "DocumentId", "dbo.Documents");
            DropIndex("dbo.DocumentDownloads", new[] { "DocumentId" });
            DropTable("dbo.DocumentDownloads");
        }
    }
}
