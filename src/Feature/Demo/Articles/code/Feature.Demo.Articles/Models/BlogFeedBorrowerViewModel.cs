using Feature.Demo.Articles.Services.BlogBorrower.Models;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;

namespace Feature.Demo.Articles.Models
{
    public class BlogFeedBorrowerViewModel
    {
        public BlogFeedItem[] FeedItems { get; set; }

        public Item[] SitecoreItems { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="BlogFeedBorrowerViewModel"/> class.
        /// </summary>
        public BlogFeedBorrowerViewModel()
        {
            FeedItems = Enumerable.Empty<BlogFeedItem>().ToArray();

            SitecoreItems = Enumerable.Empty<Item>().ToArray();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BlogFeedBorrowerViewModel"/> class.
        /// </summary>
        public BlogFeedBorrowerViewModel(IEnumerable<BlogFeedItem> feedItems, IEnumerable<Item> sitecoreItems)
        {
            FeedItems = feedItems != null 
                ? feedItems.ToArray() 
                : Enumerable.Empty<BlogFeedItem>().ToArray();

            SitecoreItems = sitecoreItems != null 
                ? sitecoreItems.ToArray() 
                : Enumerable.Empty<Item>().ToArray();
        }
    }
}