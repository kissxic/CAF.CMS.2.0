namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArticleCategory", "IconName", c => c.String(maxLength: 50));
            AddColumn("dbo.Channel", "IconName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Channel", "IconName");
            DropColumn("dbo.ArticleCategory", "IconName");
        }
    }
}
