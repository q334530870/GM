
using System.Collections.Generic;

namespace DKP.Core
{
    /// <summary>
    /// Paged list interface
    /// </summary>
    public interface IPagedList<T> : IList<T>
    {
        int total { get; }
        int limit { get; }
        string sort { get; }
        string order { get; }
        List<T> rows { get; }
    }
}
