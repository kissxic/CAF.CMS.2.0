using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Common;
using CAF.WebSite.Application.Services.Users;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using CAF.Mvc.JQuery.Datatables.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Helpers;
using CAF.WebSite.Application.Services.Seo;
using CAF.Infrastructure.Core.Domain.Cms.Agents;
using CAF.Infrastructure.Core.Domain.Cms.Articles;

namespace CAF.WebSite.Mvc.Seller.Controllers
{
    public class AgentController : SellerAdminControllerBase
    {
        #region Fields
        private readonly IArticleCategoryService _categoryService;
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly IUserContentService _userContentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public AgentController(
            IArticleCategoryService categoryService,
            IArticleService articleService,
            IUserService userService,
            IUserContentService userContentService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            this._categoryService = categoryService;
            this._articleService = articleService;
            this._userService = userService;
            this._userContentService = userContentService;
            this._dateTimeHelper = dateTimeHelper;
            this._workContext = workContext;
        }
        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {

            return View();
        }

        [HttpPost]
        public ActionResult List(DataTablesParam dataTableParam)
        {

            //load all article comments
            var agentContent = _userContentService.GetAllUserContent<AgencyContent>(0, _workContext.CurrentUser.VendorId, null, dataTableParam.PageIndex, dataTableParam.PageSize);
            var total = agentContent.TotalCount;
            var result = new DataTablesData
            {
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                sEcho = dataTableParam.sEcho,
                aaData = agentContent.Select(agencyContent =>
                {
                    var agentModel = new ArticleAgnetModel();
                    var user = _userService.GetUserById(agencyContent.UserId ?? 0);
                    var article = _articleService.GetArticleById(agencyContent.ArticleId ?? 0) ?? new Article();

                    agentModel.Id = agencyContent.Id;
                    agentModel.ArticleId = article.Id;
                    agentModel.ArticleTitle = article.Title;
                    agentModel.Url = Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme);

                    agentModel.IpAddress = agencyContent.IpAddress;
                    agentModel.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(agencyContent.CreatedOnUtc, DateTimeKind.Utc);
                    agentModel.Message = CAF.Infrastructure.Core.Html.HtmlUtils.FormatText(agencyContent.Message, false, true, false, false, false, false);
                    agentModel.TrueName = agencyContent.TrueName;
                    agentModel.TelePhone = agencyContent.TelePhone;
                    agentModel.Mobile = agencyContent.Mobile;
                    agentModel.QQ = agencyContent.QQ;
                    agentModel.Email = agencyContent.Email;
                    agentModel.ProvinceName = agencyContent.ProvinceName;
                    agentModel.CityName = agencyContent.CityName;
                    agentModel.DistributionChannelId = agencyContent.DistributionChannelId;
                    agentModel.AgentPropertyId = agencyContent.AgentPropertyId;
                    agentModel.Message = agencyContent.Message;
                    agentModel.ShowAuthId = agencyContent.ShowAuthId;
                    agentModel.OtherChannel = agencyContent.OtherChannel;
                    agentModel.IsApproved = true;
                    if (user == null)
                        agentModel.UserName = "".NaIfEmpty();
                    else
                        agentModel.UserName = user.GetFullName();

                    return agentModel;
                }).Cast<object>().ToArray(),
            };
            return new JsonResult
            {
                Data = result
            };
        }

        public ActionResult Detail(int id)
        {

            var agencyContent = _userContentService.GetUserContentById(id) as AgencyContent;
            if (agencyContent == null)
                throw new ArgumentException("No agent found with the specified id");
            var article = _articleService.GetArticleById(agencyContent.ArticleId ?? 0) ?? new Article();

            var agentModel = new ArticleAgnetModel();
            var user = _userService.GetUserById(agencyContent.UserId ?? 0);
            agentModel.Id = agencyContent.Id;
            agentModel.ArticleId = article.Id;
            agentModel.ArticleTitle = article.Title;
            agentModel.Url = Url.RouteUrl("Article", new { SeName = article.GetSeName() }, Request.Url.Scheme);

            agentModel.IpAddress = agencyContent.IpAddress;
            agentModel.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(agencyContent.CreatedOnUtc, DateTimeKind.Utc);
            agentModel.Message = CAF.Infrastructure.Core.Html.HtmlUtils.FormatText(agencyContent.Message, false, true, false, false, false, false);
            agentModel.TrueName = agencyContent.TrueName;
            agentModel.TelePhone = agencyContent.TelePhone;
            agentModel.Mobile = agencyContent.Mobile;
            agentModel.QQ = agencyContent.QQ;
            agentModel.Email = agencyContent.Email;
            agentModel.ProvinceName = agencyContent.ProvinceName;
            agentModel.CityName = agencyContent.CityName;
            agentModel.DistributionChannelId = agencyContent.DistributionChannelId;
            agentModel.AgentPropertyId = agencyContent.AgentPropertyId;
            agentModel.Message = agencyContent.Message;
            agentModel.ShowAuth = ((ShowAuthType)agencyContent.ShowAuthId).ToString();
            agentModel.OtherChannel = agencyContent.OtherChannel;
            agentModel.IsApproved = true;
            if (user == null)
                agentModel.UserName = "".NaIfEmpty();
            else
                agentModel.UserName = user.GetFullName();
            return View(agentModel);
        }

        public ActionResult Delete(int id)
        {


            var comment = _userContentService.GetUserContentById(id) as AgencyContent;
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var articlePost = comment.Article;
            _userContentService.DeleteUserContent(comment);
            //update totals
            _articleService.UpdateCommentTotals(articlePost);

            return Json(new { Result = true });
        }


        #endregion

    }
}