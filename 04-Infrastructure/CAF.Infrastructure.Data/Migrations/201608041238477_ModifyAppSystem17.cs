namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem17 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberGrade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemName = c.String(nullable: false, maxLength: 255),
                        GradeName = c.String(maxLength: 255),
                        Integral = c.Int(nullable: false),
                        Comment = c.String(maxLength: 500),
                        IsDefault = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberGradeMapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(nullable: false, maxLength: 400),
                        MemberGradeId = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.User", "MemberGradeId", c => c.Int(nullable: false));
            AddColumn("dbo.Channel", "LimitedToMemberGrades", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Channel", "LimitedToMemberGrades");
            DropColumn("dbo.User", "MemberGradeId");
            DropTable("dbo.MemberGradeMapping");
            DropTable("dbo.MemberGrade");
        }
    }
}
