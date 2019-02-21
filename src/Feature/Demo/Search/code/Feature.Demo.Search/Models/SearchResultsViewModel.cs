using Feature.Demo.Search.Services.Search.Models;
using PagedList;
using SolrNet;
using System.Diagnostics;

namespace Feature.Demo.Search.Models
{
    /// <summary>
    /// View Model for the SearchResults view.
    /// </summary>
    public class SearchResultsViewModel
    {
        public string Query { get; set; }

        public int PageNumber { get; set; }

        public string Facet { get; set; }

        public string FacetName { get; set; }

        public SolrQueryResults<PageContentResultItem> SearchResults { get; set; }

        public StaticPagedList<PageContentResultItem> PagedResults { get; set; }

        public FacetCount[] FacetCounts { get; set; }

        [DebuggerDisplay("Key= {Key}, Name= {Name}, Count= {Count}")]
        public class FacetCount
        {
            public string Key { get; set; }

            public string Name { get; set; }

            public int Count { get; set; }
        }
    }
}