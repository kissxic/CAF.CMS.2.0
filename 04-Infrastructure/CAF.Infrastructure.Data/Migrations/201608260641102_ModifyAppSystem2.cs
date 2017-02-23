namespace CAF.Infrastructure.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyAppSystem2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vendor", "CompanyWebSite", c => c.String(maxLength: 100));
            AddColumn("dbo.Vendor", "ContactsFax", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "CompanyName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "CompanyPhone", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "LegalPerson", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "ContactsName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "ContactsPhone", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "ContactsEmail", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "BankAccountName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "BankAccountNumber", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "BankName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Vendor", "BankCode", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Vendor", "BankCode", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "BankName", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "BankAccountNumber", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "BankAccountName", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "ContactsEmail", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "ContactsPhone", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "ContactsName", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "LegalPerson", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "CompanyPhone", c => c.String(maxLength: 400));
            AlterColumn("dbo.Vendor", "CompanyName", c => c.String(maxLength: 400));
            DropColumn("dbo.Vendor", "ContactsFax");
            DropColumn("dbo.Vendor", "CompanyWebSite");
        }
    }
}
