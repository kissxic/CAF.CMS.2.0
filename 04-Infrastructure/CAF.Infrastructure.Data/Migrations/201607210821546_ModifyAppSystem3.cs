namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ArticleCategoryExtend", "CategoryId", "dbo.ArticleCategory");
            DropIndex("dbo.ArticleCategoryExtend", new[] { "CategoryId" });
            CreateTable(
                "dbo.ProductCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        Alias = c.String(maxLength: 100),
                        FullName = c.String(maxLength: 400),
                        ParentCategoryId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        PictureId = c.Int(),
                        Description = c.String(),
                        Published = c.Boolean(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Picture", t => t.PictureId)
                .Index(t => t.PictureId);
            
            AddColumn("dbo.Article", "ProductCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Article", "ProductCategoryId");
            AddForeignKey("dbo.Article", "ProductCategoryId", "dbo.ProductCategory", "Id");
            DropTable("dbo.ArticleCategoryExtend");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ArticleCategoryExtend",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        Title = c.String(maxLength: 50),
                        ControlType = c.String(maxLength: 50),
                        DataType = c.String(maxLength: 50),
                        DataLength = c.Int(nullable: false),
                        DataPlace = c.Int(nullable: false),
                        ItemOption = c.String(maxLength: 50),
                        DefaultValue = c.String(maxLength: 50),
                        IsRequired = c.Boolean(nullable: false),
                        IsPassword = c.Boolean(nullable: false),
                        IsHtml = c.Boolean(nullable: false),
                        EditorType = c.Int(nullable: false),
                        ValidTipMsg = c.String(maxLength: 50),
                        ValidErrorMsg = c.String(maxLength: 50),
                        ValidPattern = c.String(maxLength: 50),
                        SortId = c.Int(nullable: false),
                        IsSys = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Article", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.ProductCategory", "PictureId", "dbo.Picture");
            DropIndex("dbo.ProductCategory", new[] { "PictureId" });
            DropIndex("dbo.Article", new[] { "ProductCategoryId" });
            DropColumn("dbo.Article", "ProductCategoryId");
            DropTable("dbo.ProductCategory");
            CreateIndex("dbo.ArticleCategoryExtend", "CategoryId");
            AddForeignKey("dbo.ArticleCategoryExtend", "CategoryId", "dbo.ArticleCategory", "Id");
        }
    }
}
