using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using Sitecore.Data.Items;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Foundation.Demo.Search.Services
{
    /// <summary>
    /// Provides base methods for working with Solr Edismax Search queries.
    /// </summary>
    public abstract class EdismaxSearchBase
    {
        /// <summary>
        /// Search for content using an instance of <see cref="EdismaxSolrQuery"/>.
        /// </summary>
        protected virtual SolrQueryResults<T> EdismaxSearch<T>(
            string q,
            Item solrQueryItem,
            int startRow = 0,
            string language = "",
            ICollection<ISolrQuery> additionalFilters = null)
        {
            // Validate the parameters.
            if (string.IsNullOrWhiteSpace(q) || solrQueryItem == null)
            {
                return new SolrQueryResults<T>();
            }

            // Build the EdismaxSolrQuery object from the Sitecore item.
            EdismaxSolrQuery query = BuildEdismaxSolrQuery(solrQueryItem);

            return EdismaxSearch<T>(q, query, startRow, language, additionalFilters);
        }

        /// <summary>
        /// Search for content using a Sitecore <see cref="Item"/>.
        /// </summary>
        protected virtual SolrQueryResults<T> EdismaxSearch<T>(
            string q, 
            EdismaxSolrQuery query, 
            int startRow = 0, 
            string language = "",
            ICollection<ISolrQuery> additionalFilters = null)
        {
            // Validate the parameters.
            if (string.IsNullOrWhiteSpace(q) || query == null)
            {
                return new SolrQueryResults<T>();
            }

            // Filters: Base Filters.
            var filters = new List<ISolrQuery>();

            // Base Filters: Language
            language = !string.IsNullOrWhiteSpace(language)
                ? language
                : Sitecore.Context.Language.Name;

            filters.Add(new SolrQueryByField("_language", language));

            // Base Filters: do NOT return standard values items.
            filters.Add(!new SolrQueryByField("_name", Sitecore.Constants.StandardValuesItemName));

            // Filters: Locations.
            SolrMultipleCriteriaQuery locationsQuery = BuildMultipleCriteriaQuery(query.Locations, "_path");
            if (locationsQuery != null)
            {
                filters.Add(locationsQuery);
            }

            // Filters: Templates.
            SolrMultipleCriteriaQuery templateQuery = BuildMultipleCriteriaQuery(query.Templates, "_template");
            if (templateQuery != null)
            {
                filters.Add(templateQuery);
            }

            // Additional Filters
            if (additionalFilters != null && additionalFilters.Any())
            {
                filters.AddRange(additionalFilters);
            }

            // Edismax Parameters
            var extraParams = new List<KeyValuePair<string, string>>();

            // Use Edismax
            extraParams.Add(new KeyValuePair<string, string>("defType", "edismax"));

            // Query Fields (qf)
            string qf = BuildFlatBoostString(query.QueryFields);
            if (!string.IsNullOrWhiteSpace(qf))
            {
                extraParams.Add(new KeyValuePair<string, string>("qf", qf));
            }

            // Query Tie
            if (query.Tie > 0)
            {
                extraParams.Add(new KeyValuePair<string, string>("tie", query.Tie.ToString()));
            }

            // Minimum Match
            if (!string.IsNullOrWhiteSpace(query.MinimumMatch))
            {
                extraParams.Add(new KeyValuePair<string, string>("mm", query.MinimumMatch));
            }

            // Minimum Match Auto Relax
            if (!string.IsNullOrWhiteSpace(query.MinimumMatchAutoRelax))
            {
                extraParams.Add(new KeyValuePair<string, string>("mm.autoRelax", query.MinimumMatchAutoRelax));
            }

            // Phrase Fields (pf)
            string pf = BuildFlatBoostString(query.PhraseFields);
            if (!string.IsNullOrWhiteSpace(pf))
            {
                extraParams.Add(new KeyValuePair<string, string>("pf", pf));
            }

            // Phrase Fields 2 (pf2)
            string pf2 = BuildFlatBoostString(query.PhraseFields2);
            if (!string.IsNullOrWhiteSpace(pf2))
            {
                extraParams.Add(new KeyValuePair<string, string>("pf2", pf2));
            }

            // Phrase Fields 3 (pf3)
            string pf3 = BuildFlatBoostString(query.PhraseFields3);
            if (!string.IsNullOrWhiteSpace(pf3))
            {
                extraParams.Add(new KeyValuePair<string, string>("pf3", pf3));
            }

            // PhraseSlop (ps)
            if (query.PhraseSlop > 0)
            {
                extraParams.Add(new KeyValuePair<string, string>("ps", query.PhraseSlop.ToString()));
            }

            // Phrase Slop 2 (ps2)
            if (query.PhraseSlop2 > 0)
            {
                extraParams.Add(new KeyValuePair<string, string>("ps2", query.PhraseSlop2.ToString()));
            }

            // Phrase Slop 3 (ps3)
            if (query.PhraseSlop3 > 0)
            {
                extraParams.Add(new KeyValuePair<string, string>("ps3", query.PhraseSlop3.ToString()));
            }

            // Boost Query (bq)
            if (!string.IsNullOrWhiteSpace(query.BoostQuery))
            {
                extraParams.Add(new KeyValuePair<string, string>("bq", query.BoostQuery));
            }

            // Boost Function (bf)
            if (!string.IsNullOrWhiteSpace(query.BoostFunction))
            {
                extraParams.Add(new KeyValuePair<string, string>("bf", query.BoostFunction));
            }

            // Boost (boost)
            if (!string.IsNullOrWhiteSpace(query.Boost))
            {
                extraParams.Add(new KeyValuePair<string, string>("boost", query.Boost));
            }

            // Facets
            if (query.Facet)
            {
                extraParams.Add(new KeyValuePair<string, string>("facet", "true"));

                // Facet Field
                if (!string.IsNullOrWhiteSpace(query.FacetField))
                {
                    extraParams.Add(new KeyValuePair<string, string>("facet.field", query.FacetField));
                }

                // Facet Query (facet.query)
                if (!string.IsNullOrWhiteSpace(query.FacetQuery))
                {
                    extraParams.Add(new KeyValuePair<string, string>("facet.query", query.FacetQuery));
                }

                // Facet Prefix (facet.prefix)
                if (!string.IsNullOrWhiteSpace(query.FacetPrefix))
                {
                    extraParams.Add(new KeyValuePair<string, string>("facet.prefix", query.FacetPrefix));
                }
            }

            // Spellcheck Dictionary (spellcheck.dictionary):
            if (query.Spellcheck)
            {
                extraParams.Add(new KeyValuePair<string, string>("spellcheck", "true"));

                // Spellcheck Dictionary Name, as set in solr configs. (spellcheck.dictionary)
                if (!string.IsNullOrWhiteSpace(query.SpellcheckDictionary))
                {
                    extraParams.Add(new KeyValuePair<string, string>("spellcheck.dictionary", query.SpellcheckDictionary));
                }

                // Spellcheck Dictionary (spellcheck.count):
                if (query.SpellcheckCount > 0)
                {
                    extraParams.Add(new KeyValuePair<string, string>("spellcheck.count", query.SpellcheckCount.ToString()));
                }

                extraParams.Add(new KeyValuePair<string, string>("spellcheck.onlyMorePopular", query.SpellcheckOnlyMorePopular.ToString().ToLowerInvariant()));
                extraParams.Add(new KeyValuePair<string, string>("spellcheck.extendedResults", query.SpellcheckExtendedResults.ToString().ToLowerInvariant()));

                // Spellcheck collate
                if (query.SpellcheckCollate)
                {
                    extraParams.Add(new KeyValuePair<string, string>("spellcheck.collate", "true"));

                    // Spellcheck Max Collations (spellcheck.maxCollations):
                    if (query.SpellcheckCount > 0)
                    {
                        extraParams.Add(new KeyValuePair<string, string>("spellcheck.maxCollations", query.SpellcheckMaxCollations.ToString()));
                    }

                    // Spellcheck Max Collations (spellcheck.maxCollationTries):
                    if (query.SpellcheckMaxCollationTries > 0)
                    {
                        extraParams.Add(new KeyValuePair<string, string>("spellcheck.maxCollationTries", query.SpellcheckMaxCollationTries.ToString()));
                    }

                    // Spellcheck Max Collations (spellcheck.accuracy):
                    if (query.SpellcheckAccuracy > 0)
                    {
                        extraParams.Add(new KeyValuePair<string, string>("spellcheck.accuracy", query.SpellcheckAccuracy.ToString()));
                    }

                    // Spellcheck Collate Extended Results (spellcheck.collateExtendedResults)
                    extraParams.Add(new KeyValuePair<string, string>("spellcheck.collateExtendedResults", query.SpellcheckCollateExtendedResults.ToString().ToLowerInvariant()));
                }
            }

            // Highlight
            if (query.Highlight)
            {
                extraParams.Add(new KeyValuePair<string, string>("hl", "true"));

                // Highlight Fields (hl.fl)
                if (!string.IsNullOrWhiteSpace(query.HighlightFields))
                {
                    extraParams.Add(new KeyValuePair<string, string>("hl.fl", query.HighlightFields));
                }

                // Highlight Pre
                if (!string.IsNullOrWhiteSpace(query.HighlightPre))
                {
                    extraParams.Add(new KeyValuePair<string, string>("hl.simple.pre", query.HighlightPre));
                }

                // Highlight Post
                if (!string.IsNullOrWhiteSpace(query.HighlightPost))
                {
                    extraParams.Add(new KeyValuePair<string, string>("hl.simple.post", query.HighlightPost));
                }

                // Hightlight requireFieldMatch
                extraParams.Add(new KeyValuePair<string, string>("hl.requireFieldMatch", query.HighlightRequireFieldMatch.ToString().ToLowerInvariant()));
                
                // Hightlight use Phrase Highlighter
                extraParams.Add(new KeyValuePair<string, string>("hl.usePhraseHighlighter", query.HighlightUsePhaseHighlighter.ToString().ToLowerInvariant()));

                // Highlight FragSize
                extraParams.Add(new KeyValuePair<string, string>("hl.fragsize", query.HighlightFragSize.ToString()));

                // Highlight Highlight Snippets
                extraParams.Add(new KeyValuePair<string, string>("hl.snippets", query.HighlightSnippets.ToString()));                
            }

            // Execute the Solr query.
            using (IProviderSearchContext ctx = BuildSearchIndex(query).CreateSearchContext())
            {
                SolrQueryResults<T> results = ctx.Query<T>(q, new SolrNet.Commands.Parameters.QueryOptions
                {
                    Rows = query.Rows,
                    StartOrCursor = new StartOrCursor.Start(startRow),
                    ExtraParams = extraParams,
                    FilterQueries = filters,
                    Debug = query.DebugQuery,
                    OrderBy = BuildOrderBy(query.Sort).ToList()
                });

                return results;
            }
        }

        /// <summary>
        /// Builds the strongly-typed <see cref="EdismaxSolrQuery"/> instance from the given Sitecore Item id.
        /// </summary>
        protected virtual EdismaxSolrQuery BuildEdismaxSolrQuery(Item item)
        {
            if (item == null)
            {
                throw new ArgumentException(string.Format("Item not found for ID {0}", item));
            }

            return new EdismaxSolrQuery
            {
                IndexName = item.RenderField(FieldIdFor(p => p.IndexName)),
                Rows = item.RenderIntegerField(FieldIdFor(p => p.Rows), 10),
                Sort = item.RenderField(FieldIdFor(p => p.Sort)),
                DebugQuery = item.RenderBoolField(FieldIdFor(p => p.DebugQuery)),

                QueryFields = item.RenderNameValueField(FieldIdFor(p => p.QueryFields)),
                Tie = item.RenderFloatField(FieldIdFor(p => p.Tie)),

                Locations = item.RenderGuidsField(FieldIdFor(p => p.Locations)),
                Templates = item.RenderGuidsField(FieldIdFor(p => p.Templates)),

                MinimumMatch = item.RenderField(FieldIdFor(p => p.MinimumMatch)),
                MinimumMatchAutoRelax = item.RenderField(FieldIdFor(p => p.MinimumMatchAutoRelax)),

                PhraseFields = item.RenderNameValueField(FieldIdFor(p => p.PhraseFields)),
                PhraseFields2 = item.RenderNameValueField(FieldIdFor(p => p.PhraseFields2)),
                PhraseFields3 = item.RenderNameValueField(FieldIdFor(p => p.PhraseFields3)),

                PhraseSlop = item.RenderFloatField(FieldIdFor(p => p.PhraseSlop)),
                PhraseSlop2 = item.RenderFloatField(FieldIdFor(p => p.PhraseSlop2)),
                PhraseSlop3 = item.RenderFloatField(FieldIdFor(p => p.PhraseSlop3)),

                Boost = item.RenderField(FieldIdFor(p => p.Boost)),
                BoostFunction = item.RenderField(FieldIdFor(p => p.BoostFunction)),
                BoostQuery = item.RenderField(FieldIdFor(p => p.BoostQuery)),

                Facet = item.RenderBoolField(FieldIdFor(p => p.Facet)),
                FacetField = item.RenderField(FieldIdFor(p => p.FacetField)),
                FacetQuery = item.RenderField(FieldIdFor(p => p.FacetQuery)),
                FacetPrefix = item.RenderField(FieldIdFor(p => p.FacetPrefix)),

                Spellcheck = item.RenderBoolField(FieldIdFor(p => p.Spellcheck)),
                SpellcheckDictionary = item.RenderField(FieldIdFor(p => p.SpellcheckDictionary)),
                SpellcheckCount = item.RenderIntegerField(FieldIdFor(p => p.SpellcheckCount)),
                SpellcheckOnlyMorePopular = item.RenderBoolField(FieldIdFor(p => p.SpellcheckOnlyMorePopular)),
                SpellcheckExtendedResults = item.RenderBoolField(FieldIdFor(p => p.SpellcheckExtendedResults)),
                SpellcheckCollate = item.RenderBoolField(FieldIdFor(p => p.SpellcheckCollate)),
                SpellcheckMaxCollations = item.RenderIntegerField(FieldIdFor(p => p.SpellcheckMaxCollations)),
                SpellcheckMaxCollationTries = item.RenderIntegerField(FieldIdFor(p => p.SpellcheckMaxCollationTries)),
                SpellcheckCollateExtendedResults = item.RenderBoolField(FieldIdFor(p => p.SpellcheckCollateExtendedResults)),
                SpellcheckAccuracy = item.RenderFloatField(FieldIdFor(p => p.SpellcheckAccuracy)),

                Highlight = item.RenderBoolField(FieldIdFor(p => p.Highlight)),
                HighlightMethod = item.RenderField(FieldIdFor(p => p.HighlightMethod)),
                HighlightPre = item.RenderField(FieldIdFor(p => p.HighlightPre)),
                HighlightPost = item.RenderField(FieldIdFor(p => p.HighlightPost)),
                HighlightRequireFieldMatch = item.RenderBoolField(FieldIdFor(p => p.HighlightRequireFieldMatch)),
                HighlightUsePhaseHighlighter = item.RenderBoolField(FieldIdFor(p => p.HighlightUsePhaseHighlighter)),
                HighlightFragSize = item.RenderIntegerField(FieldIdFor(p => p.HighlightFragSize)),
                HighlightFields = item.RenderField(FieldIdFor(p => p.HighlightFields)),
                HighlightSnippets = item.RenderIntegerField(FieldIdFor(p => p.HighlightSnippets))
            };
        }

        /// <summary>
        /// Builds the order by list from a comma-delimited string.
        /// </summary>
        protected virtual IEnumerable<SortOrder> BuildOrderBy(string sortBy)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                // Get each sort field by splitting on commas.
                string[] sortFields = sortBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string sf in sortFields)
                {
                    // Parse the field name and order, e.g. 'title_t ASC'
                    string[] sortItem = sf.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sortItem.Length == 2)
                    {
                        string field = sortItem[0];     // title_t
                        string direction = sortItem[1]; // ASC or DESC

                        Order order = direction.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? Order.DESC : Order.ASC;

                        yield return new SortOrder(field, order);
                    }
                    else
                    {
                        // Default to Ascending.
                        yield return new SortOrder(sf, Order.ASC);
                    }
                }
            }
        }

        /// <summary>
        /// Builds a flat boost string from a <see cref="NameValueCollection"/>.
        /// </summary>
        protected virtual string BuildFlatBoostString(NameValueCollection nvc)
        {
            IEnumerable<string> boosts = BuildBoosts(nvc);

            return string.Join(" ", boosts);
        }

        /// <summary>
        /// Builds a lists of boosts from a <see cref="NameValueCollection"/>.
        /// </summary>
        protected virtual IEnumerable<string> BuildBoosts(NameValueCollection nvc)
        {
            if (nvc != null)
            {
                var items = nvc.AllKeys.SelectMany(nvc.GetValues, (k, v) => new { Key = k, Value = v });

                foreach (var item in items)
                {
                    // No boost.
                    if (string.IsNullOrWhiteSpace(item.Value))
                    {
                        yield return item.Key;
                    }

                    yield return string.Concat(item.Key, "^", item.Value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="SolrMultipleCriteriaQuery"/> from a list of guids.
        /// </summary>
        protected virtual SolrMultipleCriteriaQuery BuildMultipleCriteriaQuery(IEnumerable<Guid> guidList, string fieldName, string oper = "OR")
        {
            if (guidList != null 
                && guidList.Any() 
                && !string.IsNullOrWhiteSpace(fieldName) 
                && !string.IsNullOrWhiteSpace(oper))
            {
                var queries = new List<ISolrQuery>();

                foreach(Guid g in guidList)
                {
                    queries.Add(new SolrQueryByField(fieldName, FormatIdForSearch(g)));
                }

                return new SolrMultipleCriteriaQuery(queries, oper);
            }

            return null;
        }

        /// <summary>
        /// Builds the Search index.
        /// </summary>
        protected virtual ISearchIndex BuildSearchIndex(EdismaxSolrQuery query)
        {
            if (query != null && !string.IsNullOrWhiteSpace(query.IndexName))
            {
                return ContentSearchManager.GetIndex(query.IndexName);
            }

            // Build the index using the current context.
            Item homeItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);
            var indexable = new SitecoreIndexableItem(homeItem);

            return ContentSearchManager.GetIndex(indexable);
        }

        /// <summary>
        /// Formats a Sitecore ID or Guid for solr search.
        /// </summary>
        protected virtual string FormatIdForSearch(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id.ToLowerInvariant()
                    .Replace("-", string.Empty)
                    .Replace("{", string.Empty)
                    .Replace("}", string.Empty);
            }

            return string.Empty;
        }

        /// <summary>
        /// Formats a Sitecore ID or Guid for solr search.
        /// </summary>
        protected virtual string FormatIdForSearch(Sitecore.Data.ID id)
        {
            return FormatIdForSearch(id.ToString());
        }

        /// <summary>
        /// Formats a Sitecore ID or Guid for solr search.
        /// </summary>
        protected virtual string FormatIdForSearch(Guid id)
        {
            return FormatIdForSearch(id.ToString());
        }

        /// <summary>
        /// Convenience method that gets the Sitecore Field ID for a <see cref="EdismaxSolrQuery"/> property.
        /// </summary>
        protected virtual Guid FieldIdFor<TOut>(Expression<Func<EdismaxSolrQuery, TOut>> propertyExpression)
        {
            return GetPropValue<EdismaxSolrQuery, TOut, SitecoreFieldAttribute, Guid>(propertyExpression, attr => attr.FieldId);
        }

        /// <summary>
        /// Gets the Attribute value from a property.
        /// See: https://stackoverflow.com/a/32501356
        /// </summary>
        protected virtual TValue GetPropValue<T, TOut, TAttribute, TValue>(
            Expression<Func<T, TOut>> propertyExpression,
            Func<TAttribute, TValue> valueSelector)
        where TAttribute : Attribute
        {
            var expression = (MemberExpression)propertyExpression.Body;
            var propertyInfo = (PropertyInfo)expression.Member;
            var attr = propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;

            return attr != null ? valueSelector(attr) : default(TValue);
        }
    }
}