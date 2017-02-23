namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HomeFloorInfo", "RelateVendorIds", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HomeFloorInfo", "RelateVendorIds");
        }
    }
}
