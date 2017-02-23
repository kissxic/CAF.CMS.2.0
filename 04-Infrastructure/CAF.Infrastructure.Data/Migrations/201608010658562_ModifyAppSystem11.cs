namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem11 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Addresses", newName: "Address");
            CreateTable(
                "dbo.Vendor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        Email = c.String(maxLength: 400),
                        Description = c.String(),
                        PictureId = c.Int(nullable: false),
                        AdminComment = c.String(),
                        CompanyName = c.String(maxLength: 400),
                        CompanyRegionId = c.Int(nullable: false),
                        CompanyAddress = c.String(maxLength: 400),
                        CompanyPhone = c.String(maxLength: 400),
                        CompanyEmployeeCountId = c.Int(nullable: false),
                        CompanyRegisteredCapital = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LegalPerson = c.String(maxLength: 400),
                        ContactsName = c.String(maxLength: 400),
                        ContactsPhone = c.String(maxLength: 400),
                        ContactsEmail = c.String(maxLength: 400),
                        BankAccountName = c.String(maxLength: 400),
                        BankAccountNumber = c.String(maxLength: 400),
                        BankName = c.String(maxLength: 400),
                        BankCode = c.String(maxLength: 400),
                        BankRegionId = c.Int(nullable: false),
                        BankPictureId = c.Int(nullable: false),
                        VendorGradeId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        StageId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        MetaKeywords = c.String(maxLength: 400),
                        MetaDescription = c.String(),
                        MetaTitle = c.String(maxLength: 400),
                        PageSize = c.Int(nullable: false),
                        AllowCustomersToSelectPageSize = c.Boolean(nullable: false),
                        PageSizeOptions = c.String(maxLength: 200),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VendorNote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VendorId = c.Int(nullable: false),
                        Note = c.String(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vendor", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
            
            AddColumn("dbo.User", "VendorId", c => c.Int(nullable: false));
            AddColumn("dbo.ArticleReview", "VendorId", c => c.Int(nullable: false));
            AddColumn("dbo.Article", "StateProvinceId", c => c.Int());
            AddColumn("dbo.Article", "CityId", c => c.Int());
            AddColumn("dbo.Article", "DistrictId", c => c.Int());
            AddColumn("dbo.Article", "VendorId", c => c.Int(nullable: false));
            CreateIndex("dbo.Article", "StateProvinceId");
            CreateIndex("dbo.Article", "CityId");
            CreateIndex("dbo.Article", "DistrictId");
            AddForeignKey("dbo.Article", "CityId", "dbo.City", "Id");
            AddForeignKey("dbo.Article", "DistrictId", "dbo.District", "Id");
            AddForeignKey("dbo.Article", "StateProvinceId", "dbo.StateProvince", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Article", "StateProvinceId", "dbo.StateProvince");
            DropForeignKey("dbo.Article", "DistrictId", "dbo.District");
            DropForeignKey("dbo.Article", "CityId", "dbo.City");
            DropForeignKey("dbo.VendorNote", "VendorId", "dbo.Vendor");
            DropIndex("dbo.Article", new[] { "DistrictId" });
            DropIndex("dbo.Article", new[] { "CityId" });
            DropIndex("dbo.Article", new[] { "StateProvinceId" });
            DropIndex("dbo.VendorNote", new[] { "VendorId" });
            DropColumn("dbo.Article", "VendorId");
            DropColumn("dbo.Article", "DistrictId");
            DropColumn("dbo.Article", "CityId");
            DropColumn("dbo.Article", "StateProvinceId");
            DropColumn("dbo.ArticleReview", "VendorId");
            DropColumn("dbo.User", "VendorId");
            DropTable("dbo.VendorNote");
            DropTable("dbo.Vendor");
            RenameTable(name: "dbo.Address", newName: "Addresses");
        }
    }
}
