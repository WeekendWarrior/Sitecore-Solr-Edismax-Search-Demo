using System;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Foundation.Demo.Search.Services
{
    /// <summary>
    /// Maps to the sitecore item for defining a Solr Edismax Query.
    /// Template: /sitecore/templates/Foundation/Solr Query
    /// </summary>
    public class EdismaxSolrQuery
    {
        /// <summary>
        /// Get/set the index to query. Leave null or empty to query the context database.
        /// </summary>
        [SitecoreField("Index Name", "{C77F0C8A-6CB5-49E6-B9E5-FFC42AA602FE}")]
        public string IndexName { get; set; }

        /// <summary>
        /// Get/set the number of rows to return in the result set.
        /// </summary>
        [SitecoreField("rows", "{97F3D687-9506-4D38-A8C3-09F92DFAA4A0}")]
        public int Rows { get; set; }

        /// <summary>
        /// Get/Set the sort (e.g. title_t desc). Note that this negates all document scores and relevancy, so its not recommended to set a manual sort, so use wisely.
        /// </summary>        /// 
        [SitecoreField("sort", "{FECC5AF3-7728-4B4E-BDDD-7F53272D4D05}")]
        public string Sort { get; set; }

        /// <summary>
        /// Gets/sets the flag that enables query debugging.
        /// </summary>
        [SitecoreField("debug", "{4DD25D78-A4B4-4888-BAC7-F7AAAA4BDA05}")]
        public bool DebugQuery { get; set; }

        /// <summary>
        /// Get/set the query locations for filtering. Note filters do not affect boosting scores / relevancy.
        /// </summary>
        [SitecoreField("Locations", "{566E8C87-29C6-413A-8A9F-4E05D22366C2}")]
        public Guid[] Locations { get; set; }

        /// <summary>
        /// Get/set the query templates for filtering. Note filters do not affect boosting scores / relevancy.
        /// </summary>
        [SitecoreField("Templates", "{B1E24936-CD9F-47D5-809E-3D4ADFCDEC9A}")]
        public Guid[] Templates { get; set; }

        /// <summary>
        /// The fields to query.
        /// </summary>
        /// </summary>
        [SitecoreField("qf", "{ECD7BF38-6CEF-4ECF-B817-E445F334C454}")]
        public NameValueCollection QueryFields { get; set; }

        /// <summary>
        /// The tie parameter specifies a float value (which should be something much less than 1) to use as tiebreaker in DisMax / EDixMax queries.
        /// </summary>
        [SitecoreField("tie", "{76271495-B548-4074-90B2-A42B9FFB77FD}")]
        public float Tie { get; set; }

        /// <summary>
        /// Gets/sets the minimum match.
        /// </summary>
        [SitecoreField("mm", "{43ECA852-9923-4204-96A7-FBA9832AD540}")]
        public string MinimumMatch { get; set; }

        /// <summary>
        /// Gets/sets the minimum match auto relax.
        /// </summary>
        [SitecoreField("mmAutoRelax", "{8E285186-3842-4176-8E24-0DE9721A5337}")]
        public string MinimumMatchAutoRelax { get; set; }

        /// <summary>
        /// Gets/Sets the phrase fields.
        /// </summary>
        [SitecoreField("pf", "{E57A2467-8B0F-4071-8898-D20B9D9FCF18}")]
        public NameValueCollection PhraseFields { get; set; }

        /// <summary>
        /// Gets/Sets the phrase 2 fields.
        /// </summary>
        [SitecoreField("pf2", "{2B7836EB-34D9-4F15-98F9-E742BA0A5CF4}")]
        public NameValueCollection PhraseFields2 { get; set; }

        /// <summary>
        /// Gets/Sets the phrase 3 fields.
        /// </summary>
        [SitecoreField("pf3", "{9DE11F40-BC71-40AB-8165-D0C26FC80C90}")]
        public NameValueCollection PhraseFields3 { get; set; }

        /// <summary>
        /// Get/Set the phrase slop
        /// </summary>
        [SitecoreField("ps", "{491EAFBD-7404-4B3B-B488-02FD28B61D3A}")]
        public float PhraseSlop { get; set; }

        /// <summary>
        /// Gets/Sets the phrase slop 2 fields.
        /// </summary>
        [SitecoreField("ps2", "{0FDF2E23-7C83-44C3-A425-28FD138B8556}")]
        public float PhraseSlop2 { get; set; }

        /// <summary>
        /// Gets/Sets the phrase slop 3 fields.
        /// </summary>
        [SitecoreField("ps3", "{56D64224-8A77-432C-9A76-3A353EE6077A}")]
        public float PhraseSlop3 { get; set; }

        /// <summary>
        /// Gets/sets the boost field (usually a function, and different from the "BF" parameter).
        /// </summary>
        [SitecoreField("boost", "{B37C5B9E-3282-446A-A0A9-D11975C41C21}")]
        public string Boost { get; set; }

        /// <summary>
        /// Gets/sets the boost function (Note this is different from the "boost" parameter).
        /// </summary>
        [SitecoreField("bf", "{A1546FA7-FBBF-48FB-86C7-591F126F2454}")]
        public string BoostFunction { get; set; }

        /// <summary>
        /// Gets/sets the boost query.
        /// </summary>
        [SitecoreField("bq", "{590D1FEE-1B01-4184-AC2A-01D9663AC1E2}")]
        public string BoostQuery { get; set; }

        /// <summary>
        /// Gets/sets if facetting is on.
        /// </summary>
        [SitecoreField("facet", "{EAEF823A-9751-4198-8B39-95172185A677}")]
        public bool Facet { get; set; }

        /// <summary>
        /// Gets/sets the facet field.
        /// </summary>
        [SitecoreField("facetField", "{BF0E4F5D-D4D6-4EE4-A6B2-C6B69C5690AB}")]
        public string FacetField { get; set; }

        /// <summary>
        /// Gets/sets the facet field.
        /// </summary>
        [SitecoreField("facetQuery", "{9AE20EF3-2C0B-4E86-83D3-6BA540F8A5E1}")]
        public string FacetQuery { get; set; }

        /// <summary>
        /// Gets/sets the facet prefix.
        /// </summary>
        [SitecoreField("facetPrefix", "{DB030B5E-673B-40A6-B4A8-D415852B439C}")]
        public string FacetPrefix { get; set; }

        /// <summary>
        /// Gets/sets if spellcheck is enabled.
        /// </summary>
        [SitecoreField("spellcheck", "{5D4444B6-9E21-4347-8644-496E4D487222}")]
        public bool Spellcheck { get; set; }

        /// <summary>
        /// Gets/sets the name of the spellcheck dictionary to use.
        /// </summary>
        [SitecoreField("spellcheckDictionary", "{20E3B2E2-1C9D-45ED-B2BD-D70491A25FF6}")]
        public string SpellcheckDictionary { get; set; }

        /// <summary>
        /// Gets/sets the spellcheck count.
        /// </summary>
        [SitecoreField("spellcheckCount", "{09B310F2-758B-4342-95B0-572863AFDCE1}")]
        public int SpellcheckCount{ get; set; }

        /// <summary>
        /// Gets/sets the spellcheck flag to only return more popular results.
        /// </summary>
        [SitecoreField("spellcheckOnlyMorePopular", "{16C02170-9B49-4D2A-A4E0-B98A224C34D5}")]
        public bool SpellcheckOnlyMorePopular { get; set; }

        /// <summary>
        /// Gets/sets the spellcheck extended results flag.
        /// </summary>
        [SitecoreField("spellcheckExtendedResults", "{164640AC-CCC2-478A-A577-35D2D3BB35F9}")]
        public bool SpellcheckExtendedResults{ get; set; }

        /// <summary>
        /// Gets/sets the spellcheck collate.
        /// </summary>
        [SitecoreField("spellcheckCollate", "{BF9B121C-64A7-4895-B912-1C8821F010E6}")]
        public bool SpellcheckCollate { get; set; }

        /// <summary>
        /// Gets/sets the spellcheck max collations.
        /// </summary>
        [SitecoreField("spellcheckMaxCollations", "{89BDA16E-7487-49EC-838E-B7498278E41D}")]
        public int SpellcheckMaxCollations { get; set; }

        /// <summary>
        /// Gets/sets the spellcheck max collations.
        /// </summary>
        [SitecoreField("spellcheckMaxCollationTries", "{F8725436-0762-4FE6-B1F1-3BD16B10458B}")]
        public int SpellcheckMaxCollationTries { get; set; }

        /// <summary>
        /// Gets / Sets spellcheck collate extended results.
        /// </summary>
        [SitecoreField("spellcheckCollateExtendedResults", "{9417CC5B-B74B-4A7E-BF17-17C0018501AE}")]
        public bool SpellcheckCollateExtendedResults { get; set; }

        /// <summary>
        /// Gets/sets the spellcheck accuracy.
        /// </summary>
        [SitecoreField("spellcheckAccuracy", "{8F6D47C0-A413-49E6-9D05-17165D330265}")]
        public float SpellcheckAccuracy { get; set; }

        /// <summary>
        /// Gets/sets if highlighting is enabled.
        /// </summary>
        [SitecoreField("highlight", "{D62DF421-2B7A-44CA-A2AE-CDC15CB0E712}")]
        public bool Highlight { get; set; }

        /// <summary>
        /// The highlighting implementation to use. Acceptable values are: unified, original, fastVector, and postings. If blank, "original" wiis the default in solr.
        /// </summary>
        [SitecoreField("highlightMethod", "{89BC9307-2284-488E-9969-93322CC2340C}")]
        public string HighlightMethod { get; set; }

        /// <summary>
        /// Specifies the “tag” to use before a highlighted term. This can be any string, but is most often an HTML or XML tag.
        /// </summary>
        [SitecoreField("highlightPre", "{84C3F6A1-1EBB-4287-8061-40C87BCB7DE9}")]
        public string HighlightPre { get; set; }

        /// <summary>
        /// Specifies the “tag” to use after a highlighted term. This can be any string, but is most often an HTML or XML tag.
        /// </summary>
        [SitecoreField("highlightPost", "{BAA1E790-1576-4AC5-8F5B-7D74355304AB}")]
        public string HighlightPost { get; set; }

        /// <summary>
        /// Gets/sets the highlight requireFieldMatch. 
        /// By default, false, all query terms will be highlighted for each field to be highlighted (hl.fl) no matter what fields the parsed query refer to.
        /// If set to true, only query terms aligning with the field being highlighted will in turn be highlighted.
        /// </summary>
        [SitecoreField("highlightRequireFieldMatch", "{15BA2BE7-CB5D-44C2-BB58-ADCE3C77DF16}")]
        public bool HighlightRequireFieldMatch { get; set; }

        /// <summary>
        /// If set to true, Solr will highlight phrase queries (and other advanced position-sensitive queries) accurately – as phrases.
        /// If false, the parts of the phrase will be highlighted everywhere instead of only when it forms the given phrase.
        /// </summary>
        [SitecoreField("highlightUsePhraseHighlighter", "{684CE51C-0EAB-44B0-92C1-CE17E5364F1D}")]
        public bool HighlightUsePhaseHighlighter { get; set; }

        /// <summary>
        /// Specifies the approximate size, in characters, of fragments to consider for highlighting. 
        /// 0 indicates that no fragmenting should be considered and the whole field value should be used.
        /// </summary>
        [SitecoreField("highlightFragSize", "{74309B6F-7C26-486F-8C81-0E9DB07CFBD0}")]
        public int HighlightFragSize { get; set; }

        /// <summary>
        /// Specifies a list of fields to highlight. If left blank, the "qf" field is used automatically by solr.
        /// Accepts a comma- or space-delimited list of fields for which Solr should generate highlighted snippets.
        /// </summary>
        [SitecoreField("highlightFields", "{C955BE3C-8167-41D0-AFD6-8206639B75E2}")]
        public string HighlightFields { get; set; }

        /// <summary>
        /// Specifies maximum number of highlighted snippets to generate per field.
        /// It is possible for any number of snippets from zero to this value to be generated.
        /// If left blank, Solr default of 1 will be used.
        /// </summary>
        [SitecoreField("highlightSnippets", "{E4374C25-1F87-425B-A98B-61C6B539F445}")]
        public int HighlightSnippets { get; set; }
        
    }

    [DebuggerDisplay("FieldName = {FieldName}, BoostValue = {BoostValue}")]
    public class Field
    {
        public string FieldName { get; set; }

        public float BoostValue { get; set; }
    }
}