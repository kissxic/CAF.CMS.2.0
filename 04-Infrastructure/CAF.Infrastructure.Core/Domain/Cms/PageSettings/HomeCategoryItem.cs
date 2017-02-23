using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using System;
namespace CAF.Infrastructure.Core.Domain.Cms.PageSettings
{
    public class HomeCategoryItem : BaseEntity
    {
        public int HomeCategoryId { get; set; }

        public HomeCategory HomeCategory { get; set; }

        public int CategoryId { get; set; }

        public int ParentCategoryId { get; set; }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int Depth { get; set; }

        public int RowNumber { get; set; }

    }
}
