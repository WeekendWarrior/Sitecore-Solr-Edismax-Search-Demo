using Feature.Demo.Articles.Models;
using Feature.Demo.Articles.Services.BlogBorrower.Models;
using Feature.Demo.Articles.Services.BlogBorrower.Services;
using Feature.Demo.Articles.Services.BlogWriter.Services;
using Sitecore.Mvc.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data.Items;

namespace Feature.Demo.Articles.Controllers
{
    /// <summary>
    /// Sitecore Controller that steals ("borrows") blogs from Sitecore's official blog feed.
    /// </summary>
    public class BlogFeedBorrowerController : SitecoreController
    {
        protected IBlogBorrower _blogBorrower;

        protected IBlogWriter _blogWriter;

        public BlogFeedBorrowerController()
        {
            // TODO: implement DI
            _blogBorrower = new SitecoreFeedBlogBorrower();
            _blogWriter = new SitecoreBlogWriter();
        }

        /// <summary>
        /// Returns the BlogFeedBorrower Rendering.
        /// </summary>
        public ActionResult BlogFeedBorrower(bool? update = false)
        {
            // Build the view model.
            var model = new BlogFeedBorrowerViewModel
            {
                FeedItems = _blogBorrower.GetAll().ToArray()
            };
            
            if (update.HasValue && update.Value)
            {
                // TODO: Write blog feed items as sitecore items.
                model.SitecoreItems = _blogWriter.WriteBlogs(model.FeedItems).ToArray();
            }

            return View(Views.BlogFeedBorrower, model);
        }

        /// <summary>
        /// List of views used by the <see cref="BlogFeedBorrowerController"/> class.
        /// </summary>
        public static class Views
        {
            public const string BlogFeedBorrower = "~/Views/Demo/Articles/BlogFeedBorrower.cshtml";
        }
    }
}