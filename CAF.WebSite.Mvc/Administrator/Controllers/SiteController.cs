using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Sites;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Directory;
using CAF.Mvc.JQuery.Datatables.Models;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public partial class SiteController : AdminControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ICurrencyService _currencyService;

        public SiteController(ISiteService siteService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ICurrencyService currencyService)
        {
            this._siteService = siteService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._currencyService = currencyService;
        }


        private void PrepareSiteModel(SiteModel model, Site site)
        {
            model.AvailableCurrencies = _currencyService.GetAllCurrencies(false, site == null ? 0 : site.Id)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();
        }


        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        public ActionResult List()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            return View();
        }

        public ActionResult AllSites(string label, int selectedId = 0)
        {
            var sites = _siteService.GetAllSites();

            if (label.HasValue())
            {
                sites.Insert(0, new Site { Name = label, Id = 0 });
            }

            var list =
                from m in sites
                select new
                {
                    id = m.Id.ToString(),
                    text = m.Name,
                    selected = m.Id == selectedId
                };

            return new JsonResult { Data = list.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();
            var siteModels = _siteService.GetAllSites().Select(x =>
            {
                var model = x.ToModel();

                PrepareSiteModel(model, x);

                model.Hosts = model.Hosts.EmptyNull().Replace(",", "<br />");

                return model;
            })
                    .ToList();
            var total = siteModels.Count;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = siteModels.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult Create()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var model = new SiteModel();
            PrepareSiteModel(model, null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(SiteModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var site = model.ToEntity();
                //ensure we have "/" at the end
                site.Url = site.Url.EnsureEndsWith("/");
                site.AddEntitySysParam();
                _siteService.InsertSite(site);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Sites.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = site.Id }) : RedirectToAction("List");
            }
            PrepareSiteModel(model, null);
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var site = _siteService.GetSiteById(id);
            if (site == null)
                //No site found with the specified id
                return RedirectToAction("List");

            var model = site.ToModel();
       
            PrepareSiteModel(model, null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(SiteModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var site = _siteService.GetSiteById(model.Id);
            if (site == null)
                //No site found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                site = model.ToEntity(site);
                //ensure we have "/" at the end
                site.Url = site.Url.EnsureEndsWith("/");
                site.AddEntitySysParam(false, true);
                _siteService.UpdateSite(site);

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Sites.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = site.Id }) : RedirectToAction("List");
            }
            PrepareSiteModel(model, null);
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSites))
            //    return AccessDeniedView();

            var site = _siteService.GetSiteById(id);
            if (site == null)
                //No site found with the specified id
                return RedirectToAction("List");

            try
            {
                _siteService.DeleteSite(site);

                //when we delete a site we should also ensure that all "per site" settings will also be deleted
                var settingsToDelete = _settingService
                    .GetAllSettings()
                    .Where(s => s.SiteId == id)
                    .ToList();
                foreach (var setting in settingsToDelete)
                    _settingService.DeleteSetting(setting);
                //when we had two sites and now have only one site, we also should delete all "per site" settings
                var allSites = _siteService.GetAllSites();
                if (allSites.Count == 1)
                {
                    settingsToDelete = _settingService
                        .GetAllSettings()
                        .Where(s => s.SiteId == allSites[0].Id)
                        .ToList();
                    foreach (var setting in settingsToDelete)
                        _settingService.DeleteSetting(setting);
                }

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Sites.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("Edit", new { id = site.Id });
            }
        }
    }
}
