using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Media;
using CAF.WebSite.Application.Services.Topics;
using CAF.WebSite.Application.Services.Vendors;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.WebUI.Security;
using CAF.WebSite.Mvc.Models.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Controllers
{
    /// <summary>
    /// 商家
    /// </summary>
    public partial class VendorController : PublicControllerBase
    {
        #region Fields

        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IPictureService _pictureService;
        #endregion

        #region Constructors

        public VendorController(IVendorService vendorService,
            ILocalizationService localizationService,
             IModelTemplateService modelTemplateService,
            IWorkContext workContext,
            ISiteContext siteContext,
            ICacheManager cacheManager,
            IPictureService pictureService)
        {
            this._modelTemplateService = modelTemplateService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
            this._pictureService = pictureService;
        }

        #endregion



        #region Methods

        /// <summary>
        /// 店铺明细
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns></returns>
        public ActionResult VendorDetails(string systemName)
        {

            return View();
        }

        /// <summary>
        /// 店铺明细
        /// </summary>
        /// <param name="vendorId">系统名称</param>
        /// <returns></returns>
        public ActionResult VendorBlock(int vendorId)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted)
                return PartialView(null);
            var model = vendor.ToModel();
            model.LogoPictureUrl = _pictureService.GetPictureUrl(vendor.PictureId.GetValueOrDefault());
            return PartialView(model);
        }
        #endregion
    }
}