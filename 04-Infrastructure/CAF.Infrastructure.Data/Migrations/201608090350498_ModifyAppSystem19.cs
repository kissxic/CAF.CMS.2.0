namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem19 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AgencyContent",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                        TrueName = c.String(maxLength: 50),
                        TelePhone = c.String(maxLength: 50),
                        Mobile = c.String(maxLength: 50),
                        QQ = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        Province = c.String(maxLength: 50),
                        DistributionChannelId = c.String(maxLength: 100),
                        AgentPropertyId = c.String(maxLength: 100),
                        Message = c.String(),
                        ShowAuthId = c.Int(nullable: false),
                        OtherChannel = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserContent", t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.ArticleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AgencyContent", "ArticleId", "dbo.Article");
            DropForeignKey("dbo.AgencyContent", "Id", "dbo.UserContent");
            DropIndex("dbo.AgencyContent", new[] { "ArticleId" });
            DropIndex("dbo.AgencyContent", new[] { "Id" });
            DropTable("dbo.AgencyContent");
        }
    }
}
