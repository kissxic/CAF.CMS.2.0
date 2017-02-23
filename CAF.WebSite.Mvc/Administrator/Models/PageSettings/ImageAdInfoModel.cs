using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.WebSite.Application.WebUI.Mvc;
 
namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
    public class ImageAdInfoModel : EntityModelBase
    {
        public int PictureId { get; set; }

        public Picture Picture { get; set; }

        public string PictureUrl { get; set; }

        public string Url { get; set; }

    }
}