using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Seller.Validators.Articles;
 
namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    public class ArticleTagModel : EntityModelBase, ILocalizedModel<ArticleTagLocalizedModel>
    {
        public ArticleTagModel()
        {
            Locales = new List<ArticleTagLocalizedModel>();
        }
        [LangResourceDisplayName("Admin.ContentManagement.ArticleTags.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleTags.Fields.ArticleCount", "内容数量")]
        public int ArticleCount { get; set; }

        public IList<ArticleTagLocalizedModel> Locales { get; set; }
    }

    public class ArticleTagLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleTags.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }
    }
}