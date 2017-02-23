namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Channel", "ChannelTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Channel", "ChannelTypeId");
        }
    }
}
