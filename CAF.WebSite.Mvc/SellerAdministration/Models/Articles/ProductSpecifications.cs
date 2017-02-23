using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Mvc.Seller.Validators.Articles;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{

    public class ProductSpecifications
    {
        public ProductSpecifications()
        {
            Type = new List<string>();
            TypeId = new List<int>();
        }

        public int IsPic { get; set; }

        public List<string> Type { get; set; }
        public List<int> TypeId { get; set; }

        public SpecificationsValue Data { get; set; }
    }

    public class SpecificationsValue
    {
        public SpecificationsValue()
        {
            ch = new List<SpecificationsValue>();
        }


        public string name { get; set; }

        public string type { get; set; }

        public int typeid { get; set; }

        public string color { get; set; }

        public string pic { get; set; }

        public decimal sku_price { get; set; }

        public int sku_stock { get; set; }

        public int sku_weight { get; set; }

        public string sku_code { get; set; }

        public List<SpecificationsValue> ch { get; set; }

    }

}