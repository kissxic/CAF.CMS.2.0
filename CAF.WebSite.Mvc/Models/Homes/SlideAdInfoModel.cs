using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.Mvc.Models.Homes
{
    public class SlideAdInfoModel : EntityModelBase
    {

        public string Description { get; set; }


        public string Url { get; set; }


        public int PictureId { get; set; }


        public string PictureUrl { get; set; }


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
