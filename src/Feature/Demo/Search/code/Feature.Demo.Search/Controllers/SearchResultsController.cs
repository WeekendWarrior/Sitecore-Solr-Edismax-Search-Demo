using Feature.Demo.Search.Models;
using Feature.Demo.Search.Services.Search.Models;
using Feature.Demo.Search.Services.Search.Services;
using Foundation.Demo.Search.Services;
using PagedList;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Feature.Demo.Search.Models.SearchResultsViewModel;

namespace Feature.Demo.Search.Controllers
{
    /// <summary>
    /// Executes and Renders Search results.
    /// </summary>
    public class SearchResultsController : SitecoreController
    {
        private IDemoSearchService _searchService;

        /// <summary>
        /// Creates a new instance of the <see cref="SearchResultsController"/> class.
        /// </summary>
        public SearchResultsController()
        {
            // TODO: inject with DI
            _searchService = new SolrDemoSearchService();
        }

        /// <summary>
        /// Returns the search results rendering.
        /// </summary>
        public ActionResult SearchResults(string q, int pageNumber = 1, string facet = "")
        {
            // Get the rendering datasource item.
            Item datasourceItem = GetDatasourceItem();

            // Convert Sitecore item into a EdismaxSolrQuery.
            EdismaxSolrQuery query = _searchService.BuildEdismaxSolrQueryFromItem(datasourceItem);

            // Build the model.
            var model = new SearchResultsViewModel
            {
                Query = q,
                SearchResults = new SolrQueryResults<PageContentResultItem>(),
                PageNumber = pageNumber,
                Facet = facet
            };

            // Execute the Search.
            if (datasourceItem != null && !string.IsNullOrWhiteSpace(q))
            {
                // Convert 1-based pageNumber to 0-based pageIndex.
                int pageIndex = pageNumber > 0 ? pageNumber - 1 : 0;
                int startRow = pageIndex * query.Rows;

                // Execute the search.
                model.SearchResults = _searchService.Search(q, query, startRow, facetValue: facet);
            }

            // Page the results.
            model.PagedResults = new StaticPagedList<PageContentResultItem>(
                model.SearchResults,
                pageNumber,
                query.Rows,
                model.SearchResults.NumFound
            );

            // Facet Counts.
            model.FacetCounts = BuildFacetCounts(model).ToArray();
            
            // Determine the selected Facet Name.
            Guid selectedFacetId = Guid.Empty;
            if (Guid.TryParse(facet, out selectedFacetId))
            {
                model.FacetName = DetermineFacetName(model, selectedFacetId);
            }

            // Send the model to the view.
            return View(Views.SearchResults, model);
        }

        /// <summary>
        /// List of views used by the <see cref="SearchResultsController"/> class.
        /// </summary>
        public static class Views
        {
            public const string SearchResults = "~/Views/Demo/Search/SearchResults.cshtml";
        }

        #region infrastructure

        /// <summary>
        /// Builds facet counts.
        /// </summary>
        private static IEnumerable<FacetCount> BuildFacetCounts(
            SearchResultsViewModel model, 
            string facetNotSelectedValue = "*Facet NOT Selected*"
            )
        {
            if (model.SearchResults != null
                && model.SearchResults.FacetFields != null
                && model.SearchResults.FacetFields.Keys != null)
            {
                foreach (string facetKey in model.SearchResults.FacetFields.Keys)
                {
                    ICollection<KeyValuePair<string, int>> facets = model.SearchResults.FacetFields[facetKey];

                    foreach (KeyValuePair<string, int> facetCount in facets)
                    {
                        // Try to replace the itemID with the Item Name.
                        Item item = null;
                        Guid itemId = Guid.Empty;
                        if (Guid.TryParse(facetCount.Key, out itemId) && itemId != Guid.Empty)
                        {
                            item = Context.Database.GetItem(new ID(itemId));
                        }

                        yield return new FacetCount
                        {
                            Key = facetCount.Key,
                            Name = item != null ? item.Name : facetNotSelectedValue,
                            Count = facetCount.Value
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Builds facet counts.
        /// </summary>
        private static string DetermineFacetName(SearchResultsViewModel model, Guid selectedFacetKey)
        {
            if (model.SearchResults != null
                && model.SearchResults.FacetFields != null
                && model.SearchResults.FacetFields.Keys != null)
            {
                foreach (string facetKey in model.SearchResults.FacetFields.Keys)
                {
                    ICollection<KeyValuePair<string, int>> facets = model.SearchResults.FacetFields[facetKey];

                    foreach (KeyValuePair<string, int> facetCount in facets)
                    {
                        // Try to replace the itemID with the Item Name.
                        Item item = null;
                        Guid itemId = Guid.Empty;
                        if (Guid.TryParse(facetCount.Key, out itemId) 
                            && itemId != Guid.Empty
                            && itemId == selectedFacetKey)
                        {
                            item = Context.Database.GetItem(new ID(itemId));

                            if (item != null)
                            {
                                return item.Name;
                            }
                        }                        
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the datasource item.
        /// </summary>
        public Item GetDatasourceItem()
        {
            var datasourceId = RenderingContext.Current.Rendering.DataSource;

            return ID.IsID(datasourceId) ? Context.Database.GetItem(datasourceId) : null;
        }

        #endregion
    }
}