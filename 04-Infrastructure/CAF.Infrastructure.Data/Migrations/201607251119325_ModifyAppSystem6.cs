namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAttributeOption", "ColorSquaresRgb", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductAttributeOption", "ColorSquaresRgb");
        }
    }
}
