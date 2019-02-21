using Feature.Demo.Articles.Services.BlogBorrower.Models;
using Sitecore.Data.Items;
using System.Collections.Generic;

namespace Feature.Demo.Articles.Services.BlogWriter.Services
{
    public interface IBlogWriter
    {
        IEnumerable<Item> WriteBlogs(IEnumerable<BlogFeedItem> feedItems);
    }
}
