using CAF.Infrastructure.Data;
using CAF.Infrastructure.Data.Setup;
using CAF.WebSite.PageSettings.Data.Migrations;
using System;
using System.Data.Entity;
namespace CAF.WebSite.PageSettings.Data
{
	public class PageSettingObjectContext : ObjectContextBase
	{
        public const string ALIASKEY = "caf_object_context_pagesetting";
		static PageSettingObjectContext()
		{
            var initializer = new MigrateDatabaseInitializer<PageSettingObjectContext, Configuration>
            {
                TablesToCheck = new[] { "BannerInfo" }
            };
            Database.SetInitializer(initializer);
		}
		public PageSettingObjectContext()
		{
		}
        public PageSettingObjectContext(string nameOrConnectionString)
            : base(nameOrConnectionString, ALIASKEY)
		{
		}
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
            modelBuilder.Configurations.Add(new BannerInfoMap());
			base.OnModelCreating(modelBuilder);
		}
	}
}
