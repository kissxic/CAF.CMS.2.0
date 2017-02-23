using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Media;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Mvc.Seller.Models.Articles;

namespace CAF.WebSite.Mvc.Seller.Controllers
{
    public class ProductCategoryController : SellerAdminControllerBase
    {
        #region Fields
        private readonly IProductCategoryService _categoryService;
        private readonly IUserActivityService _userActivityService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ArticleCatalogSettings _catalogSettings;
        private readonly IChannelService _channelService;
        #endregion

        #region Ctor

        public ProductCategoryController(
            IProductCategoryService categoryService,
            IUserActivityService userActivityService,
            ArticleCatalogSettings catalogSettings,
            IChannelService channelService,
            IWorkContext workContext,
            ISiteContext siteContext)
        {
            this._categoryService = categoryService;
            this._userActivityService = userActivityService;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._channelService = channelService;
            this._siteContext = siteContext;
        }
        #endregion


        #region List

    
        public JsonResult GetAuthorizationCategory(int? channelId)
        {
            var businessCategory = _categoryService.GetAllCategories(showHidden: false);

            List<CategoryJsonModel> categoryJsonModels = new List<CategoryJsonModel>();

            foreach (ProductCategory categoryInfo in businessCategory)
            {
                string[] strArrays = categoryInfo.Path.Split(new char[] { '|' });
                int num = int.Parse(strArrays.Length > 0 ? strArrays[0] : "0");
                int num1 = int.Parse(strArrays.Length > 1 ? strArrays[1] : "0");
                var category = _categoryService.GetProductCategoryById(num);
                var categoryJsonModel = new CategoryJsonModel()
                {
                    Name = category.Name,
                    Id = category.Id.ToString(),
                    SubCategory = new List<SecondLevelCategory>()
                };
                SecondLevelCategory secondLevelCategory = null;
                ThirdLevelCategoty thirdLevelCategoty = null;
                ProductCategory category1 = _categoryService.GetProductCategoryById(num1);
                if (category1 != null)
                {
                    secondLevelCategory = new SecondLevelCategory()
                    {
                        Name = category1.Name,
                        Id = category1.Id.ToString(),
                        SubCategory = new List<ThirdLevelCategoty>()
                    };

                    if (strArrays.Length >= 3)
                    {
                        thirdLevelCategoty = new ThirdLevelCategoty()
                        {
                            Id = categoryInfo.Id.ToString(),
                            Name = categoryInfo.Name
                        };

                    }
                    if (thirdLevelCategoty != null)
                    {
                        secondLevelCategory.SubCategory.Add(thirdLevelCategoty);
                    }
                }
                CategoryJsonModel categoryJsonModel2 = categoryJsonModels.FirstOrDefault((CategoryJsonModel j) => j.Id == category.Id.ToString());
                if (categoryJsonModel2 != null && category1 != null && categoryJsonModel2.SubCategory.Any((SecondLevelCategory j) => j.Id == category1.Id.ToString()))
                {
                    categoryJsonModel2.SubCategory.FirstOrDefault((SecondLevelCategory j) => j.Id == category1.Id.ToString()).SubCategory.Add(thirdLevelCategoty);
                }
                else if (categoryJsonModel2 != null && secondLevelCategory != null)
                {
                    categoryJsonModel2.SubCategory.Add(secondLevelCategory);
                }
                else if (secondLevelCategory != null)
                {
                    categoryJsonModel.SubCategory.Add(secondLevelCategory);
                }
                if (categoryJsonModels.FirstOrDefault((CategoryJsonModel j) => j.Id == category.Id.ToString()) != null)
                {
                    continue;
                }
                categoryJsonModels.Add(categoryJsonModel);
            }
            return Json(new { json = categoryJsonModels }, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}