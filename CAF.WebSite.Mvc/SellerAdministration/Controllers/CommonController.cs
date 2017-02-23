using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Async;
using CAF.Infrastructure.Core.Collections;
using CAF.Infrastructure.Core.Data;
using CAF.Infrastructure.Core.Plugins;
using CAF.Infrastructure.Core.Utilities;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Security;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Security;
using CAF.Infrastructure.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Mvc.Seller.Models.Common;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.Services.Members;

namespace CAF.WebSite.Mvc.Seller.Controllers
{
    public class CommonController : SellerAdminControllerBase
    {
        #region Fields

        private readonly IWebHelper _webHelper;

        private readonly Lazy<SecuritySettings> _securitySettings;
        private readonly Lazy<IMenuPublisher> _menuPublisher;
        private readonly IDbContext _dbContext;
        private readonly Func<string, ICacheManager> _cache;
        private readonly IArticleService _articleService;
        private readonly IPermissionService _permissionService;
        private readonly ICommonServices _services;
        private readonly SiteInformationSettings _siteSettings;
        private readonly IChannelService _channelService;
        private readonly IArticleCategoryService _categoryService;
        private readonly IMemberGradeMappingService _memberGradeMappingService;
        private readonly static object _lock = new object();
        private readonly static object s_lock = new object();
        #endregion

        #region Constructors

        public CommonController(
            IWebHelper webHelper,
            Lazy<SecuritySettings> securitySettings,
            Lazy<IMenuPublisher> menuPublisher,
            IPermissionService permissionService,
            IArticleService articleService,
            ICommonServices services,
            IDbContext dbContext,
            SiteInformationSettings siteSettings,
            IArticleCategoryService categoryService,
            IChannelService channelService,
            IMemberGradeMappingService memberGradeMappingService,
            Func<string, ICacheManager> cache)
        {
            this._webHelper = webHelper;
            this._securitySettings = securitySettings;
            this._menuPublisher = menuPublisher;
            this._dbContext = dbContext;
            this._cache = cache;
            this._services = services;
            this._articleService = articleService;
            this._siteSettings = siteSettings;
            this._categoryService = categoryService;
            this._channelService = channelService;
            this._permissionService = permissionService;
            this._memberGradeMappingService = memberGradeMappingService;
        }

        #endregion

        #region Methods

        #region Navbar & Menu
        [ChildActionOnly]
        public ActionResult Navbar()
        {
            var currentUser = _services.WorkContext.CurrentUser;

            ViewBag.UserName = _services.Settings.LoadSetting<UserSettings>().UserNamesEnabled ? currentUser.UserName : currentUser.Email;
            ViewBag.Sites = _services.SiteService.GetAllSites();
            ViewBag.SiteContentShare = this._siteSettings.SiteContentShare;
            if (_services.Permissions.Authorize(StandardPermissionProvider.ManageMaintenance))
            {
                // ViewBag.CheckUpdateResult = AsyncRunner.RunSync(() => CheckUpdateAsync(false));
            }

            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var cacheManager = _services.Cache;

            var customerRolesIds = _services.WorkContext.CurrentUser.UserRoles.Where(x => x.Active).Select(x => x.Id).ToList();
            string cacheKey = string.Format("cafsite.pres.selleradminmenu.navigation-{0}-{1}", _services.WorkContext.WorkingLanguage.Id, string.Join(",", customerRolesIds));

            var rootNode = cacheManager.Get(cacheKey, () =>
            {
                lock (s_lock)
                {
                    return PrepareSellerMenu();
                }
            });

            return PartialView(rootNode);
        }

        private TreeNode<MenuItem> PrepareSellerMenu()
        {
            XmlSiteMap siteMap = new XmlSiteMap(this._cache("static"));
            siteMap.LoadFrom(_webHelper.MapPath("~/SellerAdministration/sitemap.config"));

            var rootNode = ConvertSitemapNodeToMenuItemNode(siteMap.RootNode);

            _menuPublisher.Value.RegisterMenus(rootNode, "selleradmin");

            // hide based on permissions
            rootNode.TraverseTree(x =>
            {
                if (!x.IsRoot)
                {
                    if (!MenuItemAccessPermitted(x.Value))
                    {
                        x.Value.Visible = false;
                    }
                }
            });

            // hide dropdown nodes when no child is visible
            rootNode.TraverseTree(x =>
            {
                if (!x.IsRoot)
                {
                    var item = x.Value;
                    if (!item.IsGroupHeader && !item.HasRoute())
                    {
                        if (!x.Children.Any(child => child.Value.Visible))
                        {
                            item.Visible = false;
                        }
                    }
                }
            });

            return rootNode;
        }

        private TreeNode<MenuItem> ConvertSitemapNodeToMenuItemNode(SiteMapNode node)
        {
            var item = new MenuItem();
            var treeNode = new TreeNode<MenuItem>(item);

            if (node.RouteName.HasValue())
            {
                item.RouteName = node.RouteName;
            }
            else if (node.ActionName.HasValue() && node.ControllerName.HasValue())
            {
                item.ActionName = node.ActionName;
                item.ControllerName = node.ControllerName;
            }
            else if (node.Url.HasValue())
            {
                item.Url = node.Url;
            }
            item.RouteValues = node.RouteValues;

            item.Visible = node.Visible;
            item.Text = node.Title;
            item.Attributes.Merge(node.Attributes);

            if (node.Attributes.ContainsKey("permissionNames"))
                item.PermissionNames = node.Attributes["permissionNames"] as string;

            if (node.Attributes.ContainsKey("id"))
                item.Id = node.Attributes["id"] as string;

            if (node.Attributes.ContainsKey("resKey"))
                item.ResKey = node.Attributes["resKey"] as string;

            if (node.Attributes.ContainsKey("iconClass"))
                item.Icon = node.Attributes["iconClass"] as string;

            if (node.Attributes.ContainsKey("imageUrl"))
                item.ImageUrl = node.Attributes["imageUrl"] as string;

            if (node.Attributes.ContainsKey("isGroupHeader"))
                item.IsGroupHeader = Boolean.Parse(node.Attributes["isGroupHeader"] as string);

            // iterate children recursively
            foreach (var childNode in node.ChildNodes)
            {
                var childTreeNode = ConvertSitemapNodeToMenuItemNode(childNode);
                treeNode.Append(childTreeNode);
            }

            return treeNode;
        }

        private bool MenuItemAccessPermitted(MenuItem item)
        {
            var result = true;

            if (_securitySettings.Value.HideAdminMenuItemsBasedOnPermissions && item.PermissionNames.HasValue())
            {
                var permitted = item.PermissionNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(x => _services.Permissions.Authorize(x.Trim()));
                if (!permitted)
                {
                    result = false;
                }
            }

            return result;
        }

        public ActionResult CategoryMenu()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageArticles))
                return AccessDeniedView();
            var allChannels = _channelService.GetAllChannels();

            #region categories
            var allCategories = _categoryService.GetAllCategories(showHidden: false);

            var menuModels = new List<MenuModel>();
            foreach (var item in allChannels)
            {
                if (_memberGradeMappingService.Authorize(item))
                {
                    var menuModel = new MenuModel();
                    menuModel.Text = item.Title;
                    menuModel.Id = item.Id.ToString();
                    menuModel.IconName = item.IconName;
                    menuModel.Href = Url.Action("List", "Article", new { area = "seller", SearchChannelId = item.Id });
                    menuModels.Add(menuModel);
                }
            }
            #endregion
            
            return PartialView(menuModels);
        }

        private void ConvertMenuItemNode(MenuModel node, List<ArticleCategory> allCategroySource, List<ArticleCategory> categroySource)
        {
            foreach (var childNode in categroySource)
            {
                var menuItemModel = new MenuModel();
                menuItemModel.Text = childNode.Name;
                menuItemModel.Id = childNode.Id.ToString();
                menuItemModel.Href = Url.Action("List", new { SearchCategoryId = childNode.Id });
                node.Childitems.Add(menuItemModel);
                var childCategory = allCategroySource.Where(p => p.ParentCategoryId == childNode.Id).ToList();
                if (childCategory.Count > 0)
                    ConvertMenuItemNode(menuItemModel, allCategroySource, childCategory);
            }
        }

        #endregion

        [HttpPost]
        public JsonResult SetSelectedTab(string navId, string tabId, string path)
        {
            if (navId.HasValue() && tabId.HasValue() && path.HasValue())
            {
                var info = new SelectedTabInfo { TabId = tabId, Path = path };
                TempData["SelectedTab." + navId] = info;
            }
            return Json(new { Success = true });
        }

        public ActionResult SetSelectedSite(int? siteId, string returnUrl)
        {
            using (HttpContext.PreviewModeCookie())
            {
                _services.SiteContext.SetPreviewSite(siteId);
            }
            if (returnUrl.IsEmpty() && Request.UrlReferrer != null && Request.UrlReferrer.ToString().Length > 0)
            {
                returnUrl = Request.UrlReferrer.ToString();
            }

            return Redirect(returnUrl);
        }


        #endregion
    }
}