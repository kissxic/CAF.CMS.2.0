namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HomeCategoryItem", "Name", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HomeCategoryItem", "Name", c => c.String());
        }
    }
}
