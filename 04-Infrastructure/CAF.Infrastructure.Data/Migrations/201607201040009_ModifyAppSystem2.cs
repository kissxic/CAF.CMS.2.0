namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ArticleAttribute", newName: "ArticleExtendedAttribute");
            DropForeignKey("dbo.ArticleExtend", "ArticleId", "dbo.Article");
            DropIndex("dbo.ArticleExtend", new[] { "ArticleId" });
            CreateTable(
                "dbo.ProductVariantAttributeCombination",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        AttributesXml = c.String(),
                        StockQuantity = c.Int(nullable: false),
                        AllowOutOfStockOrders = c.Boolean(nullable: false),
                        Sku = c.String(maxLength: 400),
                        Gtin = c.String(maxLength: 400),
                        ManufacturerPartNumber = c.String(maxLength: 400),
                        Price = c.Decimal(precision: 18, scale: 4),
                        Length = c.Decimal(precision: 18, scale: 4),
                        Width = c.Decimal(precision: 18, scale: 4),
                        Height = c.Decimal(precision: 18, scale: 4),
                        BasePriceAmount = c.Decimal(precision: 18, scale: 4),
                        BasePriceBaseAmount = c.Int(),
                        AssignedPictureIds = c.String(maxLength: 1000),
                        DeliveryTimeId = c.Int(),
                        QuantityUnitId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("dbo.DeliveryTime", t => t.DeliveryTimeId)
                .ForeignKey("dbo.QuantityUnit", t => t.QuantityUnitId)
                .Index(t => t.ArticleId)
                .Index(t => t.DeliveryTimeId)
                .Index(t => t.QuantityUnitId);
            
            CreateTable(
                "dbo.Product_ProductAttribute_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(nullable: false),
                        ProductAttributeId = c.Int(nullable: false),
                        TextPrompt = c.String(),
                        IsRequired = c.Boolean(nullable: false),
                        AttributeControlTypeId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("dbo.ProductAttribute", t => t.ProductAttributeId, cascadeDelete: true)
                .Index(t => t.ArticleId, name: "IX_Article_ProductAttribute_Mapping_ProductId_DisplayOrder")
                .Index(t => t.ProductAttributeId)
                .Index(t => t.DisplayOrder, name: "IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder");
            
            CreateTable(
                "dbo.ProductAttribute",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Alias = c.String(maxLength: 100),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductAttributeOption",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductAttributeId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductAttribute", t => t.ProductAttributeId, cascadeDelete: true)
                .Index(t => t.ProductAttributeId);
            
            CreateTable(
                "dbo.ProductVariantAttributeValue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductVariantAttributeId = c.Int(nullable: false),
                        Alias = c.String(maxLength: 100),
                        Name = c.String(),
                        ColorSquaresRgb = c.String(maxLength: 100),
                        PriceAdjustment = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WeightAdjustment = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsPreSelected = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        ValueTypeId = c.Int(nullable: false),
                        LinkedProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product_ProductAttribute_Mapping", t => t.ProductVariantAttributeId, cascadeDelete: true)
                .Index(t => new { t.ProductVariantAttributeId, t.DisplayOrder }, name: "IX_ProductVariantAttributeValue_ProductVariantAttributeId_DisplayOrder");
            
            DropTable("dbo.ArticleExtend");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ArticleExtend",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Value = c.String(maxLength: 50),
                        Type = c.String(maxLength: 500),
                        SortId = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.ProductVariantAttributeValue", "ProductVariantAttributeId", "dbo.Product_ProductAttribute_Mapping");
            DropForeignKey("dbo.Product_ProductAttribute_Mapping", "ProductAttributeId", "dbo.ProductAttribute");
            DropForeignKey("dbo.ProductAttributeOption", "ProductAttributeId", "dbo.ProductAttribute");
            DropForeignKey("dbo.Product_ProductAttribute_Mapping", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.ProductVariantAttributeCombination", "QuantityUnitId", "dbo.QuantityUnit");
            DropForeignKey("dbo.ProductVariantAttributeCombination", "DeliveryTimeId", "dbo.DeliveryTime");
            DropForeignKey("dbo.ProductVariantAttributeCombination", "ArticleId", "dbo.Article");
            DropIndex("dbo.ProductVariantAttributeValue", "IX_ProductVariantAttributeValue_ProductVariantAttributeId_DisplayOrder");
            DropIndex("dbo.ProductAttributeOption", new[] { "ProductAttributeId" });
            DropIndex("dbo.Product_ProductAttribute_Mapping", "IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder");
            DropIndex("dbo.Product_ProductAttribute_Mapping", new[] { "ProductAttributeId" });
            DropIndex("dbo.Product_ProductAttribute_Mapping", "IX_Article_ProductAttribute_Mapping_ProductId_DisplayOrder");
            DropIndex("dbo.ProductVariantAttributeCombination", new[] { "QuantityUnitId" });
            DropIndex("dbo.ProductVariantAttributeCombination", new[] { "DeliveryTimeId" });
            DropIndex("dbo.ProductVariantAttributeCombination", new[] { "ArticleId" });
            DropTable("dbo.ProductVariantAttributeValue");
            DropTable("dbo.ProductAttributeOption");
            DropTable("dbo.ProductAttribute");
            DropTable("dbo.Product_ProductAttribute_Mapping");
            DropTable("dbo.ProductVariantAttributeCombination");
            CreateIndex("dbo.ArticleExtend", "ArticleId");
            AddForeignKey("dbo.ArticleExtend", "ArticleId", "dbo.Article", "Id");
            RenameTable(name: "dbo.ArticleExtendedAttribute", newName: "ArticleAttribute");
        }
    }
}
