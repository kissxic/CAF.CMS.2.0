using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    public class CategoryJsonModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<SecondLevelCategory> SubCategory { get; set; }

        public CategoryJsonModel()
        {
        }
    }
}