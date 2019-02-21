using Feature.Demo.Articles.Services.BlogBorrower.Models;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Feature.Demo.Articles.Services.BlogBorrower.Services
{
    public class SitecoreFeedBlogBorrower : IBlogBorrower
    {
        protected const string FeedUrl = "http://feeds.sitecore.net/Feed/LatestPosts";

        /// <summary>
        /// Gets all the <see cref="BlogFeedItem"/> items from http://feeds.sitecore.net/Feed/LatestPosts.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BlogFeedItem> GetAll()
        {
            XmlReader reader = XmlReader.Create(FeedUrl);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            foreach (SyndicationItem item in feed.Items)
            {
                yield return new BlogFeedItem
                {
                    Title = item.Title.Text,
                    Description = item.Summary.Text,
                    Updated = item.LastUpdatedTime,
                    Link = item.Links.Any() ? item.Links.First().Uri.ToString() : null,
                    FeedID = item.Id
                };
            }
        }

        
    }
}