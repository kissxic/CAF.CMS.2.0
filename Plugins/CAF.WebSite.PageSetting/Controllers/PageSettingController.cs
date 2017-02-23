using System;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services.Localization;


namespace CAF.WebSite.PageSettings.Controllers
{

    [AdminAuthorize]
    public class PageSettingController : PluginControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;

        public PageSettingController(
            ISettingService settingService, IPluginFinder pluginFinder,
            ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
        }

        public ActionResult Configure()
        {

            return View();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST()
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            return View();
        }
       

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object model)
        {
            int pictureId = 0;
            string Url = "";
            if (model != null)
            {
                int pageId = 0;
                string entity = "";
                //    if (model.GetType() == typeof(ProductOverviewModel))
                //    {
                //        ProductOverviewModel articleModel = (ProductOverviewModel)model;
                //        pageId = articleModel.Id;
                //        entity = "product";
                //    }
                //    else
                //    {
                //        if (model.GetType() == typeof(CategoryModel))
                //        {
                //            CategoryModel categoryModel = (CategoryModel)model;
                //            pageId = categoryModel.Id;
                //            entity = "category";
                //        }
                //        else
                //        {
                //            if (model.GetType() == typeof(TopicModel))
                //            {
                //                TopicModel topicModel = (TopicModel)model;
                //                pageId = topicModel.Id;
                //                entity = "topic";
                //            }
                //        }
                //    }
                //    CustomBannerRecord bannerRecord = this._customBannerService.GetCustomBannerRecord(pageId, entity);
                //    if (bannerRecord != null)
                //    {
                //        pictureId = bannerRecord.PictureId;
                //        Url = bannerRecord.Url;
                //    }
                //}
                //if (pictureId != 0)
                //{
                //    CustomBannerSettings customBannerSettings = this._settingService.LoadSetting<CustomBannerSettings>(this._storeContext.CurrentStore.Id);
                //    Picture pic = this._pictureService.GetPictureById(pictureId);
                //    return base.View(new PublicInfoModel
                //    {
                //        PicturePath = this._pictureService.GetPictureUrl(pic, 0, true, null),
                //        MaxBannerHeight = customBannerSettings.MaxBannerHeight,
                //        StretchPicture = customBannerSettings.StretchPicture,
                //        ShowBorderBottom = customBannerSettings.ShowBorderBottom,
                //        ShowBorderTop = customBannerSettings.ShowBorderTop,
                //        BorderTopColor = customBannerSettings.BorderTopColor,
                //        BorderBottomColor = customBannerSettings.BorderBottomColor,
                //        Url = Url
                //    });
                //}
            }
            return base.Content("");
        }

    }
}