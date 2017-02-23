namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductVariantAttributeValue", "ProductAttributeOptionId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductVariantAttributeValue", "ProductAttributeOptionId");
        }
    }
}
