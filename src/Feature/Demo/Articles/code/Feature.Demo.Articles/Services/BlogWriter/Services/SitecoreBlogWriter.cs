using Feature.Demo.Articles.Services.BlogBorrower.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Feature.Demo.Articles.Services.BlogWriter.Services
{
    /// <summary>
    /// Writes blogs into Sitecore
    /// </summary>
    public class SitecoreBlogWriter : IBlogWriter
    {
        /// <summary>
        /// Writes blogs into Sitecore
        /// </summary>
        public IEnumerable<Item> WriteBlogs(IEnumerable<BlogFeedItem> feedItems)
        {
            if (feedItems != null && feedItems.Any())
            {
                // Get the Article root item in master database.
                Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                Item parentItem = master.GetItem(Constants.ArticlesRootItemID);

                var results = new List<Item>();

                foreach (BlogFeedItem feedItem in feedItems)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        Item yearItem = CreateOrGetYearItem(parentItem, feedItem);

                        Item monthItem = CreateOrGetMonthItem(yearItem, feedItem);

                        Item articleItem = CreateOrGetArticleItem(monthItem, feedItem);

                        UpdateArticleItem(articleItem, feedItem);

                        PublishItems(yearItem, monthItem, articleItem);

                        yield return articleItem;
                    }
                }
            }
        }

        private struct Constants
        {
            /// <summary>
            /// /sitecore/content/Home/Articles
            /// </summary>
            public static ID ArticlesRootItemID = new ID("{9E19ADE4-04A5-47E7-ACCB-5F0168A512C1}");

            /// <summary>
            /// /sitecore/templates/Project/Demo/Articles/Article Folder
            /// </summary>
            public static TemplateID ArticleFolderTemplateID = new TemplateID(new ID("{A6270531-9C4C-4191-9870-8DE5EE8EB810}"));

            /// <summary>
            /// /sitecore/templates/Project/Demo/Articles/Article
            /// </summary>
            public static TemplateID ArticleTemplateID = new TemplateID(new ID("{DCE93757-77A5-4B68-A276-227CFB6960F4}"));

            /// <summary>
            /// /sitecore/content/Global/Article Types/Blog Post
            /// </summary>
            public static string ArticleType_BlogPost = "{E4BF1B57-1135-45AD-8C23-8E4FE29B9B15}";
        }

        #region infrastructure

        /// <summary>
        /// Gets or creates a year item.
        /// </summary>
        private Item CreateOrGetYearItem(Item parentItem, BlogFeedItem feedItem)
        {
            Item yearItem = parentItem.Axes.GetChild(feedItem.Updated.Year.ToString());
            if (yearItem == null)
            {
                yearItem = parentItem.Add(feedItem.Updated.Year.ToString(), Constants.ArticleFolderTemplateID);
            }

            return yearItem;
        }

        /// <summary>
        /// Gets or creates a month item.
        /// </summary>
        private Item CreateOrGetMonthItem(Item yearItem, BlogFeedItem feedItem)
        {
            // Create or get the Month folder item.
            string monthName = feedItem.Updated.Month < 10
                ? string.Concat("0", feedItem.Updated.Month.ToString())
                : feedItem.Updated.Month.ToString();

            Item monthItem = yearItem.Axes.GetChild(monthName);
            if (monthItem == null)
            {
                monthItem = yearItem.Add(monthName, Constants.ArticleFolderTemplateID);
            }

            return monthItem;
        }

        /// <summary>
        /// Gets or creates an article item.
        /// </summary>
        private Item CreateOrGetArticleItem(Item monthItem, BlogFeedItem feedItem)
        {
            Guid feedItemID = Guid.Empty;

            if (Guid.TryParse(feedItem.FeedID, out feedItemID))
            {
                Item articleItem = monthItem.Axes.GetChild(new ID(feedItemID));

                if (articleItem == null)
                {
                    string itemName = ItemUtil.ProposeValidItemName(feedItem.Title);

                    articleItem = monthItem.Add(itemName, Constants.ArticleTemplateID, new ID(feedItemID));
                }

                return articleItem;
            }

            return null;
        }

        /// <summary>
        /// Updates an Article item.
        /// </summary>
        private static void UpdateArticleItem(Item articleItem, BlogFeedItem feedItem)
        {
            // Set the item fields
            articleItem.Editing.BeginEdit();
            try
            {
                articleItem["Title"] = feedItem.Title;
                articleItem["Description"] = feedItem.Description;
                articleItem["Article Type"] = Constants.ArticleType_BlogPost;

                // Display Date
                DateField displayDate = articleItem.Fields["Display Date"];
                displayDate.Value = DateUtil.ToIsoDate(feedItem.Updated.Date);

                // Link
                LinkField link = articleItem.Fields["Link"];
                link.Url = feedItem.Link;
                link.Target = "_blank";
                link.LinkType = "external";
            }
            finally
            {
                articleItem.Editing.EndEdit();
            }
        }

        /// <summary>
        /// Publishes a list of items.
        /// </summary>>
        private void PublishItems(params Item[] items)
        {
            if (items != null)
            {
                foreach (Item item in items)
                {
                    PublishItem(item);
                }
            }
        }

        /// <summary>
        /// Publishes a Sitecore <see cref="Item"/>.
        /// </summary>
        private static void PublishItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                Database web = Sitecore.Configuration.Factory.GetDatabase("web");

                var publishOptions = new Sitecore.Publishing.PublishOptions(
                    master,
                    web,
                    Sitecore.Publishing.PublishMode.SingleItem,
                    item.Language,
                    DateTime.Now);

                Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions);

                publisher.Options.RootItem = item;

                publisher.Options.Deep = true;

                publisher.Publish();
            }
        }

        #endregion
    }
}