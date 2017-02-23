namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavoriteInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Article", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ArticleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FavoriteInfo", "UserId", "dbo.User");
            DropForeignKey("dbo.FavoriteInfo", "ArticleId", "dbo.Article");
            DropIndex("dbo.FavoriteInfo", new[] { "ArticleId" });
            DropIndex("dbo.FavoriteInfo", new[] { "UserId" });
            DropTable("dbo.FavoriteInfo");
        }
    }
}
