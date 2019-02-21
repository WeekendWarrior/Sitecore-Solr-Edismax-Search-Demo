using Feature.Demo.Search.Services.Search.Models;
using Foundation.Demo.Search.Services;
using Sitecore.Data.Items;
using SolrNet;
using System.Collections.Generic;

namespace Feature.Demo.Search.Services.Search.Services
{
    /// <summary>
    /// Searches Solr for sitecore content.
    /// </summary>
    public class SolrDemoSearchService : EdismaxSearchBase, IDemoSearchService
    {
        /// <summary>
        /// Search for content using an instance of <see cref="EdismaxSolrQuery"/>.
        /// </summary>
        public SolrQueryResults<PageContentResultItem> Search(
            string q,
            Item solrQueryItem,
            int startRow = 0,
            string language = "",
            string facetValue = "")
        {
            // TODO: custom business logic here.

            EdismaxSolrQuery query = BuildEdismaxSolrQueryFromItem(solrQueryItem);

            return Search(q, query, startRow, language, facetValue);
        }

        /// <summary>
        /// Search for content using a Sitecore <see cref="Item"/>.
        /// </summary>
        public SolrQueryResults<PageContentResultItem> Search(
            string q,
            EdismaxSolrQuery query,
            int startRow = 0,
            string language = "",
            string facetValue = "")
        {
            // TODO: custom business logic here

            // Filter by facet.
            var filters = new List<ISolrQuery>();

            if (!string.IsNullOrWhiteSpace(query.FacetField) && !string.IsNullOrWhiteSpace(facetValue))
            {
                filters.Add(new SolrQueryByField(query.FacetField, facetValue));
            }

            return EdismaxSearch<PageContentResultItem>(q, query, startRow, language, filters);
        }

        /// <summary>
        /// Builds a <see cref="EdismaxSolrQuery"/> from a Sitecore <see cref="Item"/>.
        /// </summary>
        public EdismaxSolrQuery BuildEdismaxSolrQueryFromItem(Item solrQueryItem)            
        {
            return base.BuildEdismaxSolrQuery(solrQueryItem);
        }
    }
}