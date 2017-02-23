using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial class PriceCalculationService : IPriceCalculationService
    {
        private readonly IArticleService _articleService;
        private readonly IArticleExtendedAttributeService _articleAttributeService;
        private readonly ICommonServices _services;
        private readonly HttpRequestBase _httpRequestBase;


        public PriceCalculationService(
            IArticleService articleService,

            IArticleExtendedAttributeService articleAttributeService,

            ICommonServices services,
            HttpRequestBase httpRequestBase)
        {
            this._articleService = articleService;
            this._articleAttributeService = articleAttributeService;
            this._services = services;
            this._httpRequestBase = httpRequestBase;
        }



        #region Methods
        /// <summary>
        /// Get article special price (is valid)
        /// </summary>
        /// <param name="article">Article</param>
        /// <returns>Article special price</returns>
        public virtual decimal? GetSpecialPrice(Article article)
        {
            if (article == null)
                throw new ArgumentNullException("article");

            if (!article.SpecialPrice.HasValue)
                return null;

            //check date range
            DateTime now = DateTime.UtcNow;
            if (article.SpecialPriceStartDateTimeUtc.HasValue)
            {
                DateTime startDate = DateTime.SpecifyKind(article.SpecialPriceStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (startDate.CompareTo(now) > 0)
                    return null;
            }
            if (article.SpecialPriceEndDateTimeUtc.HasValue)
            {
                DateTime endDate = DateTime.SpecifyKind(article.SpecialPriceEndDateTimeUtc.Value, DateTimeKind.Utc);
                if (endDate.CompareTo(now) < 0)
                    return null;
            }

            return article.SpecialPrice.Value;
        }
        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Article article,
            bool includeDiscounts)
        {
            var user = _services.WorkContext.CurrentUser;
            return GetFinalPrice(article, user, includeDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public virtual decimal GetFinalPrice(Article article,
            User user,
            bool includeDiscounts)
        {
            return GetFinalPrice(article, user, decimal.Zero, includeDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
		/// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
		public virtual decimal GetFinalPrice(Article article,
            User user,
            decimal additionalCharge,
            bool includeDiscounts)
        {
            return GetFinalPrice(article, user, additionalCharge, includeDiscounts, 1);
        }
     

        public virtual decimal GetFinalPrice(
              Article article,
              User user,
              decimal additionalCharge,
              bool includeDiscounts,
              int quantity)
        {
            //initial price
            decimal result = article.Price;

            //special price
            var specialPrice = GetSpecialPrice(article);
            if (specialPrice.HasValue)
                result = specialPrice.Value;

            //tier prices


            //discount + additional charge
            //discount + additional charge
            if (includeDiscounts)
            {
                //Discount appliedDiscount = null;
                //decimal discountAmount = GetDiscountAmount(product, user, additionalCharge, quantity, out appliedDiscount, bundleItem, context);
                //result = result + additionalCharge - discountAmount;
            }
            else
            {
                result = result + additionalCharge;
            }
            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
		/// Gets the price adjustment of a variant attribute value
		/// </summary>
		/// <param name="attributeValue">Product variant attribute value</param>
		/// <returns>Price adjustment of a variant attribute value</returns>
		public virtual decimal GetProductVariantAttributeValuePriceAdjustment(ProductVariantAttributeValue attributeValue)
        {
            if (attributeValue == null)
                throw new ArgumentNullException("attributeValue");

            if (attributeValue.ValueType == ProductVariantAttributeValueType.Simple)
                return attributeValue.PriceAdjustment;

            if (attributeValue.ValueType == ProductVariantAttributeValueType.ProductLinkage)
            {
                var linkedProduct = _articleService.GetArticleById(attributeValue.LinkedProductId);

                if (linkedProduct != null)
                {
                    var productPrice = GetFinalPrice(linkedProduct, true) * attributeValue.Quantity;
                    return productPrice;
                }
            }
            return decimal.Zero;
        }
        #endregion
    }
}
