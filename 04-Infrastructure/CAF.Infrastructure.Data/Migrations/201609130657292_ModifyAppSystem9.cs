namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserContent", "VendorId", c => c.Int());
            DropColumn("dbo.ArticleReview", "VendorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ArticleReview", "VendorId", c => c.Int(nullable: false));
            DropColumn("dbo.UserContent", "VendorId");
        }
    }
}
