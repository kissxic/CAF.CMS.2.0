namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleCategorySpecificationAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        SpecificationAttributeOptionId = c.Int(nullable: false),
                        AllowFiltering = c.Boolean(nullable: false),
                        ShowOnArticlePage = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ArticleCategory", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.SpecificationAttributeOption", t => t.SpecificationAttributeOptionId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.SpecificationAttributeOptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArticleCategorySpecificationAttribute", "SpecificationAttributeOptionId", "dbo.SpecificationAttributeOption");
            DropForeignKey("dbo.ArticleCategorySpecificationAttribute", "CategoryId", "dbo.ArticleCategory");
            DropIndex("dbo.ArticleCategorySpecificationAttribute", new[] { "SpecificationAttributeOptionId" });
            DropIndex("dbo.ArticleCategorySpecificationAttribute", new[] { "CategoryId" });
            DropTable("dbo.ArticleCategorySpecificationAttribute");
        }
    }
}
