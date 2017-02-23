
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
	public class BannerInfoModel : EntityModelBase
    {
		[LangResourceDisplayName("Admin.ContentManagement.BannerInfo.Name", "名称")]
		public string Name{get;set;}
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.UrlType", "类型")]
        public int UrlTypeId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.Url", "URL")]
        public string Url { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.Position", "位置")]
        public int Position { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.BannerInfo.DisplayOrder", "排序")]
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
