using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Models.Media;
using CAF.WebSite.Mvc.Models.ArticleCatalog;

namespace CAF.WebSite.Mvc.Models.Articles
{
    /// <summary>
    /// 系统类型
    /// </summary>
    [Serializable]
    public partial class ProductCategoryModel : EntityModelBase
    {
        public ProductCategoryModel()
        {
            PictureModel = new PictureModel();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<MenuItem>();
            PagingFilteringContext = new ArticleCatalogPagingFilteringModel();
        }

        /// <summary>
        ///类别标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 调用别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 父类别ID
        /// </summary>
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryBreadcrumb { get; set; }

        /// <summary>
        /// 深度
        /// </summary>
        public int Depth { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>

        public int? PictureId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>

        public string Description { get; set; }


        /// <summary>
        /// 获取或设置一个值,该值指示该实体是否发表
        /// </summary>
        public bool Published { get; set; }

        public string Breadcrumb { get; set; }
        /// <summary>
        ///排序数字
        /// </summary>
        public int DisplayOrder { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? ModifiedOnUtc { get; set; }

        public string BottomDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public PictureModel PictureModel { get; set; }

        public ArticleCatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<MenuItem> CategoryBreadcrumb { get; set; }

        public bool DisplayFilter { get; set; }
        public bool ShowSubcategoriesAboveArticleLists { get; set; }


        public IList<SubCategoryModel> SubCategories { get; set; }

        public IList<ArticleOverviewModel> Articles { get; set; }

        #region Nested Classes

        public partial class SubCategoryModel : EntityModelBase
        {
            public SubCategoryModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public PictureModel PictureModel { get; set; }
        }

        #endregion
    }

}
