using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Members;
using CAF.Infrastructure.Core.Domain.Sellers;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Exceptions;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.Services.Vendors;
using CAF.WebSite.Mvc.Models.ShopProfile;
using System.Web.Mvc;


namespace CAF.WebSite.Mvc.Controllers
{
    public class ShopProfileController : Controller
    {
        #region Fields



        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IUserService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _customerRegistrationService;
        private readonly IVendorService _vendorService;
        private readonly MemberGradeSettings _memberGradeSettings;

        #endregion

        #region Ctor

        public ShopProfileController(
            ILocalizationService localizationService,
            IWorkContext workContext, ISiteContext siteContext,
            IUserService customerService,
            IGenericAttributeService genericAttributeService,
            IUserRegistrationService customerRegistrationService,
            IVendorService vendorService,
            MemberGradeSettings memberGradeSettings
           )
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._vendorService = vendorService;
            this._memberGradeSettings = memberGradeSettings;
        }

        #endregion


        #region Utilities
        [NonAction]
        protected bool IsCurrentUserRegistered()
        {
            return _workContext.CurrentUser.IsRegistered();
        }

        #endregion

        // GET: ShopProfile
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OpenSeller()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();
            return View();
        }
        [HttpPost]
        public ActionResult Agreement(FormCollection form)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();
            if (!form["agree"].Equals("on"))
            {
                return RedirectToAction("OpenSeller");
            }
            if (_workContext.CurrentVendor == null)
            {
                Vendor vendorInfo = this._vendorService.CreateEmptyVendor();
                var customer = this._customerService.GetUserById(_workContext.CurrentUser.Id);
                customer.VendorId = vendorInfo.Id;
                customer.MemberGradeId = this._memberGradeSettings.DefaultMemberGradeId;
                //add to 'Guests' role
                var vendorRole = this._customerService.GetUserRoleBySystemName(SystemUserRoleNames.Vendors);
                if (vendorRole == null)
                    throw new WorkException("'Vendor' role could not be loaded");
                customer.UserRoles.Add(vendorRole);
                this._customerService.UpdateUser(customer);
                _workContext.CurrentUser = customer;
            }
            return RedirectToAction("EditProfileAudit2");
        }


        public ActionResult EditProfileAudit2()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentVendor == null)
            {
                return RedirectToAction("OpenSeller");
            }
            ShopProfileModel model = new ShopProfileModel();
            Vendor vendorInfo = this._vendorService.GetVendorById(_workContext.CurrentUser.VendorId);

            model.VendorAgreement = vendorInfo.ToModel();
            model.Frame = "经营资质";
            model.Step = 1;
            return View(model);
        }

        [HttpPost]
        public ActionResult Editprofile2(ShopProfileModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentVendor == null)
            {
                return RedirectToAction("OpenSeller");
            }
            Vendor vendorInfo = this._vendorService.GetVendorById(model.VendorAgreement.Id);
            vendorInfo.VendorStage = VendorStage.Finish;
            vendorInfo.Name = model.VendorAgreement.Name;
            vendorInfo.CompanyName = model.VendorAgreement.CompanyName;
            vendorInfo.Email = model.VendorAgreement.Email;
            this._vendorService.UpdateVendor(vendorInfo);
            return Redirect("~/seller");
        }

    }
}