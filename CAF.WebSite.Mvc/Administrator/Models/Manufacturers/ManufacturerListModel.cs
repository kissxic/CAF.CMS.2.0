using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Admin.Models.Manufacturers
{
	public class ManufacturerListModel : ModelBase
    {
        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.List.SearchManufacturerName","名称")]
        [AllowHtml]
        public string SearchManufacturerName { get; set; }

		 

		public int GridPageSize { get; set; }
    }
}