using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.WebUI.Mvc;


namespace CAF.WebSite.Mvc.Models.HomeFloors
{
    public class HomeCategoryInfoModel : ModelBase
    {
        public int CategoryId { get; set; }

        // public virtual ProductCategory CategoryInfo { get; set; }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int ParentId { get; set; }

        public int Depth { get; set; }

        public int RowNumber { get; set; }

        public string Url { get; set; }

        public HomeCategoryInfoModel()
        {
        }
    }
}