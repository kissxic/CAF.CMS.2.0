namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem13 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        CurrencyCode = c.String(nullable: false, maxLength: 5),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 8),
                        DisplayLocale = c.String(maxLength: 50),
                        CustomFormatting = c.String(maxLength: 50),
                        LimitedToSites = c.Boolean(nullable: false),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        UpdatedOnUtc = c.DateTime(nullable: false),
                        DomainEndings = c.String(maxLength: 1000),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ArticleCategory", "PriceRanges", c => c.String(maxLength: 400));
            AddColumn("dbo.Site", "PrimarySiteCurrencyId", c => c.Int(nullable: true));
            CreateIndex("dbo.Site", "PrimarySiteCurrencyId");
            AddForeignKey("dbo.Site", "PrimarySiteCurrencyId", "dbo.Currency", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Site", "PrimarySiteCurrencyId", "dbo.Currency");
            DropIndex("dbo.Site", new[] { "PrimarySiteCurrencyId" });
            DropColumn("dbo.Site", "PrimarySiteCurrencyId");
            DropColumn("dbo.ArticleCategory", "PriceRanges");
            DropTable("dbo.Currency");
        }
    }
}
