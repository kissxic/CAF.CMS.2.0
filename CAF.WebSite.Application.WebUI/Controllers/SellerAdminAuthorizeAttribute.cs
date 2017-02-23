using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Sellers;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace CAF.WebSite.Application.WebUI.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class SellerAdminAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        public IPermissionService PermissionService { get; set; }
        public IWebHelper WebHelper { get; set; }
        public IWorkContext WorkContext { get; set; }
        private void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        private IEnumerable<SellerAdminAuthorizeAttribute> GetSellerAdminAuthorizeAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(SellerAdminAuthorizeAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(SellerAdminAuthorizeAttribute), true))
                .OfType<SellerAdminAuthorizeAttribute>();
        }

        private bool IsAdminPageRequested(AuthorizationContext filterContext)
        {
            var adminAttributes = GetSellerAdminAuthorizeAttributes(filterContext.ActionDescriptor);
            if (adminAttributes != null && adminAttributes.Any())
                return true;
            return false;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [SellerAdminAuthorize] attribute when a child action cache is active");

            if (IsAdminPageRequested(filterContext))
            {
                if (!this.HasAdminAccess(filterContext))
                {
                    this.HandleUnauthorizedRequest(filterContext);
                }
                //else if (WorkContext.CurrentUser.IsAdmin())
                //{

                //}
                else if (CheckRegisterInfo(filterContext))
                {

                }
            }

        }
        private bool CheckRegisterInfo(AuthorizationContext filterContext)
        {

            bool flag = true;
            if (filterContext.IsChildAction)
            {
                return flag;
            }

            filterContext.RouteData.Values["controller"].ToString().ToLower();
            string lower = filterContext.RouteData.Values["action"].ToString().ToLower();
            filterContext.RouteData.DataTokens["area"].ToString().ToLower();
            if (WorkContext.CurrentVendor == null)
            {
                RedirectResult action = new RedirectResult("~/Member/OpenSeller");
                filterContext.Result = action;
                flag = false;
            }
            else
            {
                int valueOrDefault = (int)WorkContext.CurrentVendor.VendorStage;
                VendorStage? stage = WorkContext.CurrentVendor.VendorStage;
                if ((stage.GetValueOrDefault() != VendorStage.Finish ? true : !stage.HasValue) && filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() != "POST" && lower != string.Concat("EditProfile", valueOrDefault).ToLower())
                {
                    RedirectResult action = new RedirectResult("~/Member/" + string.Concat("EditProfileAudit", valueOrDefault));
                    filterContext.Result = action;
                    flag = false;
                }
            }
            return flag;
        }
        public virtual bool HasAdminAccess(AuthorizationContext filterContext)
        {
            var result = PermissionService.Authorize(StandardPermissionProvider.AccessSellerAdminPanel);
            return result;
        }
    }
}
