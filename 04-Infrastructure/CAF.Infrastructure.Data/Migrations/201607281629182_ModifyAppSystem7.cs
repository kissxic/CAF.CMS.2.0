namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RewardPointsHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        PointsBalance = c.Int(nullable: false),
                        UsedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Message = c.String(),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Article_SpecificationAttribute_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        SpecificationAttributeOptionId = c.Int(nullable: false),
                        AllowFiltering = c.Boolean(nullable: false),
                        ShowOnArticlePage = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("dbo.SpecificationAttributeOption", t => t.SpecificationAttributeOptionId, cascadeDelete: true)
                .Index(t => t.ArticleId)
                .Index(t => t.SpecificationAttributeOptionId);
            
            CreateTable(
                "dbo.SpecificationAttributeOption",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SpecificationAttributeId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SpecificationAttribute", t => t.SpecificationAttributeId, cascadeDelete: true)
                .Index(t => t.SpecificationAttributeId);
            
            CreateTable(
                "dbo.SpecificationAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Article_SpecificationAttribute_Mapping", "SpecificationAttributeOptionId", "dbo.SpecificationAttributeOption");
            DropForeignKey("dbo.SpecificationAttributeOption", "SpecificationAttributeId", "dbo.SpecificationAttribute");
            DropForeignKey("dbo.Article_SpecificationAttribute_Mapping", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.RewardPointsHistories", "UserId", "dbo.User");
            DropIndex("dbo.SpecificationAttributeOption", new[] { "SpecificationAttributeId" });
            DropIndex("dbo.Article_SpecificationAttribute_Mapping", new[] { "SpecificationAttributeOptionId" });
            DropIndex("dbo.Article_SpecificationAttribute_Mapping", new[] { "ArticleId" });
            DropIndex("dbo.RewardPointsHistories", new[] { "UserId" });
            DropTable("dbo.SpecificationAttribute");
            DropTable("dbo.SpecificationAttributeOption");
            DropTable("dbo.Article_SpecificationAttribute_Mapping");
            DropTable("dbo.RewardPointsHistories");
        }
    }
}
