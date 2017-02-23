namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Manufacturer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 400),
                        Description = c.String(),
                        MetaKeywords = c.String(maxLength: 400),
                        MetaDescription = c.String(),
                        MetaTitle = c.String(maxLength: 400),
                        PictureId = c.Int(),
                        PageSize = c.Int(nullable: false),
                        AllowUsersToSelectPageSize = c.Boolean(nullable: false),
                        PageSizeOptions = c.String(maxLength: 200),
                        Published = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        CreatedUserID = c.Long(),
                        ModifiedOnUtc = c.DateTime(),
                        ModifiedUserID = c.Long(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Picture", t => t.PictureId)
                .Index(t => t.PictureId)
                .Index(t => t.Deleted);
            
            AddColumn("dbo.Article", "ManufacturerId", c => c.Int());
            CreateIndex("dbo.Article", "ManufacturerId");
            AddForeignKey("dbo.Article", "ManufacturerId", "dbo.Manufacturer", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Article", "ManufacturerId", "dbo.Manufacturer");
            DropForeignKey("dbo.Manufacturer", "PictureId", "dbo.Picture");
            DropIndex("dbo.Manufacturer", new[] { "Deleted" });
            DropIndex("dbo.Manufacturer", new[] { "PictureId" });
            DropIndex("dbo.Article", new[] { "ManufacturerId" });
            DropColumn("dbo.Article", "ManufacturerId");
            DropTable("dbo.Manufacturer");
        }
    }
}
