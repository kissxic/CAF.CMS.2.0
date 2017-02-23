namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AgencyContent", "ProvinceName", c => c.String(maxLength: 50));
            AddColumn("dbo.AgencyContent", "CityName", c => c.String(maxLength: 50));
            DropColumn("dbo.AgencyContent", "Province");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AgencyContent", "Province", c => c.String(maxLength: 50));
            DropColumn("dbo.AgencyContent", "CityName");
            DropColumn("dbo.AgencyContent", "ProvinceName");
        }
    }
}
