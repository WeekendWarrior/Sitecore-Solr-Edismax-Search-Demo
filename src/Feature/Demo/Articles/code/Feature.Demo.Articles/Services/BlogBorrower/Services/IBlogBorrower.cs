using Feature.Demo.Articles.Services.BlogBorrower.Models;
using System.Collections.Generic;

namespace Feature.Demo.Articles.Services.BlogBorrower.Services
{
    public interface IBlogBorrower
    {
        IEnumerable<BlogFeedItem> GetAll();
    }
}
