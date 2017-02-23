using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
    public class SlideAdInfoModel : EntityModelBase
    {
        [LangResourceDisplayName("Admin.ContentManagement.SlideAdInfo.Name", "√Ë ˆ")]
        public string Description { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.SlideAdInfo.Name", "URL")]
        public string Url { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.SlideAdInfo.Name", "Õº∆¨")]
        public int PictureId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.SlideAdInfo.Name", "Õº∆¨")]
        public string PictureThumbnailUrl { get; set; }
        
        [LangResourceDisplayName("Admin.ContentManagement.SlideAdInfo.Name", "≈≈–Ú")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.SlideAdInfo.Name", "¿‡–Õ")]
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
