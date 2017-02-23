namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeliveryTime", "CreatedOnUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.DeliveryTime", "UpdatedOnUtc", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeliveryTime", "UpdatedOnUtc");
            DropColumn("dbo.DeliveryTime", "CreatedOnUtc");
        }
    }
}
