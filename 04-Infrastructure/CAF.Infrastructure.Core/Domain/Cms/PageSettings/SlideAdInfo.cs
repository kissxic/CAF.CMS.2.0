using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    public class SlideAdInfo : AuditedBaseEntity
    {
        public string Description { get; set; }

        public string Url { get; set; }

        public int PictureId { get; set; }

        public Picture Picture { get; set; }

        public int DisplayOrder { get; set; }

        public int SlideAdTypeId { get; set; }

        public SlideAdType SlideAdType
        {
            get
            {
                return (SlideAdType)this.SlideAdTypeId;
            }
            set
            {
                this.SlideAdTypeId = (int)value;
            }
        }
    }

}
