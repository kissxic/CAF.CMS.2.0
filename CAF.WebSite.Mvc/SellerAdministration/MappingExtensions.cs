using AutoMapper;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Users;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using CAF.WebSite.Mvc.Seller.Models.Common;
using CAF.WebSite.Mvc.Seller.Models.Users;

namespace CAF.WebSite.Mvc.Seller
{
    public static class MappingExtensions
    {
 
        #region Article
        public static Article ToEntity(this ArticleModel model)
        {
            return Mapper.Map<ArticleModel, Article>(model);
        }
        public static Article ToEntity(this ArticleModel model, Article destination)
        {
            return Mapper.Map(model, destination);
        }
        public static ArticleModel ToModel(this Article entity)
        {
            return Mapper.Map<Article, ArticleModel>(entity);
        }
        #endregion

        #region Product attributes

        public static ProductAttributeModel ToModel(this ProductAttribute entity)
        {
            return Mapper.Map<ProductAttribute, ProductAttributeModel>(entity);
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model)
        {
            return Mapper.Map<ProductAttributeModel, ProductAttribute>(model);
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model, ProductAttribute destination)
        {
            return Mapper.Map(model, destination);
        }

        //product options
        public static ProductAttributeOptionModel ToModel(this ProductAttributeOption entity)
        {
            return Mapper.Map<ProductAttributeOption, ProductAttributeOptionModel>(entity);
        }

        public static ProductAttributeOption ToEntity(this ProductAttributeOptionModel model)
        {
            return Mapper.Map<ProductAttributeOptionModel, ProductAttributeOption>(model);
        }

        public static ProductAttributeOption ToEntity(this ProductAttributeOptionModel model, ProductAttributeOption destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Users/users/users roles
        //customer roles
        public static UserRoleModel ToModel(this UserRole entity)
        {
            return Mapper.Map<UserRole, UserRoleModel>(entity);
        }

        public static UserRole ToEntity(this UserRoleModel model)
        {
            return Mapper.Map<UserRoleModel, UserRole>(model);
        }

        public static UserRole ToEntity(this UserRoleModel model, UserRole destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion

        #region Address

        public static AddressModel ToModel(this Address entity)
        {
            return Mapper.Map<Address, AddressModel>(entity);
        }

        public static Address ToEntity(this AddressModel model)
        {
            return Mapper.Map<AddressModel, Address>(model);
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion
    }
}