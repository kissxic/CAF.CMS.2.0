using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Runtime.CompilerServices;

namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
	public class HomeFloor: EntityModelBase
    {
		public int DisplayOrder { get; set; }

        public bool Enable { get; set; }

        public string Name { get; set; }

        public int StyleLevel { get; set; }

        public HomeFloor()
		{
		}
	}
}