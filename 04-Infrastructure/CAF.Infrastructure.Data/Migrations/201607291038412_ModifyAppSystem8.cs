namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Channel", "ShowExtendedAttribute", c => c.Boolean(nullable: false));
            AddColumn("dbo.Channel", "ShowSpecificationAttributes", c => c.Boolean(nullable: false));
            AddColumn("dbo.Channel", "ShowInventory", c => c.Boolean(nullable: false));
            AddColumn("dbo.Channel", "ShowPrice", c => c.Boolean(nullable: false));
            AddColumn("dbo.Channel", "ShowAttributes", c => c.Boolean(nullable: false));
            AddColumn("dbo.Channel", "ShowPromotion", c => c.Boolean(nullable: false));
            DropColumn("dbo.Channel", "ChannelTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Channel", "ChannelTypeId", c => c.Int(nullable: false));
            DropColumn("dbo.Channel", "ShowPromotion");
            DropColumn("dbo.Channel", "ShowAttributes");
            DropColumn("dbo.Channel", "ShowPrice");
            DropColumn("dbo.Channel", "ShowInventory");
            DropColumn("dbo.Channel", "ShowSpecificationAttributes");
            DropColumn("dbo.Channel", "ShowExtendedAttribute");
        }
    }
}
