
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.PageSettings.Domain;
using System;
namespace CAF.WebSite.PageSettings.Models
{
	public class BannerInfoModel : EntityModelBase
    {
		[LangResourceDisplayName("Plugins.BannerInfo.Name", "����")]
		public string Name{get;set;}
        [LangResourceDisplayName("Plugins.BannerInfo.UrlType", "����")]
        public int UrlTypeId { get; set; }
        [LangResourceDisplayName("Plugins.BannerInfo.Url", "URL")]
        public string Url { get; set; }
        [LangResourceDisplayName("Plugins.BannerInfo.Position", "λ��")]
        public int Position { get; set; }
        [LangResourceDisplayName("Plugins.BannerInfo.DisplayOrder", "����")]
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
