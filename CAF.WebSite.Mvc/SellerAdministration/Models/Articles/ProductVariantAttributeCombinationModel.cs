using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    public class ProductVariantAttributeCombinationSkuModel : EntityModelBase
    {
        public ProductVariantAttributeCombinationSkuModel()
        {
            ProductVariantAttributes = new List<ProductVariantAttributeModel>();
            AssignablePictures = new List<PictureSelectItemModel>();

            ProductAttributes = new List<ProductAttributeModel>();

        }

        public IList<PictureSelectItemModel> AssignablePictures { get; set; }


        public IList<ProductVariantAttributeModel> ProductVariantAttributes { get; set; }


        public IList<ProductAttributeModel> ProductAttributes { get; set; }

        #region Nested classes

        public class PictureSelectItemModel : EntityModelBase
        {
            public string PictureUrl { get; set; }

            public string PicturId { get; set; }

            public string ProductAttributeOptionName { get; set; }

            public int ProductAttributeOptionId { get; set; }

            public bool IsAssigned { get; set; }
        }

        public class ProductVariantAttributeModel : EntityModelBase
        {
            public ProductVariantAttributeModel()
            {
                Values = new List<ProductVariantAttributeValueModel>();
            }

            public int ProductAttributeId { get; set; }

            public string Name { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ProductVariantAttributeValueModel> Values { get; set; }
        }

        public class ProductVariantAttributeValueModel : EntityModelBase
        {
            public string Name { get; set; }
            public string Color { get; set; }
            public int ProductAttributeOptionId { get; set; }

            public bool IsPreSelected { get; set; }
        }
        #endregion
    }
}