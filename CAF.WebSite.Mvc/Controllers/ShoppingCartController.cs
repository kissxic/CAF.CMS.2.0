using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Orders;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Orders;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Security;
using CAF.WebSite.Mvc.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.WebUI.Controllers;

namespace CAF.WebSite.Mvc.Controllers
{
    public class ShoppingCartController : PublicControllerBase
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IArticleService _productService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWebHelper _webHelper;
        private readonly IUserActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ICommonServices _services;

        private readonly IProductAttributeParser _productAttributeParser;
        #endregion

        #region Constructors
        public ShoppingCartController(IUserService userService,
            IArticleService productService,
            ILocalizationService localizationService,
            ShoppingCartSettings shoppingCartSettings,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext, ISiteContext storeContext,
            ICurrencyService currencyService, IWebHelper webHelper,
            IUserActivityService customerActivityService,
            ICommonServices services,
            IProductAttributeParser productAttributeParser
            )
        {
            this._userService = userService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._localizationService = localizationService;
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._shoppingCartService = shoppingCartService;
            this._currencyService = currencyService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._services = services;
            this._productAttributeParser = productAttributeParser;
        }
        #endregion

        #region Utilities 
        [NonAction]
        protected void PrepareWishlistModel(WishlistModel model, IList<OrganizedShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;

            if (cart.Count == 0)
                return;

            #region Simple properties

            var customer = cart.FirstOrDefault().Item.User;
            model.CustomerGuid = customer.UserGuid;
            model.CustomerFullname = customer.UserName;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowProductBundleImages = _shoppingCartSettings.ShowProductBundleImagesOnShoppingCart;
            model.ShowItemsFromWishlistToCartButton = _shoppingCartSettings.ShowItemsFromWishlistToCartButton;

            model.DisplayShortDesc = _shoppingCartSettings.ShowShortDesc;


            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var wishlistCartItemModel = PrepareWishlistCartItemModel(sci);

                model.Items.Add(wishlistCartItemModel);
            }

            #endregion
        }

     

        private WishlistModel.ShoppingCartItemModel PrepareWishlistCartItemModel(OrganizedShoppingCartItem sci)
        {
            var item = sci.Item;
            var product = sci.Item.Article;

            product.MergeWithCombination(item.AttributesXml);

            var model = new WishlistModel.ShoppingCartItemModel
            {
                Id = item.Id,
                Sku = product.Sku,
                ProductId = product.Id,
                ProductName = product.GetLocalized(x => x.Title),
                ProductSeName = product.GetSeName(),
                Quantity = item.Quantity,
                ShortDesc = product.GetLocalized(x => x.ShortContent),
                ProductType = product.ArticleType,
            };
            model.ProductUrl = GetProductUrlWithAttributes(sci, model.ProductSeName);
 
            if (sci.ChildItems != null)
            {
                foreach (var childItem in sci.ChildItems.Where(x => x.Item.Id != item.Id))
                {
                    var childModel = PrepareWishlistCartItemModel(childItem);

                    model.ChildItems.Add(childModel);
                }
            }

            return model;
        }

        private string GetProductUrlWithAttributes(OrganizedShoppingCartItem cartItem, string productSeName)
        {
            var attributeQueryData = new List<List<int>>();
            var product = cartItem.Item.Article;

            if (product.ArticleType == ArticleType.Simple)
            {
                _productAttributeParser.DeserializeQueryData(attributeQueryData, cartItem.Item.AttributesXml, product.Id);
            }
            var url = _productAttributeParser.GetProductUrlWithAttributes(attributeQueryData, productSeName);
            return url;
        }
        #endregion

        #region Shopping cart
        //add product to cart using AJAX
        //currently we use this method on the product details pages
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProduct(int productId, int shoppingCartTypeId, FormCollection form)
        {
            var product = _productService.GetArticleById(productId);
            if (product == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("HomePage"),
                });
            }

            #region User entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimal customerEnteredPrice = decimal.Zero;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimarySiteCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            #endregion

            #region Quantity

            int quantity = 1;
            string key1 = "addtocart_{0}.EnteredQuantity".FormatWith(productId);
            string key2 = "addtocart_{0}.AddToCart.EnteredQuantity".FormatWith(productId);

            if (form.AllKeys.Contains(key1))
                int.TryParse(form[key1], out quantity);
            else if (form.AllKeys.Contains(key2))
                int.TryParse(form[key2], out quantity);

            #endregion

            //save item
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var addToCartContext = new AddToCartContext
            {
                Article = product,
                AttributeForm = form,
                CartType = cartType,
                CustomerEnteredPrice = customerEnteredPriceConverted,
                Quantity = quantity,
                AddRequiredArticles = true
            };

            _shoppingCartService.AddToCart(addToCartContext);

            #region Return result

            if (addToCartContext.Warnings.Count > 0)
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartContext.Warnings.ToArray()
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Title);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist"),
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            });
                        }
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Title);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart"),
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                success = true,
                                message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart"))
                            });
                        }
                    }
            }

            #endregion
        }

        #endregion

        #region Wishlist

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Wishlist(Guid? customerGuid)
        {
            //if (!_services.Permissions.Authorize(StandardPermissionProvider.EnableWishlist))
            //    return RedirectToRoute("HomePage");

            var customer = customerGuid.HasValue ? _userService.GetUserByGuid(customerGuid.Value) : _workContext.CurrentUser;
            if (customer == null)
                return RedirectToRoute("HomePage");

            var cart = customer.GetCartItems(ShoppingCartType.Wishlist, _storeContext.CurrentSite.Id);
            var model = new WishlistModel();

            PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return View(model);
        }

       

        //remove a certain wishlist cart item on the page
        [ValidateInput(false)]
        [HttpPost, ActionName("Wishlist")]
        [FormValueRequired(FormValueRequirement.StartsWith, "removefromcart-")]
        public ActionResult RemoveWishlistItem(FormCollection form)
        {
            

            //get wishlist cart item identifier
            int sciId = 0;
            foreach (var formValue in form.AllKeys)
            {
                if (formValue.StartsWith("removefromcart-", StringComparison.InvariantCultureIgnoreCase))
                    sciId = Convert.ToInt32(formValue.Substring("removefromcart-".Length));
            }

            //get wishlist cart item
            var cart = _workContext.CurrentUser.GetCartItems(ShoppingCartType.Wishlist, _storeContext.CurrentSite.Id);

            var sci = cart.FirstOrDefault(x => x.Item.Id == sciId);
            if (sci == null)
            {
                return RedirectToRoute("Wishlist");
            }

            //remove the wishlist cart item
            _shoppingCartService.DeleteShoppingCartItem(sci.Item);

            //updated wishlist
            cart = _workContext.CurrentUser.GetCartItems(ShoppingCartType.Wishlist, _storeContext.CurrentSite.Id);
            var model = new WishlistModel();

            PrepareWishlistModel(model, cart);

            return View(model);
        }

       

        #endregion
    }
}