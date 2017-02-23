namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem4 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Article", new[] { "ProductCategoryId" });
            AlterColumn("dbo.Article", "ProductCategoryId", c => c.Int());
            CreateIndex("dbo.Article", "ProductCategoryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Article", new[] { "ProductCategoryId" });
            AlterColumn("dbo.Article", "ProductCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Article", "ProductCategoryId");
        }
    }
}
