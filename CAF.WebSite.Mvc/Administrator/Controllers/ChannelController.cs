using CAF.Infrastructure.Core;
using CAF.Mvc.JQuery.Datatables.Core;
using CAF.WebSite.Application.Services.Articles;
using CAF.WebSite.Application.Services.Channels;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.WebUI.Controllers;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.Infrastructure.Core.Logging;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using CAF.WebSite.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.WebSite.Application.Services.Security;
using CAF.WebSite.Application.Services.Members;
using Newtonsoft.Json;

namespace CAF.WebSite.Mvc.Admin.Controllers
{
    public class ChannelController : AdminControllerBase
    {
        #region Fields
        private readonly IProductCategoryService _categoryService;
        private readonly IModelTemplateService _modelTemplateService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly UserSettings _userSettings;
        private readonly IChannelService _channelService;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IMemberGradeService _memberGradeService;
        private readonly IMemberGradeMappingService _memberGradeMappingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        #endregion

        #region Ctor

        public ChannelController(
            IModelTemplateService modelTemplateService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            UserSettings userSettings,
            IChannelService channelService,
            IUserActivityService userActivityService,
            IPermissionService permissionService,
            IProductCategoryService categoryService,
            IMemberGradeService memberGradeService,
            IMemberGradeMappingService memberGradeMappingService,
            ISpecificationAttributeService specificationAttributeService
             )
        {
            this._categoryService = categoryService;
            this._modelTemplateService = modelTemplateService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._userSettings = userSettings;
            this._channelService = channelService;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._memberGradeService = memberGradeService;
            this._memberGradeMappingService = memberGradeMappingService;
            this._specificationAttributeService = specificationAttributeService;

        }
        #endregion


        #region Utilities

        private void PrepareChannelModel(ChannelModel model, Channel channel, bool excludeProperties)
        {
            var channelCategories = new List<int>();
            if (channel != null)
            {
                var channelItem = _channelService.GetChannelById(channel.Id);
                channelCategories = channelItem.ProductCategorys.Select(x =>
               {
                   return x.Id;
               }).ToList();
                model.ProductCategoryId = string.Join(",", channelCategories);
            }

        }

        [NonAction]
        private void PrepareMemberGradeMappingModel(ChannelModel model, Channel channel, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableMemberGrades = _memberGradeService
                .GetAllMemberGrades()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (channel != null)
                {
                    model.SelectedMemberGradeIds = _memberGradeMappingService.GetMemberGradesIdsWithAccess(channel);
                }
                else
                {
                    model.SelectedMemberGradeIds = new int[0];
                }
            }

        }
        [NonAction]
        protected void PrepareTemplatesModel(ChannelModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _modelTemplateService.GetAllModelTemplates();
            var listTemplate = templates.Where(p => p.TemplageTypeId == (int)TemplateTypeFormat.List).ToList();
            foreach (var template in listTemplate)
            {
                model.AvailableModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
            var detailTemplate = templates.Where(p => p.TemplageTypeId == (int)TemplateTypeFormat.Detail).ToList();
            foreach (var template in detailTemplate)
            {
                model.AvailableDetailModelTemplates.Add(new SelectListItem()
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }


        /// <summary>
        /// 装配文档属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="article"></param>
        private void PrerpareArticleSpecifications(ChannelModel model, Channel channel)
        {
            if (!model.ShowSpecificationAttributes) return;

            #region specification attributes
            //specification attributes
            var specificationAttributes = _specificationAttributeService.GetSpecificationAttributes().ToList();
            for (int i = 0; i < specificationAttributes.Count; i++)
            {
                var sa = specificationAttributes[i];
                model.AddSpecificationAttributeModel.AvailableAttributes.Add(new SelectListItem { Text = sa.Name, Value = sa.Id.ToString() });
                if (i == 0)
                {
                    //attribute options
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(sa.Id))
                    {
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem { Text = sao.Name, Value = sao.Id.ToString() });
                    }
                }
            }
            //default specs values
            model.AddSpecificationAttributeModel.ShowOnProductPage = true;
            #endregion

            if (channel != null)
            {
                var productrSpecs = _specificationAttributeService.GetChannelSpecificationAttributesById(channel.Id);

                var productrSpecsModel = productrSpecs
                    .Select(x =>
                    {
                        var psaModel = new ChannelSpecificationAttributeModel
                        {
                            Id = x.Id,
                            SpecificationAttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                            SpecificationAttributeOptionName = x.SpecificationAttributeOption.Name,
                            SpecificationAttributeOptionAttributeId = x.SpecificationAttributeOption.SpecificationAttributeId,
                            SpecificationAttributeOptionId = x.SpecificationAttributeOptionId,
                            AllowFiltering = x.AllowFiltering,
                            ShowOnArticlePage = x.ShowOnArticlePage,
                            DisplayOrder = x.DisplayOrder
                        };
                        return psaModel;
                    })
                    .ToList();

                foreach (var attr in productrSpecsModel)
                {
                    var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(attr.SpecificationAttributeOptionAttributeId);

                    foreach (var option in options)
                    {
                        attr.SpecificationAttributeOptions.Add(new ChannelSpecificationAttributeModel.SpecificationAttributeOption
                        {
                            id = option.Id,
                            name = option.Name,
                            text = option.Name,
                            select = attr.SpecificationAttributeOptionId == option.Id
                        });
                    }

                    attr.SpecificationAttributeOptionsJsonString = JsonConvert.SerializeObject(attr.SpecificationAttributeOptions);
                }
                model.ChannelSpecificationAttributeModels = productrSpecsModel;
            }
        }

        [NonAction]
        protected void UpdateProductCategorys(ChannelModel model, Channel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");
            var categories = _categoryService.GetAllCategories(showHidden: true);
            var productCategoryIds = new List<int>();
            if (!model.ProductCategoryId.IsEmpty())
                productCategoryIds = model.ProductCategoryId.TrimEnd(',').Split(',').Select(x => { return x.ToInt(); }).ToList();
            foreach (var item in categories)
            {
                //判断是否已经选择
                if (productCategoryIds.Where(pc => pc == item.Id).Any())
                {
                    if (channel.ProductCategorys.Where(x => x.Id == item.Id).FirstOrDefault() == null)
                    {
                        channel.ProductCategorys.Add(item);
                    }

                }
                else
                {
                    if (channel.ProductCategorys.Where(x => x.Id == item.Id).FirstOrDefault() != null)
                    {
                        channel.ProductCategorys.Remove(item);
                    }
                }
            }
        }



        [NonAction]
        protected void UpdateMemberGradeMappings(ChannelModel model, Channel channel)
        {
            //会员等级限制
            channel.LimitedToMemberGrades = model.LimitedToMemberGrades;
            _memberGradeMappingService.SaveMemberGradeMappings<Channel>(channel, model.SelectedMemberGradeIds);
        }

        /// <summary>
        /// 更新文档属性数据
        /// </summary>
        /// <param name="article"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void UpdateArticleSpecifications(ChannelModel model, Channel channel)
        {

            var p = channel;
            var m = model;
            if (model.SpaValues.IsEmpty())
                return;
            var channelSpecificationAttributes = JsonConvert.DeserializeObject<List<ChannelSpecificationAttribute>>(model.SpaValues);
            var list = new List<ChannelSpecificationAttribute>();
            foreach (var item in channelSpecificationAttributes)
            {
                var psa = new ChannelSpecificationAttribute()
                {
                    SpecificationAttributeOptionId = item.SpecificationAttributeOptionId,
                    ChannelId = channel.Id,
                    AllowFiltering = item.AllowFiltering,
                    ShowOnArticlePage = item.ShowOnArticlePage,
                    DisplayOrder = item.DisplayOrder,
                };
                list.Add(psa);
            }
            _specificationAttributeService.InsertChannelSpecificationAttribute(channel.Id, list);
        }
        #endregion Utilities

        #region List
        // GET: Edit
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
            var channel = _channelService.GetAllChannelQ();

            return DataTablesResult.Create(channel.Select(a =>
               new ChannelModel()
               {
                   Id = a.Id,
                   Name = a.Name,
                   Title = a.Title,
                   DisplayOrder = a.DisplayOrder,


               }), dataTableParam,
             uv => new
             {
                 Title = "<b>" + uv.Title + "</b>"
             }
            );
        }
        public ActionResult AjaxProductCategory(int channelId)
        {
            var channelCategories = new List<int>();

            var channelItem = _channelService.GetChannelById(channelId);
            if (channelItem != null)
            {
                channelCategories = channelItem.ProductCategorys.Select(x =>
                {
                    return x.Id;
                }).ToList();
            }
            var categories = _categoryService.GetAllProductCategoriesByParentCategoryId(0, showHidden: true);
            var mappedCategories = categories.ToDictionary(x => x.Id);

            var cquery = categories.AsQueryable();

            var query =
                from c in cquery
                select new
                {
                    id = c.Id.ToString(),
                    pId = c.ParentCategoryId,
                    name = c.Name,
                    open = false,
                    selected = channelCategories.Any(x => x == c.Id)
                };
            return new JsonResult { Data = query.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion


        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var model = new ChannelModel();
            PrepareChannelModel(model, null, true);
            //templates
            PrepareTemplatesModel(model);
            PrepareMemberGradeMappingModel(model, null, true);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(ChannelModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {

                var channel = model.ToEntity();
                channel.AddEntitySysParam();
                UpdateProductCategorys(model, channel);
                _channelService.InsertChannel(channel);
                UpdateMemberGradeMappings(model, channel);
                UpdateArticleSpecifications(model, channel);
                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = channel.Id }) : RedirectToAction("List");
            }
            PrepareChannelModel(model, null, true);
            PrepareTemplatesModel(model);
            PrepareMemberGradeMappingModel(model, null, false);
            PrerpareArticleSpecifications(model, null);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channel = _channelService.GetChannelById(id);
            if (channel == null)
                //No channel found with the specified id
                return RedirectToAction("List");

            var model = channel.ToModel();
            PrepareChannelModel(model, channel, true);
            //templates
            PrepareTemplatesModel(model);
            PrerpareArticleSpecifications(model, channel);
            PrepareMemberGradeMappingModel(model, channel, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(ChannelModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channel = _channelService.GetChannelById(model.Id);
            if (channel == null)
                //No channel found with the specified id
                return RedirectToAction("List");
            if (ModelState.IsValid)
            {
                channel = model.ToEntity(channel);
                channel.AddEntitySysParam(false, true);
                if (model.IsChangeCategory)
                    UpdateProductCategorys(model, channel);
                _channelService.UpdateChannel(channel);
                UpdateMemberGradeMappings(model, channel);
                UpdateArticleSpecifications(model, channel);

                NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));
                return continueEditing ? RedirectToAction("Edit", channel.Id) : RedirectToAction("List");
            }
            PrepareChannelModel(model, channel, true);
            PrepareMemberGradeMappingModel(model, channel, false);
            PrerpareArticleSpecifications(model, channel);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channel = _channelService.GetChannelById(id);
            if (channel == null)
                //No channel found with the specified id
                return RedirectToAction("List");

            _channelService.DeleteChannel(channel);

            NotifySuccess(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));
            return RedirectToAction("List");
        }

        public ActionResult DeleteSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageChannel))
                return AccessDeniedView();
            var channels = new List<Channel>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                channels.AddRange(_channelService.GetChannelsByIds(ids));

                for (int i = 0; i < channels.Count; i++)
                {
                    var channel = channels[i];
                    _channelService.DeleteChannel(channel);
                }
            }

            return RedirectToAction("List");
        }
        #endregion

    }
}