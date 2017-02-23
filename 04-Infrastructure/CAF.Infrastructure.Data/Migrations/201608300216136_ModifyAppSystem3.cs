namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategory", "PageSize", c => c.Int(nullable: false));
            AddColumn("dbo.ProductCategory", "AllowUsersToSelectPageSize", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductCategory", "PageSizeOptions", c => c.String(maxLength: 200));
            AddColumn("dbo.ProductCategory", "PriceRanges", c => c.String(maxLength: 400));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductCategory", "PriceRanges");
            DropColumn("dbo.ProductCategory", "PageSizeOptions");
            DropColumn("dbo.ProductCategory", "AllowUsersToSelectPageSize");
            DropColumn("dbo.ProductCategory", "PageSize");
        }
    }
}
