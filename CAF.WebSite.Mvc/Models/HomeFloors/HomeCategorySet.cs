using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Models.HomeFloors
{
    public class HomeCategorySet : ModelBase
    {
        public List<HomeCategoryInfoModel> HomeCategories { get; set; }

        public List<HomeCategoryNew> Brands { get; set; }

        public List<HomeCategoryNew> Vendor { get; set; }

        public int RowNumber { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public HomeCategorySet()
        {
            HomeCategories = new List<HomeCategoryInfoModel>();
            Brands = new List<HomeCategoryNew>();
            Vendor = new List<HomeCategoryNew>();
        }

        public class HomeCategoryNew
        {
            public int Id { get; set; }

            public string Img { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }

            public string ImageUrl { get; set; }

            public HomeCategoryNew()
            {
            }
        }
    }
}