
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
	public class BannerInfoModel : EntityModelBase
    {
		[LangResourceDisplayName("Admin.ContentManagement.BannerInfo.Name", "����")]
		public string Name{get;set;}
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.UrlType", "����")]
        public int UrlTypeId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.Url", "URL")]
        public string Url { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.Position", "λ��")]
        public int Position { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.DisplayOrder", "����")]
        public int DisplayOrder { get; set; }


        public BannerUrltypes BannerUrltypes
        {
            get
            {
                return (BannerUrltypes)this.UrlTypeId;
            }
            set
            {
                this.UrlTypeId = (int)value;
            }
        }
    }
}
