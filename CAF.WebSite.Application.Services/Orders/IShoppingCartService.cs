using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Orders;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;


namespace CAF.WebSite.Application.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial interface IShoppingCartService
    {
        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
		/// <param name="deleteChildCartItems">A value indicating whether to delete child cart items</param>
        void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
			bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true);

		void DeleteShoppingCartItem(int shoppingCartItemId, bool resetCheckoutData = true,
			bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true);

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        int DeleteExpiredShoppingCartItems(DateTime olderThanUtc);
 
        /// <summary>
		/// Validates a article for standard properties
        /// </summary>
        /// <param name="customer">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="article">Article</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        IList<string> GetStandardWarnings(User customer, ShoppingCartType shoppingCartType,
			Article article, string selectedAttributes,
            decimal customerEnteredPrice, int quantity);

		/// <summary>
		/// Validates shopping cart item attributes
		/// </summary>
		/// <param name="customer">The customer</param>
		/// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="article">Article</param>
		/// <param name="selectedAttributes">Selected attributes</param>
		/// <param name="combination">The article variant attribute combination instance (reduces database roundtrips)</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="bundleItem">Article bundle item</param>
		/// <returns>Warnings</returns>
		IList<string> GetShoppingCartItemAttributeWarnings(
			User customer, 
			ShoppingCartType shoppingCartType,
			Article article, 
			string selectedAttributes, 
			int quantity = 1, 
			ProductVariantAttributeCombination combination = null);

      
        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="article">Article</param>
		/// <param name="storeId">Store identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
		/// <param name="automaticallyAddRequiredArticlesIfEnabled">Automatically add required articles if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a article for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate article attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
		/// <param name="getRequiredArticleWarnings">A value indicating whether we should validate required articles (articles which require other articles to be added to the cart)</param>
		/// <param name="getBundleWarnings">A value indicating whether we should validate bundle and bundle items</param>
		/// <param name="bundleItem">Article bundle item if bundles should be validated</param>
		/// <param name="childItems">Child cart items to validate bundle items</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartItemWarnings(User customer, ShoppingCartType shoppingCartType,
			Article article, int storeId, 
			string selectedAttributes, decimal customerEnteredPrice,
			int quantity, bool automaticallyAddRequiredArticlesIfEnabled,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredArticleWarnings = true,
			bool getBundleWarnings = true,   IList<OrganizedShoppingCartItem> childItems = null);

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
		IList<string> GetShoppingCartWarnings(IList<OrganizedShoppingCartItem> shoppingCart,
            string checkoutAttributes, bool validateCheckoutAttributes);

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="article">Article</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <returns>Found shopping cart item</returns>
		OrganizedShoppingCartItem FindShoppingCartItemInTheCart(IList<OrganizedShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
			Article article,
            string selectedAttributes = "",
            decimal customerEnteredPrice = decimal.Zero);

		/// <summary>
		/// Add article to cart
		/// </summary>
		/// <param name="customer">The customer</param>
		/// <param name="article">The article</param>
		/// <param name="cartType">Cart type</param>
		/// <param name="storeId">Store identifier</param>
		/// <param name="selectedAttributes">Selected attributes</param>
		/// <param name="customerEnteredPrice">Price entered by customer</param>
		/// <param name="quantity">Quantity</param>
		/// <param name="automaticallyAddRequiredArticlesIfEnabled">Whether to add required articles</param>
		/// <param name="ctx">Add to cart context</param>
		/// <returns>List with warnings</returns>
		List<string> AddToCart(User customer, Article article, ShoppingCartType cartType, int storeId, string selectedAttributes,
			decimal customerEnteredPrice, int quantity, bool automaticallyAddRequiredArticlesIfEnabled, AddToCartContext ctx = null);

		/// <summary>
		/// Add article to cart
		/// </summary>
		/// <param name="ctx">Add to cart context</param>
		void AddToCart(AddToCartContext ctx);

		/// <summary>
		/// Stores the shopping card items in the database
		/// </summary>
		/// <param name="ctx">Add to cart context</param>
		void AddToCartStoring(AddToCartContext ctx);

        /// <summary>
        /// Validates if all required attributes are selected
        /// </summary>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="article">Article</param>
        /// <returns>bool</returns>
        bool AreAllAttributesForCombinationSelected(string selectedAttributes, Article article);

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="customer">User</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        IList<string> UpdateShoppingCartItem(User customer, int shoppingCartItemId,
            int newQuantity, bool resetCheckoutData);

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromUser">From customer</param>
        /// <param name="toUser">To customer</param>
        void MigrateShoppingCart(User fromUser, User toUser);

		/// <summary>
		/// Copies a shopping cart item.
		/// </summary>
		/// <param name="sci">Shopping cart item</param>
		/// <param name="customer">The customer</param>
		/// <param name="cartType">Shopping cart type</param>
		/// <param name="storeId">Store Id</param>
		/// <param name="addRequiredArticlesIfEnabled">Add required articles if enabled</param>
		/// <returns>List with add-to-cart warnings.</returns>
		IList<string> Copy(OrganizedShoppingCartItem sci, User customer, ShoppingCartType cartType, int storeId, bool addRequiredArticlesIfEnabled);
    }
}
