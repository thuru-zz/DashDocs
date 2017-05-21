using DashDocs.ControllerBase;
using DashDocs.Models;
using DashDocs.Services;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DashDocs.Controllers
{
    [Authorize]
    public class SearchController : DashDocsControllerBase
    {
        public async Task<ActionResult> Index(string search)
        {
            var documents = new List<DocumentIndex>();

            if(Request.QueryString["search"] != null)
            {
                var searchService = new SearchService();
                documents = await searchService.SearchAsync(search, DashDocsClaims.CustomerId);
            }

            return View(documents);
        }
    }
}