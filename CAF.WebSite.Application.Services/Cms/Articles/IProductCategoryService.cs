using CAF.Infrastructure.Core.Pages;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// ProductCategory service interface
    /// </summary>
    public partial interface IProductCategoryService
    {
        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">ProductCategory</param>
        /// <param name="deleteChilds">Whether to delete child categories or to set them to no parent.</param>
        void DeleteProductCategory(ProductCategory category, bool deleteChilds = false);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">ProductCategory name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="alias">Alias to be filtered</param>
        /// <param name="applyNavigationFilters">Whether to apply <see cref="IProductCategoryNavigationFilter"/> instances to the actual categories query. Never applied when <paramref name="showHidden"/> is <c>true</c></param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Categories</returns>
        IPagedList<ProductCategory> GetAllCategories(string categoryName = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string alias = null,
            bool applyNavigationFilters = true, bool ignoreCategoriesWithoutExistingParent = true);
        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category collection</returns>
        IList<ProductCategory> GetAllProductCategoriesByParentCategoryId(int parentCategoryId, bool showHidden = false);


        IQueryable<ProductCategory> GetAllCategoriesQ();

        IList<ProductCategory> GetCategorysByIds(int[] CategoryIds);
        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Categories</returns>
        IList<ProductCategory> GetAllCategoriesDisplayedOnHomePage();

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">ProductCategory identifier</param>
        /// <returns>ProductCategory</returns>
        ProductCategory GetProductCategoryById(int categoryId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">ProductCategory</param>
        void InsertProductCategory(ProductCategory category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">ProductCategory</param>
        void UpdateProductCategory(ProductCategory category);

        /// <summary>
        /// 根据文章ID获取类别
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        ProductCategory GetProductCategoriesByArticleId(int articleId, bool showHidden = false);

        
    }

}
