using System;
using System.Data.Entity.Migrations;
namespace CAF.WebSite.PageSettings.Data.Migrations
{
	internal sealed class Configuration : DbMigrationsConfiguration<PageSettingObjectContext>
	{
		public Configuration()
		{
            base.AutomaticMigrationsEnabled = false;
            base.MigrationsDirectory = "Data\\Migrations";
            base.ContextKey = "CAF.WebSite.PageSettings";
		}
		protected override void Seed(PageSettingObjectContext context)
		{
		}
	}
}
