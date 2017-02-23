
using System.Reflection;
using AutoMapper;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Seller.Models.Articles;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.WebSite.Application.Services.Seo;
using CAF.WebSite.Mvc.Seller.Models.Users;
using CAF.Infrastructure.Core.Domain.Users;

namespace CAF.WebSite.Mvc.Seller.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        class OptionalFkConverter : ITypeConverter<int, int?>
        {
            public int? Convert(ResolutionContext context)
            {
                var srcName = context.PropertyMap.SourceMember.Name;

                if (context.PropertyMap.SourceMember.MemberType == MemberTypes.Property && srcName.EndsWith("Id") && !context.SourceType.IsNullable())
                {
                    var src = (int)context.SourceValue;
                    return src == 0 ? (int?)null : src;
                }

                return (int?)context.SourceValue;
            }
        }

        public void Execute()
        {
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)

            // special mapper, that avoids DbUpdate exceptions in cases where
            // optional (nullable) int FK properties are 0 instead of null 
            // after mapping model > entity.
            Mapper.CreateMap<int, int?>().ConvertUsing(new OptionalFkConverter());


            Mapper.CreateMap<UserRole, UserRoleModel>()
              .ForMember(dest => dest.TaxDisplayTypes, mo => mo.Ignore())
                /*.ForMember(dest => dest.TaxDisplayType, mo => mo.MapFrom((src) => src.TaxDisplayType))*/;
            Mapper.CreateMap<UserRoleModel, UserRole>()
                .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());

            Mapper.CreateMap<ArticleCategoryModel, ArticleCategory>()
                     .ForMember(dest => dest.Deleted, mo => mo.Ignore());
            Mapper.CreateMap<ArticleCategory, ArticleCategoryModel>()
                    .ForMember(dest => dest.AvailableModelTemplates, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableDefaultViewModes, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Breadcrumb, mo => mo.Ignore())
                .ForMember(dest => dest.ParentCategoryBreadcrumb, mo => mo.Ignore())
                .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedUserRoleIds, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore());

          
            Mapper.CreateMap<ArticleModel, Article>().ForMember(dest => dest.DisplayOrder, mo => mo.Ignore())
                .ForMember(dest => dest.ArticleTags, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.ModifiedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Deleted, mo => mo.Ignore())
                .ForMember(dest => dest.ArticleAlbum, mo => mo.Ignore());
            Mapper.CreateMap<Article, ArticleModel>()
                .ForMember(dest => dest.PictureThumbnailUrl, mo => mo.Ignore())
                .ForMember(dest => dest.NoThumb, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableModelTemplates, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedUserRoleIds, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableSites, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedSiteIds, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Url, mo => mo.Ignore()); ;
 

            //product attributes
            Mapper.CreateMap<ProductAttribute, ProductAttributeModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore());
            Mapper.CreateMap<ProductAttributeModel, ProductAttribute>();
            Mapper.CreateMap<ProductAttributeOption, ProductAttributeOptionModel>()
          .ForMember(dest => dest.Locales, mo => mo.Ignore())
          .ForMember(dest => dest.Multiple, mo => mo.Ignore());
            Mapper.CreateMap<ProductAttributeOptionModel, ProductAttributeOption>()
                .ForMember(dest => dest.ProductAttribute, mo => mo.Ignore());

            //specification attributes
            Mapper.CreateMap<SpecificationAttribute, SpecificationAttributeModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.OptionCount, mo => mo.Ignore());
            Mapper.CreateMap<SpecificationAttributeModel, SpecificationAttribute>()
                .ForMember(dest => dest.SpecificationAttributeOptions, mo => mo.Ignore());
            Mapper.CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Multiple, mo => mo.Ignore());
            Mapper.CreateMap<SpecificationAttributeOptionModel, SpecificationAttributeOption>()
                .ForMember(dest => dest.SpecificationAttribute, mo => mo.Ignore())
                .ForMember(dest => dest.ArticleSpecificationAttributes, mo => mo.Ignore());

            //address
            Mapper.CreateMap<Address, AddressModel>()
                .ForMember(dest => dest.AddressHtml, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStates, mo => mo.Ignore())
                .ForMember(dest => dest.FirstNameEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.FirstNameRequired, mo => mo.Ignore())
                .ForMember(dest => dest.LastNameEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.LastNameRequired, mo => mo.Ignore())
                .ForMember(dest => dest.EmailEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.EmailRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CompanyEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CompanyRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CountryEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.StateProvinceEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Enabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Required, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeRequired, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneRequired, mo => mo.Ignore())
                .ForMember(dest => dest.FaxEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.FaxRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CountryName, mo => mo.MapFrom(src => src.Country != null ? src.Country.Name : null))
                .ForMember(dest => dest.StateProvinceName, mo => mo.MapFrom(src => src.StateProvince != null ? src.StateProvince.Name : null));
            Mapper.CreateMap<AddressModel, Address>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Country, mo => mo.Ignore())
                .ForMember(dest => dest.StateProvince, mo => mo.Ignore());
        }

        public int Order
        {
            get { return 0; }
        }
    }
}