namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HomeCategoryItem", "SeName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HomeCategoryItem", "SeName");
        }
    }
}
