using CAF.WebSite.Application.Services.Directory;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Common;
using CAF.Infrastructure.Core.Domain.Directory;
using CAF.WebSite.Mvc.Models.ArticleCatalog;
using CAF.WebSite.Mvc.Models.Common;
using CAF.WebSite.Application.Services.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAF.Infrastructure.Core.Domain.Cms.Clients;
using CAF.WebSite.Mvc.Models.Catalog;
using CAF.WebSite.Mvc.Models.ShopProfile;
using CAF.Infrastructure.Core.Domain.Sellers;
using CAF.WebSite.Mvc.Models.Articles;
//using CAF.Infrastructure.Core.Domain.Shop.Catalog;


namespace CAF.WebSite.Mvc
{
    public static class MappingExtensions
    {

        //manufacturer
        public static ClientModel ToModel(this Client entity)
        {
            if (entity == null)
                return null;

            var model = new ClientModel()
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description),
                MetaKeywords = entity.GetLocalized(x => x.MetaKeywords),
                MetaDescription = entity.GetLocalized(x => x.MetaDescription),
                MetaTitle = entity.GetLocalized(x => x.MetaTitle),
                SeName = entity.GetSeName(),
            };
            return model;
        }
        //category
        public static ArticleCategoryModel ToModel(this ArticleCategory entity)
        {
            if (entity == null)
                return null;

            var model = new ArticleCategoryModel
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                FullName = entity.GetLocalized(x => x.FullName),
                Description = entity.GetLocalized(x => x.Description),
                BottomDescription = entity.GetLocalized(x => x.BottomDescription),
                MetaKeywords = entity.GetLocalized(x => x.MetaKeywords),
                MetaDescription = entity.GetLocalized(x => x.MetaDescription),
                MetaTitle = entity.GetLocalized(x => x.MetaTitle),
                SeName = entity.GetSeName(),
            };
            return model;
        }


       

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">A value indicating whether to exclude properties</param>
        /// <param name="addressSettings">Address settings</param>
        /// <param name="localizationService">Localization service (used to prepare a select list)</param>
        /// <param name="stateProvinceService">State service (used to prepare a select list). null to don't prepare the list.</param>
        /// <param name="loadCountries">A function to load countries  (used to prepare a select list). null to don't prepare the list.</param>
        public static void PrepareModel(this AddressModel model,
            Address address, bool excludeProperties,
            AddressSettings addressSettings,
            ILocalizationService localizationService = null,
            IStateProvinceService stateProvinceService = null,
            ICityService cityService = null,
            IDistrictService districtService = null,
            Func<IList<Country>> loadCountries = null)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (addressSettings == null)
                throw new ArgumentNullException("addressSettings");

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.EmailMatch = address.Email;
                model.Company = address.Company;
                model.CountryId = address.CountryId;
                model.CountryName = address.Country != null
                    ? address.Country.GetLocalized(x => x.Name)
                    : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = address.StateProvince != null
                    ? address.StateProvince.GetLocalized(x => x.Name)
                    : null;
                model.CityId = address.CityId;
                model.CityName = address.City != null
                    ? address.City.GetLocalized(x => x.Name)
                    : null;
                model.DistrictId = address.DistrictId;
                model.DistrictName = address.District != null
                    ? address.District.GetLocalized(x => x.Name)
                    : null;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                if (localizationService == null)
                    throw new ArgumentNullException("localizationService");

                model.AvailableCountries.Add(new SelectListItem() { Text = localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in loadCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    //states
                    if (stateProvinceService == null)
                        throw new ArgumentNullException("stateProvinceService");

                    var states = stateProvinceService
                        .GetStateProvincesByCountryId(model.CountryId.HasValue ? model.CountryId.Value : 0)
                        .ToList();
                    if (states.Count > 0)
                    {
                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem()
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        model.AvailableStates.Add(new SelectListItem()
                        {
                            Text = localizationService.GetResource("Address.OtherNonUS"),
                            Value = "0"
                        });
                    }
                    if (addressSettings.CityEnabled)
                    {
                        //citys
                        if (cityService == null)
                            throw new ArgumentNullException("stateProvinceService");

                        var citys = cityService
                            .GetCitysByProvinceId(model.StateProvinceId.HasValue ? model.StateProvinceId.Value : 0)
                            .ToList();
                        if (citys.Count > 0)
                        {
                            foreach (var s in citys)
                            {
                                model.AvailableCitys.Add(new SelectListItem()
                                {
                                    Text = s.GetLocalized(x => x.Name),
                                    Value = s.Id.ToString(),
                                    Selected = (s.Id == model.CityId)
                                });
                            }
                        }
                        else
                        {
                            model.AvailableCitys.Add(new SelectListItem()
                            {
                                Text = localizationService.GetResource("Address.OtherNonUS"),
                                Value = "0"
                            });
                        }
                        if (addressSettings.DistrictEnabled)
                        {
                            //districts
                            if (districtService == null)
                                throw new ArgumentNullException("stateProvinceService");

                            var districts = districtService
                                .GetDistrictsByCityId(model.CityId.HasValue ? model.CityId.Value : 0)
                                .ToList();
                            if (districts.Count > 0)
                            {
                                foreach (var s in districts)
                                {
                                    model.AvailableDistricts.Add(new SelectListItem()
                                    {
                                        Text = s.GetLocalized(x => x.Name),
                                        Value = s.Id.ToString(),
                                        Selected = (s.Id == model.DistrictId)
                                    });
                                }
                            }
                            else
                            {
                                model.AvailableDistricts.Add(new SelectListItem()
                                {
                                    Text = localizationService.GetResource("Address.OtherNonUS"),
                                    Value = "0"
                                });
                            }
                        }
                    }
                }
            }

            //form fields
            model.ValidateEmailAddress = addressSettings.ValidateEmailAddress;
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.CityEnabled = addressSettings.CityEnabled;
            model.DistrictEnabled = addressSettings.DistrictEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;
        }
        public static Address ToEntity(this AddressModel model)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity);
        }
        public static Address ToEntity(this AddressModel model, Address destination)
        {
            if (model == null)
                return destination;

            destination.Id = model.Id;
            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.CityId = model.CityId;
            destination.DistrictId = model.DistrictId;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;

            return destination;
        }

        public static VendorModel ToModel(this Vendor entity)
        {
            if (entity == null)
                return null;

            var model = new VendorModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Description = entity.Description,
                PictureId = entity.PictureId,
                AdminComment = entity.AdminComment,
                CompanyName = entity.CompanyName,
                CompanyRegionId = entity.CompanyRegionId,
                CompanyAddress = entity.CompanyAddress,
                CompanyPhone = entity.CompanyPhone,
                CompanyEmployeeCountId = entity.CompanyEmployeeCountId,
                CompanyRegisteredCapital = entity.CompanyRegisteredCapital,
                LegalPerson = entity.LegalPerson,
                ContactsName = entity.ContactsName,
                StageId = entity.StageId,
                ContactsPhone = entity.ContactsPhone,
                ContactsEmail = entity.ContactsEmail,
                BankAccountName = entity.BankAccountName,
                BankAccountNumber = entity.BankAccountNumber,
                BankName = entity.BankName,
                BankCode = entity.BankCode,
                BankRegionId = entity.BankRegionId,
                BankPictureId = entity.BankPictureId,
                VendorGradeId = entity.VendorGradeId,
                SeName = entity.GetSeName(),
                CreateDate = entity.CreateDate,

            };
            return model;
        }
        public static ProductCategoryModel ToModel(this ProductCategory entity)
        {
            if (entity == null)
                return null;

            var model = new ProductCategoryModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Alias = entity.Alias,
                Depth = entity.Depth,
                Path = entity.Path,
                PictureId = entity.PictureId,
                Description = entity.Description,
                Published = entity.Published,
                DisplayOrder = entity.DisplayOrder,
                CreatedOnUtc = entity.CreatedOnUtc,
                ModifiedOnUtc = entity.ModifiedOnUtc,

            };
            return model;
        }

    }
}