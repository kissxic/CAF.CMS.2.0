
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.Mvc.Models.Homes
{
    public class BannerInfoModel : EntityModelBase
    {

        public string Name { get; set; }

        public int UrlTypeId { get; set; }

        public string Url { get; set; }

        public int Position { get; set; }

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
