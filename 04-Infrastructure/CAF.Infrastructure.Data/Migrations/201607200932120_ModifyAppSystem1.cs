namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Article", "IsFreeShipping", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Article", "IsFreeShipping");
        }
    }
}
