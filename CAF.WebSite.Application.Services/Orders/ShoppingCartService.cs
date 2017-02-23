using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Orders;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core.Localization;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace CAF.WebSite.Application.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class ShoppingCartService : IShoppingCartService
    {
        #region Fields

        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ICurrencyService _currencyService;
        private readonly IArticleService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IUserService _userService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly ISiteMappingService _siteMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly ArticleCatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sciRepository">Shopping cart repository</param>
        /// <param name="workContext">Work context</param>
		/// <param name="siteContext">Site context</param>
        /// <param name="currencyService">Currency service</param>
        /// <param name="productService">Article settings</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="productAttributeParser">Article attribute parser</param>
        /// <param name="checkoutAttributeService">Checkout attribute service</param>
        /// <param name="checkoutAttributeParser">Checkout attribute parser</param>
        /// <param name="priceFormatter">Price formatter</param>
        /// <param name="userService">User service</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="permissionService">Permission service</param>
        /// <param name="aclService">ACL service</param>
		/// <param name="siteMappingService">Site mapping service</param>
		/// <param name="genericAttributeService">Generic attribute service</param>
        public ShoppingCartService(
            IRepository<ShoppingCartItem> sciRepository,
            IWorkContext workContext,
            ISiteContext siteContext,
            ICurrencyService currencyService,
            IArticleService productService, ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IPriceFormatter priceFormatter,
            IUserService userService,
            ShoppingCartSettings shoppingCartSettings,
            IEventPublisher eventPublisher,
            IPermissionService permissionService,
            IAclService aclService,
            ISiteMappingService siteMappingService,
            IGenericAttributeService genericAttributeService,
            IDownloadService downloadService,
            ArticleCatalogSettings catalogSettings)
        {
            this._sciRepository = sciRepository;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._currencyService = currencyService;
            this._productService = productService;
            this._localizationService = localizationService;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeService = productAttributeService;
            this._priceFormatter = priceFormatter;
            this._userService = userService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._eventPublisher = eventPublisher;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._siteMappingService = siteMappingService;
            this._genericAttributeService = genericAttributeService;
            this._downloadService = downloadService;
            this._catalogSettings = catalogSettings;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current user</param>
        /// <param name="deleteChildCartItems">A value indicating whether to delete child cart items</param>
        public virtual void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException("shoppingCartItem");

            var user = shoppingCartItem.User;
            var siteId = shoppingCartItem.SiteId;
            int cartItemId = shoppingCartItem.Id;

            //reset checkout data
            if (resetCheckoutData)
            {
                _userService.ResetCheckoutData(shoppingCartItem.User, shoppingCartItem.SiteId);
            }

            //delete item
            _sciRepository.Delete(shoppingCartItem);


            //event notification
            _eventPublisher.EntityDeleted(shoppingCartItem);

            // delete child items
            if (deleteChildCartItems)
            {
                var childCartItems = _sciRepository.Table
                    .Where(x => x.UserId == user.Id && x.ParentItemId != null && x.ParentItemId.Value == cartItemId && x.Id != cartItemId)
                    .ToList();

                foreach (var cartItem in childCartItems)
                    DeleteShoppingCartItem(cartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes, false);
            }
        }

        public virtual void DeleteShoppingCartItem(int shoppingCartItemId, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false, bool deleteChildCartItems = true)
        {
            if (shoppingCartItemId != 0)
            {
                var shoppingCartItem = _sciRepository.GetById(shoppingCartItemId);

                DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes, deleteChildCartItems);
            }
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteExpiredShoppingCartItems(DateTime olderThanUtc)
        {
            var query =
                from sci in _sciRepository.Table
                where sci.UpdatedOnUtc < olderThanUtc && sci.ParentItemId == null
                select sci;

            var cartItems = query.ToList();

            foreach (var parentItem in cartItems)
            {
                var childItems = _sciRepository.Table
                    .Where(x => x.ParentItemId != null && x.ParentItemId.Value == parentItem.Id && x.Id != parentItem.Id).ToList();

                foreach (var childItem in childItems)
                    _sciRepository.Delete(childItem);

                _sciRepository.Delete(parentItem);
            }

            return cartItems.Count;
        }


        /// <summary>
        /// Validates a product for standard properties
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Article</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetStandardWarnings(User user, ShoppingCartType shoppingCartType,
            Article product, string selectedAttributes, decimal userEnteredPrice, int quantity)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //deleted?
            if (product.Deleted)
            {
                warnings.Add(T("ShoppingCart.ArticleDeleted"));
                return warnings;
            }

            //published?
            if (product.StatusFormat != StatusFormat.Norma)
            {
                warnings.Add(T("ShoppingCart.ArticleUnpublished"));
            }

            //ACL
            if (!_aclService.Authorize(product, user))
            {
                warnings.Add(T("ShoppingCart.ArticleUnpublished"));
            }

            //Site mapping
            if (!_siteMappingService.Authorize(product, _siteContext.CurrentSite.Id))
            {
                warnings.Add(T("ShoppingCart.ArticleUnpublished"));
            }

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
            {
                warnings.Add(T("ShoppingCart.BuyingDisabled"));
            }



            //user entered price
            if (product.CustomerEntersPrice)
            {
                if (userEnteredPrice < product.MinimumCustomerEnteredPrice ||
                    userEnteredPrice > product.MaximumCustomerEnteredPrice)
                {
                    var minimumUserEnteredPrice = _currencyService.ConvertFromPrimarySiteCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    var maximumUserEnteredPrice = _currencyService.ConvertFromPrimarySiteCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);

                    warnings.Add(T("ShoppingCart.UserEnteredPrice.RangeError",
                        _priceFormatter.FormatPrice(minimumUserEnteredPrice, true, false),
                        _priceFormatter.FormatPrice(maximumUserEnteredPrice, true, false))
                    );
                }
            }

            //quantity validation
            //var hasQtyWarnings = false;
            //if (quantity < product.OrderMinimumQuantity)
            //{
            //    warnings.Add(T("ShoppingCart.MinimumQuantity", product.OrderMinimumQuantity));
            //    hasQtyWarnings = true;
            //}

            //if (quantity > product.OrderMaximumQuantity)
            //{
            //    warnings.Add(T("ShoppingCart.MaximumQuantity", product.OrderMaximumQuantity));
            //    hasQtyWarnings = true;
            //}
            return warnings;
        }

        public virtual IList<string> GetShoppingCartItemAttributeWarnings(
            User user,
            ShoppingCartType shoppingCartType,
            Article product,
            string selectedAttributes,
            int quantity = 1,
            ProductVariantAttributeCombination combination = null)
        {
            Guard.ArgumentNotNull(() => product);

            var warnings = new List<string>();

            //selected attributes
            var pva1Collection = _productAttributeParser.ParseProductVariantAttributes(selectedAttributes);
            foreach (var pva1 in pva1Collection)
            {
                var pv1 = pva1.Article;

                if (pv1 == null || pv1.Id != product.Id)
                {
                    warnings.Add(T("ShoppingCart.AttributeError"));
                    return warnings;
                }
            }

            //existing product attributes
            var pva2Collection = product.ProductVariantAttributes;
            foreach (var pva2 in pva2Collection)
            {
                if (pva2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (var pva1 in pva1Collection)
                    {
                        if (pva1.Id == pva2.Id)
                        {
                            var pvaValuesStr = _productAttributeParser.ParseValues(selectedAttributes, pva1.Id);
                            foreach (string str1 in pvaValuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        warnings.Add(T("ShoppingCart.SelectAttribute", pva2.TextPrompt.HasValue() ? pva2.TextPrompt : pva2.ProductAttribute.GetLocalized(a => a.Name)));
                    }
                }
            }

            // check if there is a selected attribute combination and if it is active
            if (warnings.Count == 0 && selectedAttributes.HasValue())
            {
                if (combination == null)
                {
                    combination = _productAttributeParser.FindProductVariantAttributeCombination(product.Id, selectedAttributes);
                }

                if (combination != null && !combination.IsActive)
                {
                    warnings.Add(T("ShoppingCart.NotAvailable"));
                }
            }

            if (warnings.Count == 0)
            {
                var pvaValues = _productAttributeParser.ParseProductVariantAttributeValues(selectedAttributes).ToList();
                foreach (var pvaValue in pvaValues)
                {
                    if (pvaValue.ValueType == ProductVariantAttributeValueType.ProductLinkage)
                    {
                        var linkedArticle = _productService.GetArticleById(pvaValue.LinkedProductId);
                        if (linkedArticle != null)
                        {
                            var linkageWarnings = GetShoppingCartItemWarnings(user, shoppingCartType, linkedArticle, _siteContext.CurrentSite.Id,
                                "", decimal.Zero, quantity * pvaValue.Quantity, false, true, true, true, true);

                            foreach (var linkageWarning in linkageWarnings)
                            {
                                warnings.Add(T("ShoppingCart.ProductLinkageAttributeWarning",
                                    pvaValue.ProductVariantAttribute.ProductAttribute.GetLocalized(a => a.Name),
                                    pvaValue.GetLocalized(a => a.Name),
                                    linkageWarning)
                                );
                            }
                        }
                        else
                        {
                            warnings.Add(T("ShoppingCart.ProductLinkageArticleNotLoading", pvaValue.LinkedProductId));
                        }
                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates if all required attributes are selected
        /// </summary>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="product">Article</param>
        /// <returns>bool</returns>
        public virtual bool AreAllAttributesForCombinationSelected(string selectedAttributes, Article product)
        {
            Guard.ArgumentNotNull(() => product);

            var hasAttributeCombinations = _sciRepository.Context.QueryForCollection(product, (Article p) => p.ProductVariantAttributeCombinations).Any();
            if (!hasAttributeCombinations)
                return true;

            //selected attributes
            var pva1Collection = _productAttributeParser.ParseProductVariantAttributes(selectedAttributes);

            //existing product attributes
            var pva2Collection = product.ProductVariantAttributes;
            foreach (var pva2 in pva2Collection)
            {
                if (pva2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (var pva1 in pva1Collection)
                    {
                        if (pva1.Id == pva2.Id)
                        {
                            var pvaValuesStr = _productAttributeParser.ParseValues(selectedAttributes, pva1.Id);
                            foreach (string str1 in pvaValuesStr)
                            {
                                if (!String.IsNullOrEmpty(str1.Trim()))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        return found;
                    }
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Article</param>
		/// <param name="siteId">Site identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
		/// <param name="automaticallyAddRequiredArticlesIfEnabled">Automatically add required products if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredArticleWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
		/// <param name="getBundleWarnings">A value indicating whether we should validate bundle and bundle items</param>
		/// <param name="bundleItem">Article bundle item if bundles should be validated</param>
		/// <param name="childItems">Child cart items to validate bundle items</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemWarnings(User user, ShoppingCartType shoppingCartType,
            Article product, int siteId,
            string selectedAttributes, decimal userEnteredPrice,
            int quantity, bool automaticallyAddRequiredArticlesIfEnabled,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredArticleWarnings = true,
            bool getBundleWarnings = true, IList<OrganizedShoppingCartItem> childItems = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(GetStandardWarnings(user, shoppingCartType, product, selectedAttributes, userEnteredPrice, quantity));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(GetShoppingCartItemAttributeWarnings(user, shoppingCartType, product, selectedAttributes, quantity));

            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
		public virtual IList<string> GetShoppingCartWarnings(IList<OrganizedShoppingCartItem> shoppingCart,
            string checkoutAttributes, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            bool hasStandartArticles = false;
            bool hasRecurringArticles = false;

            foreach (var sci in shoppingCart)
            {
                var product = sci.Item.Article;
                if (product == null)
                {
                    warnings.Add(T("ShoppingCart.CannotLoadArticle", sci.Item.ArticleId));
                    return warnings;
                }

                hasStandartArticles = true;
            }

            //don't mix standard and recurring products
            if (hasStandartArticles && hasRecurringArticles)
            {
                warnings.Add(T("ShoppingCart.CannotMixStandardAndAutoshipArticles"));
            }
            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
		/// <param name="product">Article</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">Price entered by a user</param>
        /// <returns>Found shopping cart item</returns>
		public virtual OrganizedShoppingCartItem FindShoppingCartItemInTheCart(IList<OrganizedShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            Article product,
            string selectedAttributes = "",
            decimal userEnteredPrice = decimal.Zero)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException("shoppingCart");

            if (product == null)
                throw new ArgumentNullException("product");


            foreach (var sci in shoppingCart.Where(a => a.Item.ShoppingCartType == shoppingCartType && a.Item.ParentItemId == null))
            {
                if (sci.Item.ArticleId == product.Id && sci.Item.Article.ArticleTypeId == product.ArticleTypeId)
                {
                    //attributes
                    bool attributesEqual = _productAttributeParser.AreProductAttributesEqual(sci.Item.AttributesXml, selectedAttributes);

                    //price is the same (for products which require users to enter a price)
                    var userEnteredPricesEqual = true;
                    if (sci.Item.Article.CustomerEntersPrice)
                    {
                        userEnteredPricesEqual = Math.Round(sci.Item.UserEnteredPrice, 2) == Math.Round(userEnteredPrice, 2);
                    }

                    //found?
                    if (attributesEqual && userEnteredPricesEqual)
                    {
                        return sci;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="product">The product</param>
        /// <param name="cartType">Cart type</param>
        /// <param name="siteId">Site identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="userEnteredPrice">Price entered by user</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredArticlesIfEnabled">Whether to add required products</param>
        /// <param name="ctx">Add to cart context</param>
        /// <returns>List with warnings</returns>
        public virtual List<string> AddToCart(User user, Article product, ShoppingCartType cartType, int siteId, string selectedAttributes,
            decimal userEnteredPrice, int quantity, bool automaticallyAddRequiredArticlesIfEnabled, AddToCartContext ctx = null)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            if (ctx != null && ctx.Warnings.Count > 0)
                return ctx.Warnings;    // warnings while adding bundle items to cart -> no need for further processing


            if (quantity <= 0)
            {
                warnings.Add(T("ShoppingCart.QuantityShouldPositive"));
                return warnings;
            }

            //if (parentItemId.HasValue && (parentItemId.Value == 0 || bundleItem == null || bundleItem.Id == 0))
            //{
            //	warnings.Add(T("ShoppingCart.Bundle.BundleItemNotFound", bundleItem.GetLocalizedName()));
            //	return warnings;
            //}

            //reset checkout info
            _userService.ResetCheckoutData(user, siteId);

            var cart = user.GetCartItems(cartType, siteId);
            OrganizedShoppingCartItem existingCartItem = null;


            existingCartItem = FindShoppingCartItemInTheCart(cart, cartType, product, selectedAttributes, userEnteredPrice);


            if (existingCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = existingCartItem.Item.Quantity + quantity;

                warnings.AddRange(
                    GetShoppingCartItemWarnings(user, cartType, product, siteId, selectedAttributes, userEnteredPrice, newQuantity,
                        automaticallyAddRequiredArticlesIfEnabled)
                );

                if (warnings.Count == 0)
                {
                    existingCartItem.Item.AttributesXml = selectedAttributes;
                    existingCartItem.Item.Quantity = newQuantity;
                    existingCartItem.Item.UpdatedOnUtc = DateTime.UtcNow;
                    _userService.UpdateUser(user);

                    //event notification
                    _eventPublisher.EntityUpdated(existingCartItem.Item);
                }
            }
            else
            {
                //new shopping cart item
                warnings.AddRange(
                    GetShoppingCartItemWarnings(user, cartType, product, siteId, selectedAttributes, userEnteredPrice, quantity,
                        automaticallyAddRequiredArticlesIfEnabled)
                );

                if (warnings.Count == 0)
                {
                    //maximum items validation
                    if (cartType == ShoppingCartType.ShoppingCart && cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                    {
                        warnings.Add(T("ShoppingCart.MaximumShoppingCartItems"));
                        return warnings;
                    }
                    else if (cartType == ShoppingCartType.Wishlist && cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                    {
                        warnings.Add(T("ShoppingCart.MaximumWishlistItems"));
                        return warnings;
                    }

                    var now = DateTime.UtcNow;
                    var cartItem = new ShoppingCartItem
                    {
                        ShoppingCartType = cartType,
                        SiteId = siteId,
                        Article = product,
                        AttributesXml = selectedAttributes,
                        UserEnteredPrice = userEnteredPrice,
                        Quantity = quantity,
                        CreatedOnUtc = now,
                        UpdatedOnUtc = now,
                        ParentItemId = null //parentItemId
                    };



                    if (ctx == null)
                    {
                        user.ShoppingCartItems.Add(cartItem);
                        _userService.UpdateUser(user);
                        _eventPublisher.EntityInserted(cartItem);
                    }
                    else
                    {
                        Debug.Assert(ctx.Item == null, "Add to cart item already specified");
                        ctx.Item = cartItem;

                    }
                }
            }

            return warnings;
        }

        /// <summary>
        /// Add product to cart
        /// </summary>
        /// <param name="ctx">Add to cart context</param>
        public virtual void AddToCart(AddToCartContext ctx)
        {
            var user = ctx.User ?? _workContext.CurrentUser;
            int siteId = ctx.SiteId ?? _siteContext.CurrentSite.Id;
            var cart = user.GetCartItems(ctx.CartType, siteId);

            _userService.ResetCheckoutData(user, siteId);

            if (ctx.AttributeForm != null)
            {
                var attributes = _productAttributeService.GetProductVariantAttributesByArticleId(ctx.Article.Id);

                ctx.Attributes = ctx.AttributeForm.CreateSelectedAttributesXml(ctx.Article.Id, attributes, _productAttributeParser, _localizationService,
                    _downloadService, _catalogSettings, null, ctx.Warnings, true);
            }

            ctx.Warnings.AddRange(
                AddToCart(_workContext.CurrentUser, ctx.Article, ctx.CartType, siteId, ctx.Attributes, ctx.CustomerEnteredPrice, ctx.Quantity, ctx.AddRequiredArticles, ctx)
            );



            AddToCartStoring(ctx);

        }

        /// <summary>
        /// Sites the shopping card items in the database
        /// </summary>
        /// <param name="ctx">Add to cart context</param>
        public virtual void AddToCartStoring(AddToCartContext ctx)
        {
            if (ctx.Warnings.Count == 0 && ctx.Item != null)
            {
                var user = ctx.User ?? _workContext.CurrentUser;

                user.ShoppingCartItems.Add(ctx.Item);
                _userService.UpdateUser(user);
                _eventPublisher.EntityInserted(ctx.Item);

                foreach (var childItem in ctx.ChildItems)
                {
                    childItem.ParentItemId = ctx.Item.Id;

                    user.ShoppingCartItems.Add(childItem);
                    _userService.UpdateUser(user);
                    _eventPublisher.EntityInserted(childItem);
                }
            }
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> UpdateShoppingCartItem(User user, int shoppingCartItemId, int newQuantity, bool resetCheckoutData)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var warnings = new List<string>();

            var shoppingCartItem = user.ShoppingCartItems.FirstOrDefault(sci => sci.Id == shoppingCartItemId && sci.ParentItemId == null);
            if (shoppingCartItem != null)
            {
                if (resetCheckoutData)
                {
                    //reset checkout data
                    _userService.ResetCheckoutData(user, shoppingCartItem.SiteId);
                }
                if (newQuantity > 0)
                {
                    //check warnings
                    warnings.AddRange(GetShoppingCartItemWarnings(user, shoppingCartItem.ShoppingCartType, shoppingCartItem.Article, shoppingCartItem.SiteId,
                        shoppingCartItem.AttributesXml, shoppingCartItem.UserEnteredPrice, newQuantity, false));

                    if (warnings.Count == 0)
                    {
                        //if everything is OK, then update a shopping cart item
                        shoppingCartItem.Quantity = newQuantity;
                        shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        _userService.UpdateUser(user);

                        //event notification
                        _eventPublisher.EntityUpdated(shoppingCartItem);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, true);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromUser">From user</param>
        /// <param name="toUser">To user</param>
        public virtual void MigrateShoppingCart(User fromUser, User toUser)
        {
            if (fromUser == null)
                throw new ArgumentNullException("fromUser");

            if (toUser == null)
                throw new ArgumentNullException("toUser");

            if (fromUser.Id == toUser.Id)
                return;

            int siteId = 0;
            var cartItems = fromUser.ShoppingCartItems.ToList().Organize().ToList();

            if (cartItems.Count <= 0)
                return;

            foreach (var cartItem in cartItems)
            {
                if (siteId == 0)
                    siteId = cartItem.Item.SiteId;

                Copy(cartItem, toUser, cartItem.Item.ShoppingCartType, cartItem.Item.SiteId, false);
            }

            //_eventPublisher.PublishMigrateShoppingCart(fromUser, toUser, siteId);

            foreach (var cartItem in cartItems)
            {
                DeleteShoppingCartItem(cartItem.Item);
            }
        }

        /// <summary>
        /// Copies a shopping cart item.
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="user">The user</param>
        /// <param name="cartType">Shopping cart type</param>
        /// <param name="siteId">Site Id</param>
        /// <param name="addRequiredArticlesIfEnabled">Add required products if enabled</param>
        /// <returns>List with add-to-cart warnings.</returns>
        public virtual IList<string> Copy(OrganizedShoppingCartItem sci, User user, ShoppingCartType cartType, int siteId, bool addRequiredArticlesIfEnabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (sci == null)
                throw new ArgumentNullException("item");

            var addToCartContext = new AddToCartContext
            {
                User = user
            };

            addToCartContext.Warnings = AddToCart(user, sci.Item.Article, cartType, siteId, sci.Item.AttributesXml, sci.Item.UserEnteredPrice,
                sci.Item.Quantity, addRequiredArticlesIfEnabled, addToCartContext);

            if (addToCartContext.Warnings.Count == 0 && sci.ChildItems != null)
            {
                foreach (var childItem in sci.ChildItems)
                {

                    addToCartContext.Warnings = AddToCart(user, childItem.Item.Article, cartType, siteId, childItem.Item.AttributesXml, childItem.Item.UserEnteredPrice,
                        childItem.Item.Quantity, false, addToCartContext);
                }
            }

            AddToCartStoring(addToCartContext);

            return addToCartContext.Warnings;
        }

        #endregion
    }
}
