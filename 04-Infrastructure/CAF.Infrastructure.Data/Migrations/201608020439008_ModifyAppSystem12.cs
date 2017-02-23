namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Channel_ProductCategory_Mapping",
                c => new
                    {
                        Channel_Id = c.Int(nullable: false),
                        ProductCategory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Channel_Id, t.ProductCategory_Id })
                .ForeignKey("dbo.Channel", t => t.Channel_Id, cascadeDelete: true)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategory_Id, cascadeDelete: true)
                .Index(t => t.Channel_Id)
                .Index(t => t.ProductCategory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Channel_ProductCategory_Mapping", "ProductCategory_Id", "dbo.ProductCategory");
            DropForeignKey("dbo.Channel_ProductCategory_Mapping", "Channel_Id", "dbo.Channel");
            DropIndex("dbo.Channel_ProductCategory_Mapping", new[] { "ProductCategory_Id" });
            DropIndex("dbo.Channel_ProductCategory_Mapping", new[] { "Channel_Id" });
            DropTable("dbo.Channel_ProductCategory_Mapping");
        }
    }
}
