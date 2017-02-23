namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "CityId", c => c.Int());
            AddColumn("dbo.Addresses", "DistrictId", c => c.Int());
            CreateIndex("dbo.Addresses", "CityId");
            CreateIndex("dbo.Addresses", "DistrictId");
            AddForeignKey("dbo.Addresses", "CityId", "dbo.City", "Id");
            AddForeignKey("dbo.Addresses", "DistrictId", "dbo.District", "Id");
            DropColumn("dbo.Addresses", "City");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Addresses", "City", c => c.String());
            DropForeignKey("dbo.Addresses", "DistrictId", "dbo.District");
            DropForeignKey("dbo.Addresses", "CityId", "dbo.City");
            DropIndex("dbo.Addresses", new[] { "DistrictId" });
            DropIndex("dbo.Addresses", new[] { "CityId" });
            DropColumn("dbo.Addresses", "DistrictId");
            DropColumn("dbo.Addresses", "CityId");
        }
    }
}
