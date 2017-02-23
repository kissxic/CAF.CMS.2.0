namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Channel", "CategoryShowTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Channel", "CategoryShowTypeId");
        }
    }
}
