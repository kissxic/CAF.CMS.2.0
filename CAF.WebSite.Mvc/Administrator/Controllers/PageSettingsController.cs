using System;
using System.Linq;
using System.Web.Mvc;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Plugins;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Configuration;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.PageSettings;
using CAF.WebSite.Mvc.Admin.Models.PageSettings;
using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Articles;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CAF.WebSite.Mvc.Admin.Controllers
{

    public class PageSettingsController : AdminControllerBase
    {
        private readonly IProductCategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IBannerInfoService _bannerInfoService;
        private readonly ISlideAdInfoService _slideAdInfoService;
        private readonly IHomeCategoryService _homeCategoryService;
        private readonly IHomeFloorInfoService _homeFloorInfoService;
        private readonly HomePageSettings _homePageSettings;
        public PageSettingsController(IProductCategoryService categoryService,
            IPictureService pictureService,
            IBannerInfoService bannerInfoService,
            ISlideAdInfoService slideAdInfoService,
            IHomeCategoryService homeCategoryService,
            ISettingService settingService, IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IHomeFloorInfoService homeFloorInfoService,
            HomePageSettings homePageSettings)
        {
            this._categoryService = categoryService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._bannerInfoService = bannerInfoService;
            this._slideAdInfoService = slideAdInfoService;
            this._homeCategoryService = homeCategoryService;
            this._homeFloorInfoService = homeFloorInfoService;
            this._homePageSettings = homePageSettings;
        }

        #region Utitilies
        /// <summary>
        /// 装配图片信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        [NonAction]
        private void PrepareSlideAdInfoPictureThumbnailModel(SlideAdInfoModel model, SlideAdInfo slideAdInfo)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (slideAdInfo != null)
            {
                var defaultArticlePicture = _pictureService.GetPictureById(slideAdInfo.PictureId);
                model.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultArticlePicture, 75, 75, true);
            }
        }

        private void PrepareHomeCategoryModel(HomeCategoryModel model, HomeCategory homeCategory)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            var categoryIds = model.CategoryIds;

            model.CategoryIds = string.Join(",", _homeCategoryService.GetAllHomeCategoryItems().Select(x => x.CategoryId).ToArray());

        }

        private void UpdateHomeCategory(HomeCategoryModel model, HomeCategory homeCategory)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (model.CategoryIds.IsEmpty()) return;
            var categories = _categoryService.GetAllCategories(showHidden: true);

            var categoryIds = model.CategoryIds.TrimEnd(',').Split(',').Select(x => { return x.ToInt(); });
            foreach (var item in categories)
            {
                //判断是否已经选择
                if (categoryIds.Where(pc => pc == item.Id).Any())
                {
                    if (homeCategory.HomeCategoryItems.Where(x => x.CategoryId == item.Id).FirstOrDefault() == null)
                    {
                        var homeCategoryInfo = new HomeCategoryItem()
                        {
                            CategoryId = item.Id,
                            Depth = item.Depth,
                            Name = item.Name,
                            SeName = item.GetSeName(),
                            ParentCategoryId = item.ParentCategoryId,

                        };
                        homeCategory.HomeCategoryItems.Add(homeCategoryInfo);
                    }

                }
                else
                {
                    var categoryItem = homeCategory.HomeCategoryItems.Where(x => x.CategoryId == item.Id).FirstOrDefault();
                    if (categoryItem != null)
                    {
                        homeCategory.HomeCategoryItems.Remove(categoryItem);
                    }
                }
            }

        }
        #endregion


        public ActionResult Index()
        {

            var array = _slideAdInfoService.GetAllImageAdInfos().Where(p => p.Id >= 0 || p.Id <= 8)
                .Select(x => new ImageAdInfoModel()
                {
                    Id = x.Id,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId, showDefaultPicture: true),
                    Url = x.Url
                }).ToArray();

            return View(array);
        }


        #region 首页配置
        public ActionResult HomePageSettings()
        {
            var homePageSettings = _settingService.LoadSetting<HomePageSettings>();
            var homePageSettingsModel = new HomePageSettingsModel()
            {
                RelateCategoryIds = homePageSettings.RelateCategoryIds,
                RelateManufacturerIds = homePageSettings.RelateManufacturerIds,
                RelateProductIds = homePageSettings.RelateProductIds,
                RelateVendorIds = homePageSettings.RelateVendorIds,
            };

            return View(homePageSettingsModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult HomePageSettings(HomePageSettingsModel model)
        {
            var homePageSettings = new HomePageSettings()
            {
                RelateCategoryIds = model.RelateCategoryIds,
                RelateManufacturerIds = model.RelateManufacturerIds,
                RelateProductIds = model.RelateProductIds,
                RelateVendorIds = model.RelateVendorIds,
            };
            _settingService.SaveSetting<HomePageSettings>(homePageSettings);
            return View(model);
        }

        #endregion


        #region 菜单导航

        public ActionResult NavigationList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var model = new BannerInfoModel();
            model.UrlTypeId = (int)BannerUrltypes.Link;
            return View(model);
        }

        [HttpPost]
        public ActionResult NavigationList(DataTablesParam dataTableParam, BannerInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();
            var bannerInfos = _bannerInfoService.GetAllBannerInfos();
            return DataTablesResult.Create(bannerInfos.Select(x => x.ToModel()).AsQueryable(), dataTableParam);
        }


        public ActionResult NavigationCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var model = new BannerInfoModel();
            model.BannerUrltypes = BannerUrltypes.Link;

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult NavigationCreate(BannerInfoModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            if (ModelState.IsValid)
            {

                var bannerInfo = model.ToEntity();
                bannerInfo.AddEntitySysParam();
                _bannerInfoService.InsertBannerInfo(bannerInfo);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.bannerInfos.Added"));
                return continueEditing ? RedirectToAction("NavigationEdit", new { id = bannerInfo.Id }) : RedirectToAction("NavigationList");
            }


            return View(model);
        }

        public ActionResult NavigationEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var bannerInfo = _bannerInfoService.GetBannerInfoById(id);
            if (bannerInfo == null)
                //No bannerInfo found with the specified id
                return RedirectToAction("NavigationList");

            var model = bannerInfo.ToModel();

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult NavigationEdit(BannerInfoModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var bannerInfo = _bannerInfoService.GetBannerInfoById(model.Id);
            if (bannerInfo == null)
                //No bannerInfo found with the specified id
                return RedirectToAction("NavigationList");

            if (ModelState.IsValid)
            {
                bannerInfo = model.ToEntity(bannerInfo);
                bannerInfo.AddEntitySysParam(false, true);

                _bannerInfoService.UpdateBannerInfo(bannerInfo);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.bannerInfos.Updated"));
                return continueEditing ? RedirectToAction("NavigationEdit", bannerInfo.Id) : RedirectToAction("NavigationList");
            }

            return View(model);
        }

        [HttpPost, ActionName("NavigationDelete")]
        public ActionResult NavigationDeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var bannerInfo = _bannerInfoService.GetBannerInfoById(id);
            if (bannerInfo == null)
                //No bannerInfo found with the specified id
                return RedirectToAction("NavigationList");

            _bannerInfoService.DeleteBannerInfo(bannerInfo);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.bannerInfos.Deleted"));
            return RedirectToAction("NavigationList");
        }

        #endregion

        #region 幻灯片

        public ActionResult SlideAdInfoList(int sTypeId = 5)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var model = new SlideAdInfoModel();
            model.SlideAdTypeId = sTypeId;
            return View(model);
        }

        [HttpPost]
        public ActionResult SlideAdInfoList(DataTablesParam dataTableParam, SlideAdInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();
            var slideAdInfos = _slideAdInfoService.GetAllSlideAdInfos(model.SlideAdTypeId);
            return DataTablesResult.Create(slideAdInfos.Select(x =>
            {
                var m = x.ToModel();
                PrepareSlideAdInfoPictureThumbnailModel(m, x);
                return m;
            }).AsQueryable(), dataTableParam);
        }


        public ActionResult SlideAdInfoCreate(int sTypeId = 5)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var model = new SlideAdInfoModel();
            model.SlideAdTypeId = sTypeId;

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult SlideAdInfoCreate(SlideAdInfoModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            if (ModelState.IsValid)
            {

                var slideAdInfo = model.ToEntity();
                slideAdInfo.AddEntitySysParam();
                _slideAdInfoService.InsertSlideAdInfo(slideAdInfo);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.slideAdInfos.Added"));
                return continueEditing ? RedirectToAction("SlideAdInfoEdit", new { id = slideAdInfo.Id }) : RedirectToAction("SlideAdInfoList", new { sTypeId = model.SlideAdTypeId });
            }


            return View(model);
        }

        public ActionResult SlideAdInfoEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var slideAdInfo = _slideAdInfoService.GetSlideAdInfoById(id);
            if (slideAdInfo == null)
                //No slideAdInfo found with the specified id
                return RedirectToAction("SlideAdInfoList");

            var model = slideAdInfo.ToModel();

            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult SlideAdInfoEdit(SlideAdInfoModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var slideAdInfo = _slideAdInfoService.GetSlideAdInfoById(model.Id);
            if (slideAdInfo == null)
                //No slideAdInfo found with the specified id
                return RedirectToAction("SlideAdInfoList");

            if (ModelState.IsValid)
            {
                slideAdInfo = model.ToEntity(slideAdInfo);
                slideAdInfo.AddEntitySysParam(false, true);

                _slideAdInfoService.UpdateSlideAdInfo(slideAdInfo);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.slideAdInfos.Updated"));
                return continueEditing ? RedirectToAction("SlideAdInfoEdit", slideAdInfo.Id) : RedirectToAction("SlideAdInfoList", new { sTypeId = model.SlideAdTypeId });
            }

            return View(model);
        }

        [HttpPost, ActionName("SlideAdInfoDelete")]
        public ActionResult SlideAdInfoDeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var slideAdInfo = _slideAdInfoService.GetSlideAdInfoById(id);
            if (slideAdInfo == null)
                //No slideAdInfo found with the specified id
                return RedirectToAction("SlideAdInfoList");

            _slideAdInfoService.DeleteSlideAdInfo(slideAdInfo);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.slideAdInfos.Deleted"));
            return RedirectToAction("SlideAdInfoList");
        }

        #endregion

        #region 首页菜单

        public ActionResult AjaxProductCategory(int? homeCategoryId, int? pid)
        {

            var homeCategoryIds = _homeCategoryService.GetAllHomeCategoryItemsByHomeCategoryId(homeCategoryId ?? 0).Select(x => x.CategoryId).ToList();

            var categories = _categoryService.GetAllCategories(showHidden: true);

            var cquery = categories.AsQueryable();

            var query =
                from c in cquery
                select new
                {
                    id = c.Id.ToString(),
                    pId = c.ParentCategoryId,
                    name = c.Name,
                    open = false,
                    selected = homeCategoryIds.Any(x => x == c.Id)
                };
            return new JsonResult { Data = query.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult HomeCategoryList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var model = new HomeCategoryModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult HomeCategoryList(DataTablesParam dataTableParam, HomeCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();
            var slideAdInfos = _homeCategoryService.GetAllHomeCategorys();
            return DataTablesResult.Create(slideAdInfos.Select(x =>
            {
                var m = x.ToModel();
                return m;
            }).AsQueryable(), dataTableParam);
        }


        public ActionResult HomeCategoryCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var model = new HomeCategoryModel();
            PrepareHomeCategoryModel(model, null);
            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult HomeCategoryCreate(HomeCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            if (ModelState.IsValid)
            {

                var homeCategory = model.ToEntity();
                homeCategory.AddEntitySysParam();
                UpdateHomeCategory(model, homeCategory);
                _homeCategoryService.InsertHomeCategory(homeCategory);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.slideAdInfos.Added"));
                return continueEditing ? RedirectToAction("HomeCategoryEdit", new { id = homeCategory.Id }) : RedirectToAction("HomeCategoryList");
            }

            PrepareHomeCategoryModel(model, null);
            return View(model);
        }

        public ActionResult HomeCategoryEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var homeCategory = _homeCategoryService.GetHomeCategoryById(id);
            if (homeCategory == null)
                //No homeCategory found with the specified id
                return RedirectToAction("HomeCategoryList");

            var model = homeCategory.ToModel();
            PrepareHomeCategoryModel(model, homeCategory);
            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult HomeCategoryEdit(HomeCategoryModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var homeCategory = _homeCategoryService.GetHomeCategoryById(model.Id);
            if (homeCategory == null)
                //No homeCategory found with the specified id
                return RedirectToAction("HomeCategoryList");

            if (ModelState.IsValid)
            {
                homeCategory = model.ToEntity(homeCategory);
                homeCategory.AddEntitySysParam(false, true);
                if (model.IsChangeCategory)
                    UpdateHomeCategory(model, homeCategory);
                _homeCategoryService.UpdateHomeCategory(homeCategory);


                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.slideAdInfos.Updated"));
                return continueEditing ? RedirectToAction("HomeCategoryEdit", homeCategory.Id) : RedirectToAction("HomeCategoryList");
            }
            PrepareHomeCategoryModel(model, homeCategory);
            return View(model);
        }

        [HttpPost, ActionName("HomeCategoryDelete")]
        public ActionResult HomeCategoryDeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalog))
                return AccessDeniedPartialView();

            var homeCategory = _homeCategoryService.GetHomeCategoryById(id);
            if (homeCategory == null)
                //No homeCategory found with the specified id
                return RedirectToAction("HomeCategory");

            _homeCategoryService.DeleteHomeCategory(homeCategory);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.slideAdInfos.Deleted"));
            return RedirectToAction("HomeCategoryList");
        }

        #endregion

        #region 首页楼层
        /// <summary>
        /// 楼层
        /// </summary>
        /// <returns></returns>
        public ActionResult HomeFloor()
        {
            var homeFloors = _homeFloorInfoService.GetAllHomeFloorInfos(showHidden: true);
            var homeFloor =
                 from item in homeFloors
                 select new HomeFloor()
                 {
                     DisplayOrder = item.DisplayOrder,
                     Enable = item.IsShow,
                     Id = item.Id,
                     Name = item.FloorName,
                     StyleLevel = item.StyleLevel
                 };
            return View(homeFloor);
        }

        /// <summary>
        /// 保存楼层
        /// </summary>
        /// <param name="floorDetail"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveHomeFloorDetail(string floorDetail)
        {
            var homeFloorDetail = JsonConvert.DeserializeObject<HomeFloorDetail>(floorDetail);
            var homeFloorInfo = new HomeFloorInfo()
            {
                Id = homeFloorDetail.Id,
                FloorName = homeFloorDetail.Name,
                Url = homeFloorDetail.Url,
                SubName = homeFloorDetail.SubName,
                DefaultTabName = homeFloorDetail.DefaultTabName,
                StyleLevel = homeFloorDetail.StyleLevel,
                RelateCategoryIds = homeFloorDetail.CategoryIds,
                RelateVendorIds = homeFloorDetail.VendorIds,
                RelateManufacturerIds = homeFloorDetail.ManufacturerIds,
                RelateProductIds = homeFloorDetail.ProductIds,

            };
            //添加文本信息
            homeFloorInfo.FloorTopicInfos.AddRange(
                   from i in homeFloorDetail.TextLinks
                   select new FloorTopicInfo()
                   {
                       FloorId = homeFloorDetail.Id,
                       TopicName = i.Name,
                       Url = i.Url,
                       PictureId = i.PictureId == 0 ? null : (int?)i.PictureId,
                       TopicType = Position.Top
                   });
            //添加文本信息
            homeFloorInfo.FloorTopicInfos.AddRange(
                from i in homeFloorDetail.ArticleLinks
                select new FloorTopicInfo()
                {
                    FloorId = homeFloorDetail.Id,
                    TopicName = "",
                    Url = i.Url,
                    PictureId = i.PictureId == 0 ? null : (int?)i.PictureId,
                    TopicType = (Position)((int)i.Id)
                });
            //幻灯片
            if (homeFloorDetail.Slides != null)
            {

                homeFloorDetail.Slides.Each(x =>
                {
                    var floorSlidesInfo = new FloorSlidesInfo()
                    {
                        Name = x.Name
                    };
                    x.Detail.Each(d =>
                    {
                        floorSlidesInfo.FloorSlideDetailsInfos.Add(new FloorSlideDetailsInfo()
                        {
                            Url = d.Url,
                            ArticleId = d.ArticleId,
                            PictureId = d.PictureId == 0 ? null : (int?)d.PictureId,
                        });
                    });
                    homeFloorInfo.FloorSlidesInfos.Add(floorSlidesInfo);
                });

            }
            _homeFloorInfoService.UpdateHomeFloorInfo(homeFloorInfo);
            return Json(new { success = true });
        }

        /// <summary>
        /// 更新拍寻
        /// </summary>
        /// <param name="oriRowNumber"></param>
        /// <param name="newRowNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FloorChangeSequence(int oriRowNumber, int newRowNumber)
        {
            _homeFloorInfoService.UpdateHomeFloorSequence(oriRowNumber, newRowNumber);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FloorEnableDisplay(int id, bool enable)
        {

            var homeFloorInfo = _homeFloorInfoService.GetHomeFloorInfoById(id);
            homeFloorInfo.IsShow = enable;
            _homeFloorInfoService.UpdateHomeFloorInfo(homeFloorInfo);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteFloor(int id)
        {
            var homeCategory = _homeFloorInfoService.GetHomeFloorInfoById(id);
            if (homeCategory == null)
                return RedirectToAction("HomeFloorInfo");

            _homeFloorInfoService.DeleteHomeFloorInfo(homeCategory);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }


        #region Edit
        /// <summary>
        /// 楼层一
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddHomeFloorDetail(int id = 0)
        {
            var homeFloorDetail = new HomeFloorDetail();
            var homeFloor = _homeFloorInfoService.GetHomeFloorInfoById(id);
            if (homeFloor != null)
            {
                homeFloorDetail.Id = homeFloor.Id;
                homeFloorDetail.Name = homeFloor.FloorName;
                homeFloorDetail.SubName = homeFloor.SubName;
                homeFloorDetail.Url = homeFloor.Url;
                homeFloorDetail.DefaultTabName = homeFloor.DefaultTabName;
                homeFloorDetail.ManufacturerIds = homeFloor.RelateManufacturerIds;
                homeFloorDetail.CategoryIds = homeFloor.RelateCategoryIds;
                homeFloorDetail.ProductIds = homeFloor.RelateProductIds;
                homeFloorDetail.VendorIds = homeFloor.RelateVendorIds;
                homeFloorDetail.TextLinks =
                     homeFloor.FloorTopicInfos.Where(v => v.TopicType == Position.Top).Select(item => new HomeFloorDetail.TextLink()
                     {
                         Id = item.Id,
                         Name = item.TopicName,
                         Url = item.Url,
                         PictureId = item.PictureId ?? 0,
                     }).ToList();
                homeFloorDetail.ArticleLinks =
                 homeFloor.FloorTopicInfos.Where(v => v.TopicType != Position.Top).Select(item => new HomeFloorDetail.TextLink()
                 {
                     Id = item.TopicTypeId,
                     Name = item.Url,
                     Url = _pictureService.GetPictureUrl(item.PictureId ?? 0, 75, 75, true),
                     PictureId = item.PictureId ?? 0,
                 }).OrderBy(v => v.Id).ToList();
            }
            else
            {
                homeFloor = new HomeFloorInfo();
                List<HomeFloorDetail.TextLink> textLinks = new List<HomeFloorDetail.TextLink>();
                for (int num = 0; num < 10; num++)
                {
                    HomeFloorDetail.TextLink textLink = new HomeFloorDetail.TextLink()
                    {
                        Id = num,
                        Name = "",
                        Url = ""
                    };
                    textLinks.Add(textLink);
                }
                homeFloorDetail.ArticleLinks = textLinks;
            }
            if (homeFloorDetail.Slides == null)
            {
                homeFloorDetail.Slides = new List<HomeFloorDetail.Slide>();
            }
            return View(homeFloorDetail);
        }
        /// <summary>
        /// 楼层二
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddHomeFloorDetail2(int id = 0)
        {
            var homeFloorDetail = new HomeFloorDetail();
            var homeFloor = _homeFloorInfoService.GetHomeFloorInfoById(id);
            if (homeFloor != null)
            {
                homeFloorDetail.Id = homeFloor.Id;
                homeFloorDetail.Name = homeFloor.FloorName;
                homeFloorDetail.SubName = homeFloor.SubName;
                homeFloorDetail.Url = homeFloor.Url;
                homeFloorDetail.DefaultTabName = homeFloor.DefaultTabName;
                homeFloorDetail.ManufacturerIds = homeFloor.RelateManufacturerIds;
                homeFloorDetail.CategoryIds = homeFloor.RelateCategoryIds;
                homeFloorDetail.ProductIds = homeFloor.RelateProductIds;
                homeFloorDetail.VendorIds = homeFloor.RelateVendorIds;
                homeFloorDetail.TextLinks =
                     homeFloor.FloorTopicInfos.Where(v => v.TopicType == Position.Top).Select(item => new HomeFloorDetail.TextLink()
                     {
                         Id = item.Id,
                         Name = item.TopicName,
                         Url = item.Url,
                         PictureId = item.PictureId ?? 0,
                     }).ToList();
                homeFloorDetail.ArticleLinks =
                    homeFloor.FloorTopicInfos.Where(v => v.TopicType != Position.Top).Select(item => new HomeFloorDetail.TextLink()
                    {
                        Id = item.TopicTypeId,
                        Name = item.Url,
                        Url = _pictureService.GetPictureUrl(item.PictureId ?? 0, 75, 75, true),
                        PictureId = item.PictureId ?? 0,
                    }).OrderBy(v => v.Id).ToList();

                homeFloorDetail.Slides =
                    (from item in homeFloor.FloorSlidesInfos
                     where item.FloorId == homeFloor.Id
                     select item into p
                     orderby p.Id
                     select p into item
                     select new HomeFloorDetail.Slide()
                     {
                         Id = item.Id,
                         Detail =
                             (from detail in item.FloorSlideDetailsInfos
                              where detail.FloorSlideId == item.Id
                              select detail into p
                              select new HomeFloorDetail.ArticleDetail()
                              {
                                  Id = p.Id,
                                  ArticleId = p.ArticleId ?? 0
                              }).ToList(),
                         Name = item.Name,
                         Count = (
                             from detail in item.FloorSlideDetailsInfos
                             where detail.FloorSlideId == item.Id
                             select detail).Count(),
                         Ids = ArrayToString((
                             from detail in item.FloorSlideDetailsInfos
                             where detail.FloorSlideId == item.Id
                             select detail into p
                             select p.ArticleId ?? 0).ToArray())
                     }).ToList();
            }
            else
            {
                homeFloor = new HomeFloorInfo();
                List<HomeFloorDetail.TextLink> textLinks = new List<HomeFloorDetail.TextLink>();
                for (int num = 0; num < 24; num++)
                {
                    HomeFloorDetail.TextLink textLink = new HomeFloorDetail.TextLink()
                    {
                        Id = num,
                        Name = "",
                        Url = ""
                    };
                    textLinks.Add(textLink);
                }
                homeFloorDetail.ArticleLinks = textLinks;
                homeFloorDetail.StyleLevel = 1;
            }
            if (homeFloorDetail.Slides == null)
            {
                homeFloorDetail.Slides = new List<HomeFloorDetail.Slide>();
            }

            return View(homeFloorDetail);
        }
        /// <summary>
        /// 楼层三 文本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddHomeFloorDetail3(int id = 0)
        {
            var homeFloorDetail = new HomeFloorDetail();
            var homeFloor = _homeFloorInfoService.GetHomeFloorInfoById(id);
            if (homeFloor != null)
            {
                homeFloorDetail.Id = homeFloor.Id;
                homeFloorDetail.Name = homeFloor.FloorName;

                homeFloorDetail.TextLinks =
                     homeFloor.FloorTopicInfos.Where(v => v.TopicType == Position.Top).Select(item => new HomeFloorDetail.TextLink()
                     {
                         Id = item.Id,
                         Name = item.TopicName,
                         Url = item.Url,
                         PictureId = item.PictureId ?? 0,
                     }).ToList();

            }
            else
            {
                homeFloorDetail.StyleLevel = 2;
            }

            return View(homeFloorDetail);
        }

        private string ArrayToString(int[] array)
        {
            string empty = string.Empty;
            int[] numArray = array;
            for (int i = 0; i < numArray.Length; i++)
            {
                int num = numArray[i];
                empty = string.Concat(empty, num, ",");
            }
            return empty.Substring(0, empty.Length - 1);
        }
        #endregion

        #endregion

        #region 橱窗广告位
        [HttpPost]
        public ActionResult UpdateImageAd(int id, int pic, string url)
        {
            var imageAdInfo = _slideAdInfoService.GetImageAdInfoByPostionId(id);
            if (imageAdInfo != null)
            {
                imageAdInfo.Url = url;
                imageAdInfo.PictureId = pic;
                imageAdInfo.ModifiedOnUtc = DateTime.UtcNow;
            }
            else
            {
                imageAdInfo = new ImageAdInfo()
                {
                    PostionId = id,
                    Url = url,
                    PictureId = pic,
                    CreatedOnUtc = DateTime.UtcNow,
                    ModifiedOnUtc = DateTime.UtcNow,

                };
            }
            _slideAdInfoService.UpdateImageAdInfo(imageAdInfo);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}