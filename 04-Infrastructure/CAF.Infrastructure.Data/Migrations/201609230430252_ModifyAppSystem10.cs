namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserArticlePublicNum",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ChannelId = c.Int(nullable: false),
                        PublicTotal = c.Int(nullable: false),
                        PublicedTotal = c.Int(nullable: false),
                        UnLimit = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Channel", t => t.ChannelId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ChannelId);
            
            AddColumn("dbo.Channel", "LimitNum", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserArticlePublicNum", "UserId", "dbo.User");
            DropForeignKey("dbo.UserArticlePublicNum", "ChannelId", "dbo.Channel");
            DropIndex("dbo.UserArticlePublicNum", new[] { "ChannelId" });
            DropIndex("dbo.UserArticlePublicNum", new[] { "UserId" });
            DropColumn("dbo.Channel", "LimitNum");
            DropTable("dbo.UserArticlePublicNum");
        }
    }
}
