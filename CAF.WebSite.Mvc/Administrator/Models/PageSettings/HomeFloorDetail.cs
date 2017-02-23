using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace CAF.WebSite.Mvc.Admin.Models.PageSettings
{
    public class HomeFloorDetail : EntityModelBase
    {

        /// <summary>
        /// 默认TAB名称
        /// </summary>
        public string DefaultTabName { get; set; }
        /// <summary>
        /// 楼层名称
        /// </summary>
        public string Name { get; set; }

        public List<HomeFloorDetail.TextLink> ArticleLinks { get; set; }

        public List<HomeFloorDetail.ArticleModule> ArticleModules { get; set; }

        public int StyleLevel { get; set; }

        public string SubName { get; set; }
        /// <summary>
        /// 更多的URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 关联品牌IDs
        /// </summary>
        public string ManufacturerIds { get; set; }
        /// <summary>
        /// 关联栏目分类IDs
        /// </summary>
        public string CategoryIds { get; set; }
        /// <summary>
        /// 关联栏目分类IDs
        /// </summary>
        public string ProductIds { get; set; }
        /// <summary>
        /// 关联栏目分类IDs
        /// </summary>
        public string VendorIds { get; set; }

        public List<HomeFloorDetail.Slide> Slides { get; set; }

        public List<HomeFloorDetail.TextLink> TextLinks { get; set; }

        public HomeFloorDetail()
        {
            TextLinks = new List<HomeFloorDetail.TextLink>();
            ArticleLinks = new List<HomeFloorDetail.TextLink>();
        }

        public class ArticleDetail : EntityModelBase
        {
            public int ArticleId { get; set; }

            public string Url { get; set; }

            public int PictureId { get; set; }

            public ArticleDetail()
            {
            }
        }

        public class ArticleModule : EntityModelBase
        {
            public int ModuleIndex { get; set; }

            public string Name { get; set; }

            public List<int> ArticleIds { get; set; }

            public List<HomeFloorDetail.ArticleModule.Topic> Topics { get; set; }

            public ArticleModule()
            {
                ArticleIds = new List<int>();
                Topics = new List<HomeFloorDetail.ArticleModule.Topic>();
            }

            public class Topic : EntityModelBase
            {
                public string ImageUrl { get; set; }

                public string Url { get; set; }

                public Topic()
                {
                }
            }
        }

        public class Slide : EntityModelBase
        {
            public int Count { get; set; }

            public List<HomeFloorDetail.ArticleDetail> Detail { get; set; }

            public string Ids { get; set; }

            public string Name { get; set; }

            public int PictureId { get; set; }

            public string PictureUrl { get; set; }


            public Slide()
            {
            }
        }

        public class TextLink : EntityModelBase
        {
            public string Name { get; set; }

            public string Url { get; set; }

            public int PictureId { get; set; }

            public TextLink()
            {
            }
        }
    }
}