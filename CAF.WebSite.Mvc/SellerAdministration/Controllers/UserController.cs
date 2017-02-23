using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.ExportImport;
using CAF.WebSite.Application.Services.Forums;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Messages;
using CAF.WebSite.Application.Services.Sites;
using CAF.WebSite.Application.Services.Users;
using CAF.WebSite.Application.WebUI.Plugins;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Messages;
using CAF.Infrastructure.Core.Domain.Tax;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Seller.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.WebSite.Application.Services.Vendors;
using CAF.WebSite.Mvc.Seller.Models.Common;

namespace CAF.WebSite.Mvc.Seller.Controllers
{

    public partial class UserController : SellerAdminControllerBase
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserReportService _userReportService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly UserSettings _userSettings;
        private readonly IWorkContext _workContext;
        private readonly ISiteContext _siteContext;
        private readonly IExportManager _exportManager;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ForumSettings _forumSettings;
        private readonly IForumService _forumService;
        private readonly AddressSettings _addressSettings;
        private readonly ISiteService _siteService;
        private readonly IEventPublisher _eventPublisher;
        private readonly PluginMediator _pluginMediator;
        private readonly IVendorService _vendorService;
        #endregion

        #region Constructors

        public UserController(IUserService userService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IGenericAttributeService genericAttributeService,
            IUserRegistrationService userRegistrationService,
            IUserReportService userReportService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IAddressService addressService,
            UserSettings userSettings,
            IWorkContext workContext, ISiteContext siteContext,
            IExportManager exportManager,
            IUserActivityService userActivityService,
            IPermissionService permissionService,
            AdminAreaSettings adminAreaSettings,
            IQueuedEmailService queuedEmailService, EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService, ForumSettings forumSettings,
            IForumService forumService,
            AddressSettings addressSettings, ISiteService siteService,
            IEventPublisher eventPublisher,
            PluginMediator pluginMediator,
            IVendorService vendorService
            )
        {
            this._userService = userService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._genericAttributeService = genericAttributeService;
            this._userRegistrationService = userRegistrationService;
            this._userReportService = userReportService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._userSettings = userSettings;
            this._workContext = workContext;
            this._siteContext = siteContext;
            this._exportManager = exportManager;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._forumSettings = forumSettings;
            this._forumService = forumService;
            this._addressSettings = addressSettings;
            this._siteService = siteService;
            this._eventPublisher = eventPublisher;
            this._pluginMediator = pluginMediator;
            this._vendorService = vendorService;
        }

        #endregion

        #region Utilities
        [NonAction]
        protected virtual void PrepareVendorsModel(UserModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableVendors.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Users.Users.Fields.Vendor.None"),
                Value = "0"
            });
            var vendors = _vendorService.GetAllVendors(showHidden: true);
            foreach (var vendor in vendors)
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Text = vendor.Name,
                    Value = vendor.Id.ToString()
                });
            }
        }
        [NonAction]
        protected string GetUserRolesNames(IList<UserRole> userRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < userRoles.Count; i++)
            {
                sb.Append(userRoles[i].Name);
                if (i != userRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
        [NonAction]
        protected UserModel PrepareUserModelForList(User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Email = !String.IsNullOrEmpty(user.Email) ? user.Email : (user.IsGuest() ? _localizationService.GetResource("Admin.Users.Guest") : "".NaIfEmpty()),
                UserName = user.UserName,
                FullName = user.GetFullName(),
                Company = user.GetAttribute<string>(SystemUserAttributeNames.Company),
                Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode),
                UserRoleNames = GetUserRolesNames(user.UserRoles.ToList()),
                Active = user.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc),
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc),
            };
        }

        private void PrepareUserModelForCreate(UserModel model)
        {
            string timeZoneId = (model.TimeZoneId.HasValue() ? model.TimeZoneId : _dateTimeHelper.DefaultSiteTimeZone.Id);

            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.AllowUsersToChangeUserNames = _userSettings.AllowUsersToChangeUserNames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;

            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            {
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == timeZoneId) });
            }

            model.DisplayVatNumber = false;
            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();

            if (model.SelectedUserRoleIds == null)
            {
                model.SelectedUserRoleIds = new int[0];
            }

            model.AllowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });

                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.CountryId) });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
                    }
                }
            }
        }


        #endregion

        #region Users

        //ajax
        public ActionResult AllUserRoles(string label, int selectedId)
        {
            var userRoles = _userService.GetAllUserRoles(true);
            if (label.HasValue())
            {
                userRoles.Insert(0, new UserRole { Name = label, Id = 0 });
            }

            var list = from c in userRoles
                       select new
                       {
                           id = c.Id.ToString(),
                           text = c.Name,
                           selected = c.Id == selectedId
                       };

            return new JsonResult { Data = list.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserModel();
            model.Id = user.Id;
            model.Email = user.Email;
            model.UserName = user.UserName;
            model.AdminComment = user.AdminComment;
            model.IsTaxExempt = user.IsTaxExempt;
            model.Active = user.Active;
            model.TimeZoneId = user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId);
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.AllowUsersToChangeUserNames = _userSettings.AllowUsersToChangeUserNames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);
            model.VatNumberStatusNote = ((VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId))
                .GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
            model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
            model.LastIpAddress = user.LastIpAddress;
            model.LastVisitedPage = user.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);
            model.AffiliateId = user.AffiliateId;

            if (user.AffiliateId != 0)
            {
                //var affiliate = _affiliateService.GetAffiliateById(user.AffiliateId);
                //if (affiliate != null && affiliate.Address != null)
                //    model.AffiliateFullName = affiliate.Address.GetFullName();
            }

            //form fields
            model.FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
            model.LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
            model.Gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
            model.DateOfBirth = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
            model.Company = user.GetAttribute<string>(SystemUserAttributeNames.Company);
            model.StreetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
            model.StreetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
            model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);

            model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
            model.StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId);
            model.CityId = user.GetAttribute<int>(SystemUserAttributeNames.CityId);
            model.DistrictId = user.GetAttribute<int>(SystemUserAttributeNames.DistrictId);
            model.Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
            model.Fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);

            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            model.VendorId = user.VendorId;
            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                            model.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                    else
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

                }
            }
            PrepareVendorsModel(model);
            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.SelectedUserRoleIds = user.UserRoles.Select(cr => cr.Id).ToArray();
            model.AllowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);
            //reward points gistory
            // model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Edit(UserModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);
        
            bool allowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);

            if (ModelState.IsValid)
            {
                try
                {
                    user.AdminComment = model.AdminComment;
                    user.IsTaxExempt = model.IsTaxExempt;
                    user.Active = model.Active;
                    //email
                    if (!String.IsNullOrWhiteSpace(model.Email))
                    {
                        _userRegistrationService.SetEmail(user, model.Email);
                    }
                    else
                    {
                        user.Email = model.Email;
                    }

                    //username
                    if (_userSettings.UserNamesEnabled && _userSettings.AllowUsersToChangeUserNames)
                    {
                        if (!String.IsNullOrWhiteSpace(model.UserName))
                        {
                            _userRegistrationService.SetUserName(user, model.UserName);
                        }
                        else
                        {
                            user.UserName = model.UserName;
                        }
                    }

                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        string prevVatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);

                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.VatNumber, model.VatNumber);
                        //set VAT number status
                        if (!String.IsNullOrEmpty(model.VatNumber))
                        {
                            if (!model.VatNumber.Equals(prevVatNumber, StringComparison.InvariantCultureIgnoreCase))
                            {
                                //_genericAttributeService.SaveAttribute(user,
                                //    SystemUserAttributeNames.VatNumberStatusId,
                                //    (int)_taxService.GetVatNumberStatus(model.VatNumber));
                            }
                        }
                        else
                        {
                            _genericAttributeService.SaveAttribute(user,
                                SystemUserAttributeNames.VatNumberStatusId,
                                (int)VatNumberStatus.Empty);
                        }
                    }
                    //vendor
                    user.VendorId = model.VendorId;
                    user.AddEntitySysParam(false, true);
                    _userService.UpdateUser(user);

                    //form fields
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, model.DateOfBirth);
                    if (_userSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Company, model.Company);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode, model.ZipPostalCode);

                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId, model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled && _userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CityId, model.CityId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled && _userSettings.CityEnabled && _userSettings.DistrictEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DistrictId, model.DistrictId);

                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);


                    //user roles
                    if (allowManagingUserRoles)
                    {
                        foreach (var userRole in allUserRoles)
                        {
                            if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                            {
                                //new role
                                if (user.UserRoles.Where(cr => cr.Id == userRole.Id).Count() == 0)
                                    user.UserRoles.Add(userRole);
                            }
                            else
                            {
                                //removed role
                                if (user.UserRoles.Where(cr => cr.Id == userRole.Id).Count() > 0)
                                    user.UserRoles.Remove(userRole);
                            }
                        }
                        _userService.UpdateUser(user);
                    }
                    //ensure that a user with a vendor associated is not in "Administrators" role
                    //otherwise, he won't have access to the other functionality in admin area
                    if (user.IsAdmin() && user.VendorId > 0)
                    {
                        user.VendorId = 0;
                        _userService.UpdateUser(user);
                        NotifyError(_localizationService.GetResource("Admin.Users.Users.AdminCouldNotbeVendor"));
                    }

                    //ensure that a user in the Vendors role has a vendor account associated.
                    //otherwise, he will have access to ALL products
                    if (user.IsVendor() && user.VendorId == 0)
                    {
                        var vendorRole = user
                            .UserRoles
                            .FirstOrDefault(x => x.SystemName == SystemUserRoleNames.Vendors);
                        user.UserRoles.Remove(vendorRole);
                        _userService.UpdateUser(user);
                        NotifyError(_localizationService.GetResource("Admin.Users.Users.CannotBeInVendoRoleWithoutVendorAssociated"));
                    }

                    _eventPublisher.Publish(new ModelBoundEvent(model, user, form));

                    //activity log
                    _userActivityService.InsertActivity("EditUser", _localizationService.GetResource("ActivityLog.EditUser"), user.Id);

                    NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Updated"));
                    return continueEditing ? RedirectToAction("Edit", user.Id) : RedirectToAction("List");
                }
                catch (Exception exc)
                {
                    NotifyError(exc.Message, false);
                }
            }


            //If we got this far, something failed, redisplay form
            model.UserNamesEnabled = _userSettings.UserNamesEnabled;
            model.AllowUsersToChangeUserNames = _userSettings.AllowUsersToChangeUserNames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = ((VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId))
                 .GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
            model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
            model.LastIpAddress = model.LastIpAddress;
            model.LastVisitedPage = user.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;
            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                            model.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                    else
                        model.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

                }
            }
            PrepareVendorsModel(model);
            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.AllowManagingUserRoles = allowManagingUserRoles;
            //reward points gistory
            //  model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var changePassRequest = new ChangePasswordRequest(model.Email,
                    false, _userSettings.DefaultPasswordFormat, model.Password);
                var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                if (changePassResult.Success)
                    NotifySuccess(_localizationService.GetResource("Admin.Users.Users.PasswordChanged"));
                else
                    foreach (var error in changePassResult.Errors)
                        NotifyError(error);
            }

            return RedirectToAction("Edit", user.Id);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsValid")]
        public ActionResult MarkVatNumberAsValid(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            _genericAttributeService.SaveAttribute(user,
                SystemUserAttributeNames.VatNumberStatusId,
                (int)VatNumberStatus.Valid);

            return RedirectToAction("Edit", user.Id);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsInvalid")]
        public ActionResult MarkVatNumberAsInvalid(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            _genericAttributeService.SaveAttribute(user,
                SystemUserAttributeNames.VatNumberStatusId,
                (int)VatNumberStatus.Invalid);

            return RedirectToAction("Edit", user.Id);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                _userService.DeleteUser(user);

                //remove newsletter subscriptions (if exists)
                var subscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(user.Email, 0, int.MaxValue, true);

                foreach (var subscription in subscriptions)
                {
                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
                }

                //activity log
                _userActivityService.InsertActivity("DeleteUser", _localizationService.GetResource("ActivityLog.DeleteUser"), user.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
                return RedirectToAction("Edit", new { id = user.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public ActionResult Impersonate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowUserImpersonation))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            if (!_workContext.CurrentUser.IsAdmin() && user.IsAdmin())
            {
                NotifyError("A non-admin user cannot impersonate as an administrator");
                return RedirectToAction("Edit", user.Id);
            }

            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentUser,
                SystemUserAttributeNames.ImpersonatedUserId, user.Id);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public ActionResult SendEmail(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(user.Email))
                    throw new WorkException("User email is empty");
                if (!user.Email.IsEmail())
                    throw new WorkException("User email is not valid");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new WorkException("Email subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new WorkException("Email body is empty");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new WorkException("Email account can't be loaded");

                var email = new QueuedEmail()
                {
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = user.GetFullName(),
                    To = user.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                _queuedEmailService.InsertQueuedEmail(email);
                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        public ActionResult SendPm(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                if (!_forumSettings.AllowPrivateMessages)
                    throw new WorkException("Private messages are disabled");
                if (user.IsGuest())
                    throw new WorkException("User should be registered");
                if (String.IsNullOrWhiteSpace(model.SendPm.Subject))
                    throw new WorkException("PM subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendPm.Message))
                    throw new WorkException("PM message is empty");


                var privateMessage = new PrivateMessage
                {
                    SiteId = _siteContext.CurrentSite.Id,
                    ToUserId = user.Id,
                    FromUserId = _workContext.CurrentUser.Id,
                    Subject = model.SendPm.Subject,
                    Text = model.SendPm.Message,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    IsRead = false,
                    CreatedOnUtc = DateTime.UtcNow
                };

                _forumService.InsertPrivateMessage(privateMessage);
                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.SendPM.Sent"));
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        #endregion

        #region Reward points history

        //public ActionResult RewardPointsHistorySelect(int userId)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
        //        return AccessDeniedView();

        //    var user = _userService.GetUserById(userId);
        //    if (user == null)
        //        throw new ArgumentException("No user found with the specified id");

        //    var model = new List<UserModel.RewardPointsHistoryModel>();
        //    foreach (var rph in user.RewardPointsHistory.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id))
        //    {
        //        model.Add(new UserModel.RewardPointsHistoryModel()
        //            {
        //                Points = rph.Points,
        //                PointsBalance = rph.PointsBalance,
        //                Message = rph.Message,
        //                CreatedOn = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc)
        //            });
        //    }
        //    var gridModel = new GridModel<UserModel.RewardPointsHistoryModel>
        //    {
        //        Data = model,
        //        Total = model.Count
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}

        //[ValidateInput(false)]
        //public ActionResult RewardPointsHistoryAdd(int userId, int addRewardPointsValue, string addRewardPointsMessage)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
        //        return AccessDeniedView();

        //    var user = _userService.GetUserById(userId);
        //    if (user == null)
        //        return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

        //    user.AddRewardPointsHistoryEntry(addRewardPointsValue, addRewardPointsMessage);
        //    _userService.UpdateUser(user);

        //    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region Addresses

        public ActionResult AddressesSelect(int userId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var addresses = user.Addresses.OrderByDescending(a => a.CreatedOnUtc).ThenByDescending(a => a.Id).ToList();
            var total = addresses.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = addresses.Select(x =>
                {
                    var model = x.ToModel();
                    var addressHtmlSb = new StringBuilder("<div>");
                    if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(model.Company))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Company));
                    if (_addressSettings.StreetAddressEnabled && !String.IsNullOrEmpty(model.Address1))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address1));
                    if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(model.Address2))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address2));
                    if (_addressSettings.StateProvinceEnabled && !String.IsNullOrEmpty(model.StateProvinceName))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.StateProvinceName));
                    if (_addressSettings.CityEnabled && !String.IsNullOrEmpty(model.CityeName))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.CityeName));
                    if (_addressSettings.DistrictEnabled && !String.IsNullOrEmpty(model.DistrictName))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.DistrictName));
                    if (_addressSettings.ZipPostalCodeEnabled && !String.IsNullOrEmpty(model.ZipPostalCode))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.ZipPostalCode));
                    if (_addressSettings.CountryEnabled && !String.IsNullOrEmpty(model.CountryName))
                        addressHtmlSb.AppendFormat("{0}", Server.HtmlEncode(model.CountryName));
                    addressHtmlSb.Append("</div>");
                    model.AddressHtml = addressHtmlSb.ToString();
                    return model;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };

        }

        public ActionResult AddressDelete(int userId, int addressId, DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var address = user.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            user.RemoveAddress(address);
            _userService.UpdateUser(user);
            //now delete the address record
            _addressService.DeleteAddress(address);
            return Json(new { Result = true, Message = _localizationService.GetResource("Admin.Address.AddressDelete") }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddressCreate(int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserAddressModel();
            model.Address = new AddressModel();
            model.UserId = userId;
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.ValidateEmailAddress = _addressSettings.ValidateEmailAddress;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressCreate(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                user.Addresses.Add(address);
                _userService.UpdateUser(user);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Addresses.Added"));
                return RedirectToAction("AddressEdit", new { addressId = address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            model.UserId = user.Id;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
            {
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            }
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                {
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
                }
            }
            else
            {
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            }
            return View(model);
        }

        public ActionResult AddressEdit(int addressId, int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            var model = new UserAddressModel();
            model.UserId = userId;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.ValidateEmailAddress = _addressSettings.ValidateEmailAddress;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Users.Addresses.Updated"));
                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            model.UserId = user.Id;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        #endregion

    }
}
