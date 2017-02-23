﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace CAF.WebSite.Application.Searchs.Models.Common
{
    /// <summary>
    /// PagedList which page number and page size can be changed  (on render view time for example)
    /// </summary>
    public interface IMutablePagedList : IPagedList 
    {
        /// <summary>
        /// Contains information for sorting order
        /// </summary>
        IEnumerable<SortInfo> SortInfos { get; }
        /// <summary>
        /// Slice  the current set of data by new page sizes
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        void Slice(int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos);
    }

    public interface IMutablePagedList<T> : IMutablePagedList, IPagedList<T>
    {
    }
}
