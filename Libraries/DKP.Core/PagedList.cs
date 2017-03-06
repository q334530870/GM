using System;
using System.Collections.Generic;
using System.Linq;
using DKP.Core.Infrastructure;
using DKP.Core.Models;

namespace DKP.Core
{
    /// <summary>
    /// Paged list
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    [Serializable]
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageParam">pageParam</param>
        public PagedList(IQueryable<T> source, PageParam pageParam)
        {
            this.total = source.Count();
            this.limit = pageParam.limit;
            this.sort = pageParam.sort;
            this.order = pageParam.order;
            this.AddRange(source.Skip(pageParam.offset).Take(pageParam.limit).ToList());
        }

        public int total { get; set; }
        public int limit { get; set; }
        public string sort { get; set; }
        public string order { get; set; }
        public List<T> rows { get; set; }
    }
}
