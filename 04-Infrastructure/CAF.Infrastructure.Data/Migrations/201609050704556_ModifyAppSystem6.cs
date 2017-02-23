namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem6 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Article", new[] { "CategoryId" });
            AlterColumn("dbo.Article", "CategoryId", c => c.Int());
            CreateIndex("dbo.Article", "CategoryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Article", new[] { "CategoryId" });
            AlterColumn("dbo.Article", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Article", "CategoryId");
        }
    }
}
