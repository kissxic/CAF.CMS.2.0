using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using CAF.Infrastructure.Core;
using CAF.WebSite.Mvc.Models.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Cms.Articles;

namespace CAF.WebSite.Mvc
{
    public static class ArticleDetailsExtensions
    {

        public static string UpdateArticleDetailsUrl(this ArticlePostModel model, string itemType = null)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            string url = urlHelper.Action("UpdateArticleDetails", "Article", new
            {
                productId = model.Id,
                itemType = itemType
            });

            return url;
        }



        public static Picture GetAssignedPicture(this ArticlePostModel model, IPictureService pictureService, IList<Picture> pictures, int productId = 0)
        {
            if (model != null && model.SelectedCombination != null)
            {
                Picture picture = null;
                var combiAssignedImages = model.SelectedCombination.GetAssignedPictureIds();

                if (combiAssignedImages.Length > 0)
                {
                    if (pictures == null)
                        picture = pictureService.GetPictureById(combiAssignedImages[0]);
                    else
                        picture = pictures.FirstOrDefault(p => p.Id == combiAssignedImages[0]);
                }

                if (picture == null && productId != 0)
                {
                    picture = pictureService.GetPicturesByArticleId(productId, 1).FirstOrDefault();
                }
                return picture;
            }
            return null;
        }

        public static string GetAttributeValueInfo(this ArticlePostModel.ProductVariantAttributeValueModel model)
        {
            string result = "";

            if (model.PriceAdjustment.HasValue())
                result = " [{0}]".FormatWith(model.PriceAdjustment);

            if (model.QuantityInfo > 1)
                return " × {1}".FormatWith(result, model.QuantityInfo) + result;

            return result;
        }

        public static bool ShouldBeRendered(this ArticlePostModel.ProductVariantAttributeModel variantAttribute)
        {
            switch (variantAttribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                case AttributeControlType.ColorSquares:
                    return (variantAttribute.Values.Count > 0);
                default:
                    return true;
            }
        }

        public static bool ShouldBeRendered(this IEnumerable<ArticlePostModel.ProductVariantAttributeModel> variantAttributes)
        {
            if (variantAttributes != null)
            {
                foreach (var item in variantAttributes)
                {
                    if (item.ShouldBeRendered())
                        return true;
                }
            }
            return false;
        }
    }
}