

using CAF.Infrastructure.Core.Configuration;
namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    public class HomePageSettings : ISettings
    {
        public HomePageSettings()
        {
      
          
        }
        /// <summary>
        /// 关联品牌IDs
        /// </summary>
        public string RelateManufacturerIds { get; set; }
        /// <summary>
        /// 关联栏目分类IDs
        /// </summary>
        public string RelateCategoryIds { get; set; }
        /// <summary>
        /// 关联商品IDs
        /// </summary>
        public string RelateProductIds { get; set; }
        /// <summary>
        /// 关联商品IDs
        /// </summary>
        public string RelateVendorIds { get; set; }
    }
}