using System;
using System.Linq;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.PageSettings.Models;
using CAF.WebSite.PageSettings.Domain;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.PageSettings.Services;

namespace CAF.WebSite.PageSettings.Controllers
{

    [AdminAuthorize]
    public class NavigationController : PluginControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IBannerInfoService _bannerInfoService;
        public NavigationController(IBannerInfoService bannerInfoService,
            ISettingService settingService, IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._bannerInfoService = bannerInfoService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedPartialView();

            var model = new BannerInfoModel();
            model.UrlTypeId = (int)BannerUrltypes.Link;
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam, BannerInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedPartialView();
            var bannerInfos = _bannerInfoService.GetAllBannerInfos();
            return DataTablesResult.Create(bannerInfos.Select(x => x.ToModel()).AsQueryable(), dataTableParam);
        }


        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedPartialView();

            var model = new BannerInfoModel();
            PrepareBannerInfoModel(model, null);
     
          
            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(BannerInfoModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedPartialView();

            if (ModelState.IsValid)
            {
                 
                var topic = model.ToEntity();
                topic.AddEntitySysParam();
                _topicService.InsertTopic(topic);

                // SEO
                topic.SystemName = topic.ValidateSeName(model.SystemName, model.Title, true);
                _urlRecordService.SaveSlug(topic, model.SystemName, 0);

                //locales
                UpdateLocales(topic, model);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = topic.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareBannerInfoModel(model, null);
            //Sites
            PrepareSitesMappingModel(model, null, true);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedPartialView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            var model = topic.ToModel();
            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");
            PrepareBannerInfoModel(model, null);
            //Site
            PrepareSitesMappingModel(model, topic, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = topic.GetLocalized(x => x.Title, languageId, false, false);
                locale.Body = topic.GetLocalized(x => x.Body, languageId, false, false);
                locale.MetaKeywords = topic.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = topic.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = topic.GetLocalized(x => x.MetaTitle, languageId, false, false);
            });

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(BannerInfoModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedPartialView();

            var topic = _topicService.GetTopicById(model.Id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");

            if (!model.IsPasswordProtected)
            {
                model.Password = null;
            }

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                topic.AddEntitySysParam(false, true);

                _topicService.UpdateTopic(topic);

                // SEO
                topic.SystemName = topic.ValidateSeName(model.SystemName, model.Title, true);
                _urlRecordService.SaveSlug(topic, model.SystemName, 0);

                //Sites
                SaveSiteMappings(topic, model);
                //locales
                UpdateLocales(topic, model);

                _eventPublisher.Publish(new ModelBoundEvent(model, topic, form));

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));
                return continueEditing ? RedirectToAction("Edit", topic.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            PrepareBannerInfoModel(model, null);

            //Site
            PrepareSitesMappingModel(model, topic, true);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedPartialView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            _topicService.DeleteTopic(topic);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}