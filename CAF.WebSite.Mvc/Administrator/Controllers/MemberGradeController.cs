using CAF.Infrastructure.Core.Exceptions;
using CAF.Infrastructure.Core.Email;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.WebUI.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Members;
using CAF.Infrastructure.Core.Domain.Cms.Members;
using CAF.WebSite.Mvc.Admin.Models.Members;
using CAF.Infrastructure.Core.Configuration;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
	public class MemberGradeController : AdminControllerBase
	{
        private readonly IMemberGradeService _memberGradeService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
		private readonly ISiteContext _siteContext;
        private readonly IEmailSender _emailSender;
        private readonly MemberGradeSettings _memberGradeSettings;
        private readonly IPermissionService _permissionService;

		public MemberGradeController(IMemberGradeService memberGradeService,
            ILocalizationService localizationService, ISettingService settingService,
			IEmailSender emailSender, ISiteContext siteContext,
            MemberGradeSettings memberGradeSettings, IPermissionService permissionService)
		{
            this._memberGradeService = memberGradeService;
            this._localizationService = localizationService;
            this._memberGradeSettings = memberGradeSettings;
            this._emailSender = emailSender;
            this._settingService = settingService;
			this._siteContext = siteContext;
            this._permissionService = permissionService;
		}

		public ActionResult List(string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMemberGrades))
                return AccessDeniedView();

			//mark as default email account (if selected)
			if (!String.IsNullOrEmpty(id))
			{
				int defaultMemberGradeId = Convert.ToInt32(id);
				var defaultMemberGrade = _memberGradeService.GetMemberGradeById(defaultMemberGradeId);
				if (defaultMemberGrade != null)
				{
					_memberGradeSettings.DefaultMemberGradeId = defaultMemberGradeId;
					_settingService.SaveSetting(_memberGradeSettings);
				}
			}

			return View();
		}

		[HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMemberGrades))
                return AccessDeniedView();

            var memberGradeModels = _memberGradeService.GetAllMemberGrades()
                                    .Select(x => x.ToModel())
                                    .ToList();
            foreach (var eam in memberGradeModels)
                eam.IsDefault = eam.Id == _memberGradeSettings.DefaultMemberGradeId;

            var total = memberGradeModels.Count();
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = memberGradeModels.Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
          
		}

		public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMemberGrades))
                return AccessDeniedView();

            var model = new MemberGradeModel();
			return View(model);
		}
        
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
		public ActionResult Create(MemberGradeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMemberGrades))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var memberGrade = model.ToEntity();
                _memberGradeService.InsertMemberGrade(memberGrade);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.MemberGrades.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = memberGrade.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMemberGrades))
                return AccessDeniedView();

			var memberGrade = _memberGradeService.GetMemberGradeById(id);
            if (memberGrade == null)
                //No email account found with the specified id
                return RedirectToAction("List");

			return View(memberGrade.ToModel());
		}
        
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(MemberGradeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMemberGrades))
                return AccessDeniedView();

            var memberGrade = _memberGradeService.GetMemberGradeById(model.Id);
            if (memberGrade == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                memberGrade = model.ToEntity(memberGrade);
                _memberGradeService.UpdateMemberGrade(memberGrade);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.MemberGrades.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = memberGrade.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
		}

     [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMemberGrades))
                return AccessDeniedView();

            var memberGrade = _memberGradeService.GetMemberGradeById(id);
            if (memberGrade == null)
                //No email account found with the specified id
                return RedirectToAction("List");

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.MemberGrades.Deleted"));
            _memberGradeService.DeleteMemberGrade(memberGrade);
            return RedirectToAction("List");
        }
	}
}
