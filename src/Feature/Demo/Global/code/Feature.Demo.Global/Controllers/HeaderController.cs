using Sitecore.Mvc.Controllers;
using System.Web.Mvc;

namespace Feature.Demo.Global.Controllers
{
    public class HeaderController : SitecoreController
    {
        /// <summary>
        /// Returns the header rendering.
        /// </summary>
        public ActionResult Header()
        {
            return View(Views.Header);
        }

        /// <summary>
        /// List of views used by the <see cref="HeaderController"/> class.
        /// </summary>
        public static class Views
        {
            public const string Header = "~/Views/Demo/Global/Header.cshtml";
        }
    }
}