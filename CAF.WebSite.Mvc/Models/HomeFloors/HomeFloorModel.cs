using CAF.Infrastructure.Core.Domain.Cms.PageSettings;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Models.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.WebSite.Mvc.Models.HomeFloors
{
    public class HomeFloorModel : ModelBase
    {
        public HomeFloorModel()
        {
            Brands = new List<WebFloorBrand>();
            TextLinks = new List<WebFloorTextLink>();
            ArticleLinks = new List<WebFloorArticleLinks>();
            Articles = new List<ArticleOverviewModel>();
            Vendors = new List<HomeFloors.HomeFloorModel.WebFloorTextLink>();
            Tabs = new List<Tab>();
        }
       
        public string DefaultTabName { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        public List<WebFloorBrand> Brands { get; set; }

        public List<WebFloorArticleLinks> ArticleLinks { get; set; }

        public List<WebFloorArticleLinks> RightBottons { get; set; }

        public List<WebFloorArticleLinks> RightTops { get; set; }

        public List<WebFloorArticleLinks> Scrolls { get; set; }

        public List<ArticleOverviewModel> Articles { get; set; }

        public List<WebFloorTextLink> Vendors { get; set; }

        public long StyleLevel { get; set; }

        public string SubName { get; set; }

        public List<Tab> Tabs { get; set; }

        public List<WebFloorTextLink> TextLinks { get; set; }

        public class ArticleDetail
        {
            public string ImagePath { get; set; }

            public string Name { get; set; }

            public decimal Price { get; set; }

            public long ArticleId { get; set; }

            public ArticleDetail()
            {
            }
        }

        public class Tab
        {
            public List<ArticleDetail> Detail { get; set; }

            public long Id { get; set; }

            public string Name { get; set; }

            public Tab() { }
        }

        public class WebFloorBrand
        {
            public long Id { get; set; }

            public string Img { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }

            public WebFloorBrand()
            {
            }
        }

        public class WebFloorArticleLinks
        {
            public long Id { get; set; }

            public string ImageUrl { get; set; }

            public int PictureId { get; set; }

            public Position Type { get; set; }

            public string Url { get; set; }

            public WebFloorArticleLinks()
            {
            }
        }

        public class WebFloorTextLink
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }

            public WebFloorTextLink()
            {
            }
        }
    }
}