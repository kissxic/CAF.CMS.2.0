using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Seller.Models.Articles
{
    public class BulkEditArticleModel : EntityModelBase
    {
        public int DisplayOrder { get; set; }

    }
}