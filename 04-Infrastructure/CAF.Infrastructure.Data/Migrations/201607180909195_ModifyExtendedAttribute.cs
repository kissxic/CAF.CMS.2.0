namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyExtendedAttribute : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Channel_ExtendedAttribute_Mapping", "Channel_Id", "dbo.Channel");
            DropForeignKey("dbo.Channel_ExtendedAttribute_Mapping", "ExtendedAttribute_Id", "dbo.ExtendedAttribute");
            DropIndex("dbo.Channel_ExtendedAttribute_Mapping", new[] { "Channel_Id" });
            DropIndex("dbo.Channel_ExtendedAttribute_Mapping", new[] { "ExtendedAttribute_Id" });
            AddColumn("dbo.ExtendedAttribute", "ChannelId", c => c.Int(nullable: false));
            CreateIndex("dbo.ExtendedAttribute", "ChannelId");
            AddForeignKey("dbo.ExtendedAttribute", "ChannelId", "dbo.Channel", "Id", cascadeDelete: true);
            DropColumn("dbo.ExtendedAttribute", "CategoryId");
            DropTable("dbo.Channel_ExtendedAttribute_Mapping");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Channel_ExtendedAttribute_Mapping",
                c => new
                    {
                        Channel_Id = c.Int(nullable: false),
                        ExtendedAttribute_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Channel_Id, t.ExtendedAttribute_Id });
            
            AddColumn("dbo.ExtendedAttribute", "CategoryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ExtendedAttribute", "ChannelId", "dbo.Channel");
            DropIndex("dbo.ExtendedAttribute", new[] { "ChannelId" });
            DropColumn("dbo.ExtendedAttribute", "ChannelId");
            CreateIndex("dbo.Channel_ExtendedAttribute_Mapping", "ExtendedAttribute_Id");
            CreateIndex("dbo.Channel_ExtendedAttribute_Mapping", "Channel_Id");
            AddForeignKey("dbo.Channel_ExtendedAttribute_Mapping", "ExtendedAttribute_Id", "dbo.ExtendedAttribute", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Channel_ExtendedAttribute_Mapping", "Channel_Id", "dbo.Channel", "Id", cascadeDelete: true);
        }
    }
}
