namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Article", "ChannelId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Article", "ChannelId");
        }
    }
}
