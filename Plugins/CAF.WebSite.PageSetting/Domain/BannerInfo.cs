using CAF.Infrastructure.Core;
using System;
namespace CAF.WebSite.PageSettings.Domain
{
    public class BannerInfo : AuditedBaseEntity
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
