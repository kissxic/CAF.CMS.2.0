using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Orders;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Application.Services.Articles;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Orders
{
    /// <summary>
    /// Represents a shopping cart
    /// </summary>
    public static class ShoppingCartExtensions
    {
        /// <summary>
        /// Indicates whether the shopping cart requires shipping
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>True if the shopping cart requires shipping; otherwise, false.</returns>
        public static bool RequiresShipping(this IList<OrganizedShoppingCartItem> shoppingCart)
        {
            foreach (var shoppingCartItem in shoppingCart)
                if (shoppingCartItem.Item.IsShipEnabled)
                    return true;
            return false;
        }

        /// <summary>
        /// Gets a number of product in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>Result</returns>
		public static int GetTotalArticles(this IEnumerable<OrganizedShoppingCartItem> shoppingCart)
        {
            return shoppingCart.Sum(x => x.Item.Quantity);
        }


        /// <summary>
        /// Get customer of shopping cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>User of shopping cart</returns>
        public static User GetUser(this IList<OrganizedShoppingCartItem> shoppingCart)
        {
            if (shoppingCart.Count == 0)
                return null;

            return shoppingCart[0].Item.User;
        }

		public static IEnumerable<ShoppingCartItem> Filter(this IEnumerable<ShoppingCartItem> shoppingCart, ShoppingCartType type, int? storeId = null)
		{
			var enumerable = shoppingCart.Where(x => x.ShoppingCartType == type);

			if (storeId.HasValue)
				enumerable = enumerable.Where(x => x.SiteId == storeId.Value);

			return enumerable;
		}
		public static IList<OrganizedShoppingCartItem> Organize(this IList<ShoppingCartItem> cart)
		{
			var result = new List<OrganizedShoppingCartItem>();
			var productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();

			if (cart == null || cart.Count <= 0)
				return result;

			foreach (var parent in cart.Where(x => x.ParentItemId == null))
			{
				var parentItem = new OrganizedShoppingCartItem(parent);

				var childs = cart.Where(x => x.ParentItemId != null && x.ParentItemId == parent.Id && x.Id != parent.Id && 
					x.ShoppingCartTypeId == parent.ShoppingCartTypeId );

				foreach (var child in childs)
				{
					var childItem = new OrganizedShoppingCartItem(child);

					if (parent.Article != null  && child.AttributesXml != null )
					{
						child.Article.MergeWithCombination(child.AttributesXml);

						var attributeValues = productAttributeParser.ParseProductVariantAttributeValues(child.AttributesXml).ToList();
						
					}

					parentItem.ChildItems.Add(childItem);
				}

				result.Add(parentItem);
			}

			return result;
		}
    }
}
