using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using System;

namespace Feature.Demo.Search.Services.Search.Models
{
    public class PageContentResultItem : SitecoreUISearchResultItem
    {
        [IndexField("title_t")]
        public virtual string Title { get; set; }

        [IndexField("description_t")]
        public virtual string Description { get; set; }

        //[IndexField("description_t")]
        //public virtual string ArticleType { get; set; }

        //[IndexField("display_date_tdt")]
        //public virtual DateTime DisplayDate { get; set; }
    }
}