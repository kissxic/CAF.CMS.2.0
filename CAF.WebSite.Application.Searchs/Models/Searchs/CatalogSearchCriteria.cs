﻿using CAF.Infrastructure.Core.Domain.Directory;
using CAF.Infrastructure.Core.Domain.Localization;
using CAF.WebSite.Application.Searchs.Models.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CAF.WebSite.Application.Searchs.Models.Searchs
{
    public class CatalogSearchCriteria : PagedSearchCriteria
    {
        //For JSON deserialization 
        public CatalogSearchCriteria()
            : base(new NameValueCollection())
        {
        }

        public CatalogSearchCriteria(Language language, Currency currency)
            : this(language, currency, new NameValueCollection())
        {
        }
        public CatalogSearchCriteria(Language language, Currency currency, NameValueCollection queryString)
            : base(queryString)
        {
            Language = language;
            Currency = currency;      
            SearchInChildren = true;

            Parse(queryString);
        }

        public CatalogSearchResponseGroup ResponseGroup { get; set; }
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }
        public Currency Currency { get; set; }
        public Language Language { get; set; }
        public string Keyword { get; set; }
        public string VendorId { get; set; }
        public string[] VendorIds { get; set; }

        public Term[] Terms { get; set; }
        public string SortBy { get; set; }
        public bool SearchInChildren { get; set; }

        public CatalogSearchCriteria Clone()
        {
            var retVal = new CatalogSearchCriteria(Language, Currency);
            retVal.CatalogId = CatalogId;
            retVal.CategoryId = CategoryId;
            retVal.ResponseGroup = ResponseGroup;
            retVal.Currency = Currency;
            retVal.Language = Language;
            retVal.Keyword = Keyword;
            retVal.VendorId = VendorId;
            retVal.VendorIds = VendorIds;
            retVal.SortBy = SortBy;
            retVal.SearchInChildren = SearchInChildren;
            retVal.PageNumber = PageNumber;
            retVal.PageSize = PageSize;
            if (Terms != null)
            {
                retVal.Terms = Terms.Select(x => new Term { Name = x.Name, Value = x.Value }).ToArray();
            }
            return retVal;
        }

        private void Parse(NameValueCollection queryString)
        {
            Keyword = queryString.Get("q");
            //TODO move this code to Parse or Converter method
            // tags=name1:value1,value2,value3;name2:value1,value2,value3
            SearchInChildren = Convert.ToBoolean(queryString.Get("deep_search") ?? SearchInChildren.ToString());
            ResponseGroup = EnumUtility.SafeParse(queryString.Get("resp_group"), CatalogSearchResponseGroup.WithProducts | CatalogSearchResponseGroup.WithCategories | CatalogSearchResponseGroup.WithProperties | CatalogSearchResponseGroup.WithOutlines);
            SortBy = queryString.Get("sort_by");
            Terms = (queryString.GetValues("terms") ?? new string[0])
                .SelectMany(s => s.Split(';'))
                .Select(s => s.Split(':'))
                .Where(a => a.Length == 2)
                .SelectMany(a => a[1].Split(',').Select(v => new Term { Name = a[0], Value = v }))
                .ToArray();
            VendorId = queryString.Get("vendor");
            VendorIds = (queryString.GetValues("vendors") ?? new string[0]);
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

                hash = hash * 59 + this.ResponseGroup.GetHashCode();
                hash = hash * 59 + this.SearchInChildren.GetHashCode();

                if (this.CatalogId != null)
                    hash = hash * 59 + this.CatalogId.GetHashCode();

                if (this.CategoryId != null)
                    hash = hash * 59 + this.CategoryId.GetHashCode();

                if (this.Currency != null)
                    hash = hash * 59 + this.Currency.CurrencyCode.GetHashCode();

                if (this.Language != null)
                    hash = hash * 59 + this.Language.LanguageCulture.GetHashCode();

                if (this.Keyword != null)
                    hash = hash * 59 + this.Keyword.GetHashCode();

                if (this.VendorId != null)
                    hash = hash * 59 + this.VendorId.GetHashCode();

                if (!this.VendorIds.IsNullOrEmpty())
                    hash = hash * 59 + string.Join(",", this.VendorIds).GetHashCode();

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
            retVal.Add(string.Format("respGroup={0}", ResponseGroup.ToString()));
            if (Keyword != null)
            {
                retVal.Add(string.Format("q={0}", Keyword));
            }
            if (CategoryId != null)
            {
                retVal.Add(string.Format("categoryId={0}", CategoryId));
            }
            if (VendorId != null)
            {
                retVal.Add(string.Format("vendor={0}", VendorId));
            }
            if (VendorIds != null)
            {
                retVal.Add(string.Format("vendors={0}", string.Join(",", this.VendorIds)));
            }
            return string.Join("&", retVal);
        }
    }
}
