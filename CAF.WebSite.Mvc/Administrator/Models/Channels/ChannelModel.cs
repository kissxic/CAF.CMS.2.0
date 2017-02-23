
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Admin.Validators.Channels;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Models;
using System.Web.Mvc;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Mvc.Admin.Models.Members;
using static CAF.WebSite.Mvc.Admin.Models.Articles.ArticleModel;
using CAF.WebSite.Mvc.Admin.Models.Articles;

namespace CAF.WebSite.Mvc.Admin.Models.Channels
{
    /// <summary>
    /// 系统频道表
    /// </summary>
    [Serializable]
    [Validator(typeof(ChannelValidator))]
    public partial class ChannelModel : EntityModelBase
    {
        public ChannelModel()
        {
            AvailableProductCategorys = new List<SelectListItem>();
            AvailableModelTemplates = new List<SelectListItem>();
            AvailableDetailModelTemplates = new List<SelectListItem>();
            ChannelSpecificationAttributeModels = new List<ChannelSpecificationAttributeModel>();
            AddSpecificationAttributeModel = new AddProductSpecificationAttributeModel();
        }

        /// <summary>
        /// 显示扩展信息
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ShowExtendedAttribute", "扩展属性")]
        public bool ShowExtendedAttribute { get; set; }
        /// <summary>
        /// 显示内容属性
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ShowSpecificationAttributes", "内容属性")]
        public bool ShowSpecificationAttributes { get; set; }
        /// <summary>
        /// 显示库存
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ShowInventory", "库存")]
        public bool ShowInventory { get; set; }
        /// <summary>
        /// 显示价格
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ShowPrice", "价格")]
        public bool ShowPrice { get; set; }
        /// <summary>
        /// 显示规格属性
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ShowAttributes", "规格")]
        public bool ShowAttributes { get; set; }
        /// <summary>
        /// 显示推荐
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ShowPromotion", "推荐")]
        public bool ShowPromotion { get; set; }

        /// <summary>
        ///频道名称
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.Name")]
        public string Name { get; set; }

        /// <summary>
        ///频道标题
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.Title")]
        public string Title { get; set; }
        /// <summary>
        ///频道图标
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.IconName","图标名称")]
        public string IconName { get; set; }
        
        /// <summary>
        ///排序数字
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.LimitNum","会员发布限制数量")]
        public int LimitNum { get; set; }
        /// <summary>
        ///排序数字
        /// </summary>
        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Channels. CategoryShowType","栏目分类显示方式")]
        public int CategoryShowTypeId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Channels. CategoryShowType", "栏目分类显示方式")]
        [AllowHtml]
        public string CategoryShowTypeName { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Channels. ProductCategoryShowTypeId", "商品分类显示方式")]
        public int ProductCategoryShowTypeId { get; set; }
        [LangResourceDisplayName("Admin.ContentManagement.Channels. ProductCategoryShowTypeName", "商品分类显示方式")]
        [AllowHtml]
        public string ProductCategoryShowTypeName { get; set; }

        //MemberGrade mapping
        [LangResourceDisplayName("Admin.Common.MemberGrade.LimitedTo")]
        public bool LimitedToMemberGrades { get; set; }
        [LangResourceDisplayName("Admin.Common.MemberGrade.AvailableFor")]
        public List<MemberGradeModel> AvailableMemberGrades { get; set; }
        public int[] SelectedMemberGradeIds { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.Channels.Fields.ProductCategoryId","商品分类")]
        [AllowHtml]
        public string ProductCategoryId { get; set; }
        public IList<SelectListItem> AvailableProductCategorys { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ModelTemplate")]
        [AllowHtml]
        public int ModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableModelTemplates { get; set; }

        [LangResourceDisplayName("Admin.ContentManagement.ArticleCategory.Fields.ModelDetailTemplate")]
        [AllowHtml]
        public int DetailModelTemplateId { get; set; }
        public IList<SelectListItem> AvailableDetailModelTemplates { get; set; }

        /// <summary>
        /// 内容属性
        /// </summary>
        public AddProductSpecificationAttributeModel AddSpecificationAttributeModel { get; set; }
        public IList<ChannelSpecificationAttributeModel> ChannelSpecificationAttributeModels { get; set; }
        /// <summary>
        /// 内容属性字符串
        /// </summary>
        public string SpaValues { get; set; }
        /// <summary>
        /// 记录是否修改过分类
        /// </summary>
        public bool IsChangeCategory { get; set; }

    }
}
