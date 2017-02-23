using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Mvc.Admin.Validators.Categorys;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Attributes;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Admin.Models.Sites;
using CAF.WebSite.Mvc.Admin.Models.Users;
namespace CAF.WebSite.Mvc.Admin.Models.Articles
{
    /// <summary>
    /// 系统类型
    /// </summary>
    [Serializable]
    [Validator(typeof(ProductCategoryValidator))]
    public partial class ProductCategoryModel : TabbableModel, ILocalizedModel<ProductCategoryLocalizedModel>
    {
        public ProductCategoryModel()
        {
            Locales = new List<ProductCategoryLocalizedModel>();
        }

        /// <summary>
        ///类别标题
        /// </summary>
        [DataMember]
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Name","名称")]
        public string Name { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Alias", "别名标识")]
        [AllowHtml]
        public string Alias { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.FullName", "全名")]
        [AllowHtml]
        public string FullName { get; set; }
        /// <summary>
        /// 父类别ID
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Parent", "父级")]
        [AllowHtml]
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryBreadcrumb { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Depth", "深度")]
        public int Depth { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Path", "路径")]
        public string Path { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.SeName", "SeoUrl友好名称")]
        [AllowHtml]
        public string SeName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Picture", "图片")]
        [AllowHtml]
        [UIHint("Picture")]
        public int PictureId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Description", "描述")]
        [AllowHtml]
        public string Description { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.PageSize","每页显示数量")]
        public int PageSize { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.AllowUsersToSelectPageSize", "是否允许用户选择每页显示数量")]
        public bool AllowUsersToSelectPageSize { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.PageSizeOptions","每页数量设置")]
        public string PageSizeOptions { get; set; }
        /// <summary>
        /// 获取或设置一个值,该值指示该实体是否发表
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Published", "发布")]
        public bool Published { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Name", "分类导航")]
        public string Breadcrumb { get; set; }
        /// <summary>
        ///排序数字
        /// </summary>
        [DataMember]
        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }

        [LangResourceDisplayName("Common.CreatedOn", "创建时间")]
        public DateTime? CreatedOnUtc { get; set; }

        [LangResourceDisplayName("Common.UpdatedOn", "修改时间")]
        public DateTime? ModifiedOnUtc { get; set; }

        public ProductCategory Category { get; set; }

        public IList<ProductCategoryLocalizedModel> Locales { get; set; }
 
    }

    public class ProductCategoryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.FullName", "全名")]
        public string FullName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.Description", "描述")]
        [AllowHtml]
        public string Description { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ProductCategory.Fields.SeName","SeoUrl友好名称")]
        [AllowHtml]
        public string SeName { get; set; }
    }


}
