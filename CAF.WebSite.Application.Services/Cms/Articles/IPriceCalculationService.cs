using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Users;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Articles
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial interface IPriceCalculationService
    {
        /// <summary>
        /// Get product special price (is valid)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product special price</returns>
        decimal? GetSpecialPrice(Article article);

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        decimal GetFinalPrice(Article article, bool includeDiscounts);

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        decimal GetFinalPrice(Article article,
            User user,
            bool includeDiscounts);


        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        decimal GetFinalPrice(Article article,
            User user,
            decimal additionalCharge,
            bool includeDiscounts,
              int quantity);




        /// <summary>
        /// Gets the price adjustment of a variant attribute value
        /// </summary>
        /// <param name="attributeValue">Product variant attribute value</param>
        /// <returns>Price adjustment of a variant attribute value</returns>
        decimal GetProductVariantAttributeValuePriceAdjustment(ProductVariantAttributeValue attributeValue);
    }
}
