using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Mvc.Admin.Validators.Manufacturers;

namespace CAF.WebSite.Mvc.Admin.Models.Manufacturers
{
	[Validator(typeof(ManufacturerValidator))]
    public class ManufacturerModel : EntityModelBase, ILocalizedModel<ManufacturerLocalizedModel>
    {
        public ManufacturerModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }

            Locales = new List<ManufacturerLocalizedModel>();
        }

		public int GridPageSize { get; set; }

		[LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.Description", "描述")]
        [AllowHtml]
        public string Description { get; set; }


        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.MetaKeywords", "关键字")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.MetaDescription", "关键字描述")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.MetaTitle", "关键字标题")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.SeName", "搜索引擎友好的URL别名")]
        [AllowHtml]
        public string SeName { get; set; }

        [UIHint("Picture")]
        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.Picture", "图片")]
        public int PictureId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.PageSize", "页数")]
        public int PageSize { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.AllowUsersToSelectPageSize", "允许用户选择页数")]
        public bool AllowUsersToSelectPageSize { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.PageSizeOptions", "页数数组")]
        public string PageSizeOptions { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.Published", "发布")]
        public bool Published { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.Deleted", "删除")]
        public bool Deleted { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.DisplayOrder", "排序")]
        public int DisplayOrder { get; set; }

		[LangResourceDisplayName("Common.CreatedOn", "创建时间")]
		public DateTime? CreatedOnUtc { get; set; }

		[LangResourceDisplayName("Common.ModifiedOnUtc", "更新时间")]
		public DateTime? ModifiedOnUtc { get; set; }
        
        public IList<ManufacturerLocalizedModel> Locales { get; set; }

      
    }

    public class ManufacturerLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.Name", "名称")]
        [AllowHtml]
        public string Name { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.Description", "描述")]
        [AllowHtml]
        public string Description {get;set;}

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.MetaKeywords", "关键字")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.MetaDescription", "关键字描述")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.MetaTitle", "关键字标题")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Manufacturers.Fields.SeName", "搜索引擎友好的URL别名")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}