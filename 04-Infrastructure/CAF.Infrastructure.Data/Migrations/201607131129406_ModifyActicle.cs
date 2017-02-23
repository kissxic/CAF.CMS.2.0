namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyActicle : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.City",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvinceId = c.Int(nullable: false),
                        ProvinceCode = c.String(nullable: false, maxLength: 10),
                        Code = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 100),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StateProvince", t => t.ProvinceId, cascadeDelete: true)
                .Index(t => t.ProvinceId);
            
            CreateTable(
                "dbo.District",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CityId = c.Int(nullable: false),
                        CityCode = c.String(nullable: false, maxLength: 10),
                        Code = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.City", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId);
            
            AddColumn("dbo.StateProvince", "Code", c => c.String(nullable: false, maxLength: 10));
            AddColumn("dbo.Article", "MergedDataIgnore", c => c.Boolean(nullable: false));
            AddColumn("dbo.Article", "ArticleTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "Sku", c => c.String(maxLength: 400));
            AddColumn("dbo.Article", "DeliveryTimeId", c => c.Int());
            AddColumn("dbo.Article", "QuantityUnitId", c => c.Int());
            AddColumn("dbo.Article", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "Length", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "Width", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "Height", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "StockQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "DisplayStockAvailability", c => c.Boolean(nullable: false));
            AddColumn("dbo.Article", "DisplayStockQuantity", c => c.Boolean(nullable: false));
            AddColumn("dbo.Article", "MinStockQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "LowStockActivityId", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "NotifyAdminForQuantityBelow", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "AllowBackInStockSubscriptions", c => c.Boolean(nullable: false));
            AddColumn("dbo.Article", "OrderMinimumQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "OrderMaximumQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "DisableBuyButton", c => c.Boolean(nullable: false));
            AddColumn("dbo.Article", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "OldPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "ProductCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "SpecialPrice", c => c.Decimal(precision: 18, scale: 4));
            AddColumn("dbo.Article", "SpecialPriceStartDateTimeUtc", c => c.DateTime());
            AddColumn("dbo.Article", "SpecialPriceEndDateTimeUtc", c => c.DateTime());
            AddColumn("dbo.Article", "CustomerEntersPrice", c => c.Boolean(nullable: false));
            AddColumn("dbo.Article", "MinimumCustomerEnteredPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.Article", "MaximumCustomerEnteredPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            CreateIndex("dbo.Article", "DeliveryTimeId");
            CreateIndex("dbo.Article", "QuantityUnitId");
            AddForeignKey("dbo.Article", "DeliveryTimeId", "dbo.DeliveryTime", "Id");
            AddForeignKey("dbo.Article", "QuantityUnitId", "dbo.QuantityUnit", "Id");
            
        }
        
        public override void Down()
        {
            
            
            DropForeignKey("dbo.Article", "QuantityUnitId", "dbo.QuantityUnit");
            DropForeignKey("dbo.Article", "DeliveryTimeId", "dbo.DeliveryTime");
            DropForeignKey("dbo.City", "ProvinceId", "dbo.StateProvince");
            DropForeignKey("dbo.District", "CityId", "dbo.City");
            DropIndex("dbo.Article", new[] { "QuantityUnitId" });
            DropIndex("dbo.Article", new[] { "DeliveryTimeId" });
            DropIndex("dbo.District", new[] { "CityId" });
            DropIndex("dbo.City", new[] { "ProvinceId" });
            DropColumn("dbo.Article", "MaximumCustomerEnteredPrice");
            DropColumn("dbo.Article", "MinimumCustomerEnteredPrice");
            DropColumn("dbo.Article", "CustomerEntersPrice");
            DropColumn("dbo.Article", "SpecialPriceEndDateTimeUtc");
            DropColumn("dbo.Article", "SpecialPriceStartDateTimeUtc");
            DropColumn("dbo.Article", "SpecialPrice");
            DropColumn("dbo.Article", "ProductCost");
            DropColumn("dbo.Article", "OldPrice");
            DropColumn("dbo.Article", "Price");
            DropColumn("dbo.Article", "DisableBuyButton");
            DropColumn("dbo.Article", "OrderMaximumQuantity");
            DropColumn("dbo.Article", "OrderMinimumQuantity");
            DropColumn("dbo.Article", "AllowBackInStockSubscriptions");
            DropColumn("dbo.Article", "NotifyAdminForQuantityBelow");
            DropColumn("dbo.Article", "LowStockActivityId");
            DropColumn("dbo.Article", "MinStockQuantity");
            DropColumn("dbo.Article", "DisplayStockQuantity");
            DropColumn("dbo.Article", "DisplayStockAvailability");
            DropColumn("dbo.Article", "StockQuantity");
            DropColumn("dbo.Article", "Height");
            DropColumn("dbo.Article", "Width");
            DropColumn("dbo.Article", "Length");
            DropColumn("dbo.Article", "Weight");
            DropColumn("dbo.Article", "QuantityUnitId");
            DropColumn("dbo.Article", "DeliveryTimeId");
            DropColumn("dbo.Article", "Sku");
            DropColumn("dbo.Article", "ArticleTypeId");
            DropColumn("dbo.Article", "MergedDataIgnore");
            DropColumn("dbo.StateProvince", "Code");
            DropTable("dbo.District");
            DropTable("dbo.City");
        }
    }
}
