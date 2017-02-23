using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Pages;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CAF.Infrastructure.Core.Caching;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// ProductCategory service
    /// </summary>
    public partial class ProductCategoryService : IProductCategoryService
    {
        #region Constants

        private const string CATEGORIES_BY_PARENT_CATEGORY_ID_KEY = "cms.paroductcategory.byparent-{0}-{1}-{2}-{3}";
        private const string ARTICLECATEGORIES_ALLBYCATEGORYID_KEY = "cms.productcategory.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}";
        private const string ARTICLECATEGORIES_ALLBYARTICLEID_KEY = "cms.productcategory.allbyarticleid-{0}-{1}-{2}-{3}";
        private const string CATEGORIES_PATTERN_KEY = "cms.category.";
        private const string ARTICLECATEGORIES_PATTERN_KEY = "cms.paroductcategory.";
        private const string CATEGORIES_BY_ID_KEY = "cms.paroductcategory.id-{0}";

        #endregion

        #region Fields

        private readonly IRepository<ProductCategory> _categoryRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<SiteMapping> _siteMappingRepository;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRequestCache _requestCache;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IAclService _aclService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="requestCache">Cache manager</param>
        /// <param name="categoryRepository">ProductCategory repository</param>
        /// <param name="articleProductCategoryRepository">ProductProductCategory repository</param>
        /// <param name="articleRepository">Product repository</param>
        /// <param name="aclRepository">ACL record repository</param>
        /// <param name="siteMappingRepository">Site mapping repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="siteContext">Site context</param>
        /// <param name="eventPublisher">Event publisher</param>
        public ProductCategoryService(IRequestCache requestCache,
            IRepository<ProductCategory> categoryRepository,
            IRepository<Article> articleRepository,
            IRepository<SiteMapping> siteMappingRepository,
                 IRepository<AclRecord> aclRepository,
            IWorkContext workContext,
            ISiteContext siteContext,
            IEventPublisher eventPublisher,
            IAclService aclService,
            ISiteMappingService siteMappingService)
        {
            this._requestCache = requestCache;
            this._categoryRepository = categoryRepository;
            this._articleRepository = articleRepository;
            this._siteMappingRepository = siteMappingRepository;
            this._aclRepository = aclRepository;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._eventPublisher = eventPublisher;
            this._siteMappingService = siteMappingService;
            this._aclService = aclService;
            this.QuerySettings = DbQuerySettings.Default;
        }

        public DbQuerySettings QuerySettings { get; set; }

        #endregion

        #region Utilities

        private void DeleteAllCategories(IList<ProductCategory> categories, bool delete)
        {
            foreach (var category in categories)
            {
                //if (delete)
                //    category.Deleted = true;

                //UpdateProductCategory(category);

                var childCategories = GetAllProductCategoriesByParentCategoryId(category.Id, true);
                DeleteAllCategories(childCategories, delete);
                _categoryRepository.Delete(category);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">ProductCategory</param>
        /// <param name="deleteChilds">Whether to delete child categories or to set them to no parent.</param>
        public virtual void DeleteProductCategory(ProductCategory category, bool deleteChilds = false)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            //category.Deleted = true;
            //UpdateProductCategory(category);

            var childCategories = GetAllProductCategoriesByParentCategoryId(category.Id, true);
            DeleteAllCategories(childCategories, deleteChilds);

            _categoryRepository.Delete(category);
        }

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
        public virtual IPagedList<ProductCategory> GetAllCategories(string categoryName = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string alias = null,
            bool applyNavigationFilters = true, bool ignoreCategoriesWithoutExistingParent = true)
        {
            var query = _categoryRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!String.IsNullOrWhiteSpace(categoryName))
                query = query.Where(c => c.Name.Contains(categoryName));
            if (!String.IsNullOrWhiteSpace(alias))
                query = query.Where(c => c.Alias.Contains(alias));

            query = query.OrderBy(c => c.Id).ThenBy(c => c.DisplayOrder);


            var unsortedCategories = query.ToList();

            // sort categories
            var sortedCategories = unsortedCategories.SortCategoriesForTree(ignoreCategoriesWithoutExistingParent: ignoreCategoriesWithoutExistingParent);

            // paging
            return new PagedList<ProductCategory>(sortedCategories, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentProductCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>ProductCategory collection</returns>
        public IList<ProductCategory> GetAllProductCategoriesByParentCategoryId(int parentProductCategoryId, bool showHidden = false)
        {
            string key = string.Format(CATEGORIES_BY_PARENT_CATEGORY_ID_KEY, parentProductCategoryId, showHidden, _workContext.CurrentUser.Id, _siteContext.CurrentSite.Id);
            return _requestCache.Get(key, () =>
            {
                var query = _categoryRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.Where(c => c.ParentCategoryId == parentProductCategoryId);

                query = query.OrderBy(c => c.DisplayOrder);

                if (!showHidden)
                {
                    query = ApplyHiddenCategoriesFilter(query, false);
                    query = query.OrderBy(c => c.DisplayOrder);
                }

                var categories = query.ToList();
                return categories;
            });

        }


        protected virtual IQueryable<ProductCategory> ApplyHiddenCategoriesFilter(IQueryable<ProductCategory> query, bool applyNavigationFilters)
        {
            //only distinct categories (group by ID)
            query = from c in query
                    group c by c.Id into cGroup
                    orderby cGroup.Key
                    select cGroup.FirstOrDefault();


            return query;
        }



        public IQueryable<ProductCategory> GetAllCategoriesQ()
        {
            var query = _categoryRepository.Table;
            return query;
        }

        public IList<ProductCategory> GetCategorysByIds(int[] CategoryIds)
        {
            if (CategoryIds == null || CategoryIds.Length == 0)
                return new List<ProductCategory>();

            var query = from c in _categoryRepository.Table
                        where CategoryIds.Contains(c.Id)
                        select c;
            var Categorys = query.ToList();
            //sort by passed identifiers
            var sortedCategory = new List<ProductCategory>();
            foreach (int id in CategoryIds)
            {
                var Category = Categorys.Find(x => x.Id == id);
                if (Category != null)
                    sortedCategory.Add(Category);
            }
            return sortedCategory;
        }

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <returns>Categories</returns>
        public virtual IList<ProductCategory> GetAllCategoriesDisplayedOnHomePage()
        {
            var query = from c in _categoryRepository.Table
                        orderby c.DisplayOrder
                        where c.Published
                        select c;

            var categories = query.ToList();
            return categories;
        }

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">ProductCategory identifier</param>
        /// <returns>ProductCategory</returns>
        public virtual ProductCategory GetProductCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            string key = string.Format(CATEGORIES_BY_ID_KEY, categoryId);
            return _requestCache.Get(key, () =>
            {
                return _categoryRepository.GetById(categoryId);
            });
        }

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">ProductCategory</param>
        public virtual void InsertProductCategory(ProductCategory category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            _categoryRepository.Insert(category);

            //cache
            _requestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _requestCache.RemoveByPattern(ARTICLECATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(category);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">ProductCategory</param>
        public virtual void UpdateProductCategory(ProductCategory category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            //validate category hierarchy
            var parentProductCategory = GetProductCategoryById(category.ParentCategoryId);
            while (parentProductCategory != null)
            {
                if (category.Id == parentProductCategory.Id)
                {
                    category.ParentCategoryId = 0;
                    break;
                }
                parentProductCategory = GetProductCategoryById(parentProductCategory.ParentCategoryId);
            }

            _categoryRepository.Update(category);

            //cache
            _requestCache.RemoveByPattern(CATEGORIES_PATTERN_KEY);
            _requestCache.RemoveByPattern(ARTICLECATEGORIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(category);
        }

        /// <summary>
        /// Gets a article category mapping collection
        /// </summary>
        /// <param name="articleId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Product category mapping collection</returns>
        public virtual ProductCategory GetProductCategoriesByArticleId(int articleId, bool showHidden = false)
        {
            if (articleId == 0)
                return new ProductCategory();

            string key = string.Format(ARTICLECATEGORIES_ALLBYARTICLEID_KEY, showHidden, articleId, _workContext.CurrentUser.Id, _siteContext.CurrentSite.Id);
            return _requestCache.Get(key, () =>
            {

                var query = from c in _categoryRepository.Table
                            join a in _articleRepository.Table on c.Id equals a.ProductCategoryId
                            where a.Id == articleId &&
                                  (showHidden || c.Published)
                            orderby a.DisplayOrder
                            select c;

                var result = query.FirstOrDefault();


                return result;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
       
        
        #endregion
    }
}
