using System;

namespace Feature.Demo.Articles.Services.BlogBorrower.Models
{
    /// <summary>
    /// POCO that represents a blog feed item from: http://feeds.sitecore.net/Feed/LatestPosts
    /// </summary>
    public class BlogFeedItem
    {
        public string FeedID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public DateTimeOffset Updated { get; set; }
    }
}