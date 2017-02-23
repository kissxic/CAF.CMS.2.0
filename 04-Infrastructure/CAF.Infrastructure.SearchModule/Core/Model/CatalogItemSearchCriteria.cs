﻿using System;
using System.Collections.Specialized;
using System.Text;
using CAF.Infrastructure.SearchModule.Model.Search;
using CAF.Infrastructure.SearchModule.Model.Search.Criterias;

namespace CAF.Infrastructure.SearchModule.Core.Model
{
    // TODO: move to catalog module as it is catalog specific criteria and not generic search one

    public class CatalogItemSearchCriteria : KeywordSearchCriteria
    {
        public const string DocType = "catalogitem";

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogItemSearchCriteria"/> class.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        public CatalogItemSearchCriteria(string documentType)
            : base(documentType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogItemSearchCriteria"/> class.
        /// </summary>
        public CatalogItemSearchCriteria()
            : base(DocType)
        {
        }

        /// <summary>
        /// Gets the default sort order.
        /// </summary>
        /// <value>The default sort order.</value>
        public static SearchSort DefaultSortOrder { get { return new SearchSort("__sort", false); } }

        private string _catalog;
        /// <summary>
        /// Gets or sets the indexes of the search.
        /// </summary>
        /// <value>
        /// The index of the search.
        /// </value>
        public virtual string Catalog
        {
            get { return _catalog; }
            set { ChangeState(); _catalog = value; }
        }

        private string[] _responseGroups;
        /// <summary>
        /// Gets or sets the response groups.
        /// </summary>
        /// <value>
        /// The response groups.
        /// </value>
        public virtual string[] ResponseGroups
        {
            get { return _responseGroups; }
            set { ChangeState(); _responseGroups = value; }
        }

        private StringCollection _outlines = new StringCollection();
        /// <summary>
        /// Gets or sets the outlines. Outline consists of "Category1/Category2".
        /// </summary>
        /// <example>Everything/digital-cameras</example>
        /// <value>The outlines.</value>
        public virtual StringCollection Outlines
        {
            get { return _outlines; }
            set { ChangeState(); _outlines = value; }
        }


        private StringCollection _classType = new StringCollection();

        /// <summary>
        /// Gets or sets the class types.
        /// </summary>
        /// <value>The class types.</value>
        public virtual StringCollection ClassTypes
        {
            get { return _classType; }
            set { ChangeState(); _classType = value; }
        }

        private DateTime _startDate = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the start date. The date must be in UTC format as that is format indexes are stored in.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get { return _startDate; }
            set { ChangeState(); _startDate = value; }
        }

        private DateTime? _startDateFrom;

        /// <summary>
        /// Gets or sets the start date from filter. Used for filtering new products. The date must be in UTC format as that is format indexes are stored in.
        /// </summary>
        /// <value>The start date from.</value>
        public DateTime? StartDateFrom
        {
            get { return _startDateFrom; }
            set { ChangeState(); _startDateFrom = value; }
        }

        private DateTime? _endDate;

        /// <summary>
        /// Gets or sets the end date. The date must be in UTC format as that is format indexes are stored in.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime? EndDate
        {
            get { return _endDate; }
            set { ChangeState(); _endDate = value; }
        }
    }
}
