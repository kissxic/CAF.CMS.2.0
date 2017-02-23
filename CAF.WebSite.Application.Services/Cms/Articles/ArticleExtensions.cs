using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Articles
{
    public static class ArticleExtensions
    {
        public static ProductVariantAttributeCombination MergeWithCombination(this Article article, string selectedAttributes)
        {
            return article.MergeWithCombination(selectedAttributes, EngineContext.Current.Resolve<IProductAttributeParser>());
        }

        public static ProductVariantAttributeCombination MergeWithCombination(this Article article, string selectedAttributes, IProductAttributeParser productAttributeParser)
        {
            Guard.ArgumentNotNull(productAttributeParser, "productAttributeParser");

            if (selectedAttributes.IsEmpty())
                return null;

            // let's find appropriate record
            var combination = productAttributeParser.FindProductVariantAttributeCombination(article.Id, selectedAttributes);

            if (combination != null && combination.IsActive)
            {
                article.MergeWithCombination(combination);
            }

            return combination;
        }

        public static void MergeWithCombination(this Article article, ProductVariantAttributeCombination combination)
        {
            Guard.ArgumentNotNull(article, "article");

            if (article.MergedDataValues != null)
                article.MergedDataValues.Clear();

            if (combination == null)
                return;

            if (article.MergedDataValues == null)
                article.MergedDataValues = new Dictionary<string, object>();


            article.MergedDataValues.Add("StockQuantity", combination.StockQuantity);


            if (combination.Sku.HasValue())
                article.MergedDataValues.Add("Sku", combination.Sku);
            if (combination.Gtin.HasValue())
                article.MergedDataValues.Add("Gtin", combination.Gtin);
            if (combination.ManufacturerPartNumber.HasValue())
                article.MergedDataValues.Add("ManufacturerPartNumber", combination.ManufacturerPartNumber);

            if (combination.Price.HasValue)
                article.MergedDataValues.Add("Price", combination.Price.Value);

            if (combination.DeliveryTimeId.HasValue && combination.DeliveryTimeId.Value > 0)
                article.MergedDataValues.Add("DeliveryTimeId", combination.DeliveryTimeId);

            if (combination.QuantityUnitId.HasValue && combination.QuantityUnitId.Value > 0)
                article.MergedDataValues.Add("QuantityUnitId", combination.QuantityUnitId);

            if (combination.Length.HasValue)
                article.MergedDataValues.Add("Length", combination.Length.Value);
            if (combination.Width.HasValue)
                article.MergedDataValues.Add("Width", combination.Width.Value);
            if (combination.Height.HasValue)
                article.MergedDataValues.Add("Height", combination.Height.Value);

            if (combination.BasePriceAmount.HasValue)
                article.MergedDataValues.Add("BasePriceAmount", combination.BasePriceAmount);
            if (combination.BasePriceBaseAmount.HasValue)
                article.MergedDataValues.Add("BasePriceBaseAmount", combination.BasePriceBaseAmount);
        }

        public static IList<int> GetAllCombinationPictureIds(this IEnumerable<ProductVariantAttributeCombination> combinations)
        {
            var pictureIds = new List<int>();

            if (combinations != null)
            {
                var data = combinations
                    .Where(x => x.IsActive && x.AssignedPictureIds != null)
                    .Select(x => x.AssignedPictureIds)
                    .ToList();

                if (data.Count > 0)
                {
                    int id;
                    var ids = string.Join(",", data).SplitSafe(",").Distinct();

                    foreach (string str in ids)
                    {
                        if (int.TryParse(str, out id) && !pictureIds.Exists(i => i == id))
                            pictureIds.Add(id);
                    }
                }
            }

            return pictureIds;
        }
        /// <summary>
        /// Get a default picture of a article 
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="pictureService">Picture service</param>
        /// <returns>Article picture</returns>
        public static Picture GetDefaultArticlePicture(this Article source, IPictureService pictureService)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (pictureService == null)
                throw new ArgumentNullException("pictureService");

            var picture = pictureService.GetPicturesByArticleId(source.Id, 1).FirstOrDefault();
            return picture;
        }


        public static bool ArticleTagExists(this Article article, int articleTagId)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            bool result = article.ArticleTags.ToList().Find(pt => pt.Id == articleTagId) != null;
            return result;
        }

        public static string GetArticleStatusLabel(this Article article, ILocalizationService localizationService)
        {
            if (article != null)
            {
                string key = "Admin.ContentManagement.Articles.ArticleStatus.{0}.Label".FormatWith(article.ArticleStatus.ToString());
                return localizationService.GetResource(key);
            }
            return "";
        }

        public static string GetArticleTypeLabel(this Article article, ILocalizationService localizationService)
        {
            if (article != null && article.ArticleType != ArticleType.Simple)
            {
                string key = "Admin.ContentManagement.Articles.ArticleType.{0}.Label".FormatWith(article.ArticleType.ToString());
                return localizationService.GetResource(key);
            }
            return "";
        }

        /// <summary>
        /// Finds a related article item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="articleId1">The first article identifier</param>
        /// <param name="articleId2">The second article identifier</param>
        /// <returns>Related article</returns>
        public static RelatedArticle FindRelatedArticle(this IList<RelatedArticle> source,
            int articleId1, int articleId2)
        {
            foreach (RelatedArticle relatedArticle in source)
                if (relatedArticle.ArticleId1 == articleId1 && relatedArticle.ArticleId2 == articleId2)
                    return relatedArticle;
            return null;
        }

     
    }
}
