namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem20 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Channel_SpecificationAttribute_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChannelId = c.Int(nullable: false),
                        SpecificationAttributeOptionId = c.Int(nullable: false),
                        AllowFiltering = c.Boolean(nullable: false),
                        ShowOnArticlePage = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Channel", t => t.ChannelId, cascadeDelete: true)
                .ForeignKey("dbo.SpecificationAttributeOption", t => t.SpecificationAttributeOptionId, cascadeDelete: true)
                .Index(t => t.ChannelId)
                .Index(t => t.SpecificationAttributeOptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Channel_SpecificationAttribute_Mapping", "SpecificationAttributeOptionId", "dbo.SpecificationAttributeOption");
            DropForeignKey("dbo.Channel_SpecificationAttribute_Mapping", "ChannelId", "dbo.Channel");
            DropIndex("dbo.Channel_SpecificationAttribute_Mapping", new[] { "SpecificationAttributeOptionId" });
            DropIndex("dbo.Channel_SpecificationAttribute_Mapping", new[] { "ChannelId" });
            DropTable("dbo.Channel_SpecificationAttribute_Mapping");
        }
    }
}
