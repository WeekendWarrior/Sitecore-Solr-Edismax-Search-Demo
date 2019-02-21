using Feature.Demo.Search.Services.Search.Models;
using Foundation.Demo.Search.Services;
using Sitecore.Data.Items;
using SolrNet;

namespace Feature.Demo.Search.Services.Search.Services
{
    /// <summary>
    /// Searches the search index for content.
    /// </summary>
    public interface IDemoSearchService
    {
        /// <summary>
        /// Search for content using an instance of <see cref="EdismaxSolrQuery"/>.
        /// </summary>
        SolrQueryResults<PageContentResultItem> Search(
            string q,
            Item solrQueryItem,
            int startRow = 0,
            string language = "",
            string facetValue = "");

        /// <summary>
        /// Search for content using a Sitecore <see cref="Item"/>.
        /// </summary>
        SolrQueryResults<PageContentResultItem> Search(
            string q,
            EdismaxSolrQuery query,
            int startRow = 0,
            string language = "",
            string facetValue = "");

        /// <summary>
        /// Builds a <see cref="EdismaxSolrQuery"/> from a Sitecore <see cref="Item"/>.
        /// </summary>
        EdismaxSolrQuery BuildEdismaxSolrQueryFromItem(Item solrQueryItem);
    }
}