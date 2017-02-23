using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ProductCategoryExtensions
    {
        /// <summary>
        /// Sort categories for tree representation
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="parentId">Parent category identifier</param>
        /// <param name="ignoreCategoriesWithoutExistingParent">A value indicating whether categories without parent category in provided category list (source) should be ignored</param>
        /// <returns>Sorted categories</returns>
        public static IList<ProductCategory> SortCategoriesForTree(this IList<ProductCategory> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<ProductCategory>();

            var categories = source.ToList().FindAll(c => c.ParentCategoryId == parentId);
            foreach (var cat in categories)
            {
                result.Add(cat);
                result.AddRange(SortCategoriesForTree(source, cat.Id, true));
            }
            if (!ignoreCategoriesWithoutExistingParent && result.Count != source.Count)
            {
                // find categories without parent in provided category source and insert them into result
                foreach (var cat in source)
                    if (result.Where(x => x.Id == cat.Id).FirstOrDefault() == null)
                        result.Add(cat);
            }
            return result;
        }

        public static IList<int> CategoriesForTree(this IList<ProductCategory> source, int parentId = 0, bool ignoreCategoriesWithoutExistingParent = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var result = new List<int>();

            var categories = source.ToList().FindAll(c => c.ParentCategoryId == parentId);
            foreach (var cat in categories)
            {
                result.Add(cat.Id);
                result.AddRange(CategoriesForTree(source, cat.Id, true));
            }
            if (!ignoreCategoriesWithoutExistingParent && result.Count != source.Count)
            {
                // find categories without parent in provided category source and insert them into result
                foreach (var cat in source)
                    if (result.Where(x => x == cat.Id).FirstOrDefault() == null)
                        result.Add(cat.Id);
            }
            return result;
        }

        public static string GetFullCategoryName(this ProductCategory category)
		{
			if (category != null)
			{
                if (category.Alias.HasValue())
                    return "{0} ({1})".FormatWith(category.Name, category.Alias);
				else
                    return category.Name;
			}
			return null;
		}

        public static string GetCategoryNameWithPrefix(this ProductCategory category, IProductCategoryService categoryService, IDictionary<int, ProductCategory> mappedCategories = null)
        {
            string result = string.Empty;

            while (category != null)
            {
                if (String.IsNullOrEmpty(result))
                {
                    result = category.GetFullCategoryName();
                }
                else
                {
                    result = "--" + result;
                }

                int parentId = category.ParentCategoryId;
                if (mappedCategories == null)
                {
                    category = categoryService.GetProductCategoryById(parentId);
                }
                else
                {
                    category = mappedCategories.ContainsKey(parentId) ? mappedCategories[parentId] : categoryService.GetProductCategoryById(parentId);
                }
            }
            return result;
        }

        public static string GetCategoryBreadCrumb(this ProductCategory category, IProductCategoryService categoryService, IDictionary<int, ProductCategory> mappedCategories = null)
        {
            string result = string.Empty;

            while (category != null )
            {
				// codehint: sm-edit
                if (String.IsNullOrEmpty(result))
                {
                    result = category.GetFullCategoryName();
                }
                else
                {
                    result = category.GetFullCategoryName() + " >> " + result;
                }

                int parentId = category.ParentCategoryId;
                if (mappedCategories == null)
                {
                    category = categoryService.GetProductCategoryById(parentId);
                }
                else
                {
                    category = mappedCategories.ContainsKey(parentId) ? mappedCategories[parentId] : categoryService.GetProductCategoryById(parentId);
                }
            }

            return result;
        }

    }
}
