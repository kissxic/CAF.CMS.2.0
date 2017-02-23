using CAF.Infrastructure.Core.Domain.Localization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CAF.WebSite.Application.Searchs.Models.Searchs
{
    public class ProductCategorySearchCriteria : PagedSearchCriteria
    {
        //For JSON deserialization 
        public ProductCategorySearchCriteria()
            : base(new NameValueCollection())
        {
        }

        public ProductCategorySearchCriteria(Language language)
            : this(language, new NameValueCollection())
        {
        }
        public ProductCategorySearchCriteria(Language language, NameValueCollection queryString)
            : base(queryString)
        {
            Language = language;

            Parse(queryString);
        }

        public string Outline { get; set; }

        public Language Language { get; set; }

        public string Keyword { get; set; }

        public string SortBy { get; set; }

        public ProductCategorySearchCriteria Clone()
        {
            var retVal = new ProductCategorySearchCriteria(Language);
            retVal.Language = Language;
            retVal.Keyword = Keyword;
            retVal.SortBy = SortBy;
            retVal.PageNumber = PageNumber;
            retVal.PageSize = PageSize;
            return retVal;
        }

        private void Parse(NameValueCollection queryString)
        {
            Keyword = queryString.Get("q");
            SortBy = queryString.Get("sort_by");
        }


        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = base.GetHashCode();

                if (this.Language != null)
                    hash = hash * 59 + this.Language.LanguageCulture.GetHashCode();

                if (this.Keyword != null)
                    hash = hash * 59 + this.Keyword.GetHashCode();

                if (this.SortBy != null)
                    hash = hash * 59 + this.SortBy.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            var retVal = new List<string>();
            retVal.Add(string.Format("page={0}", PageNumber));
            retVal.Add(string.Format("page_size={0}", PageSize));
            if (Keyword != null)
            {
                retVal.Add(string.Format("q={0}", Keyword));
            }
            return string.Join("&", retVal);
        }
    }
}