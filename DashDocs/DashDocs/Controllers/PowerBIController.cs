using DashDocs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DashDocs.Controllers
{
    [Authorize]
    public class PowerBIController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var poweBIService = new PowerBIService();
            var viewModel = await poweBIService.GetDocumentDownloadReportAsync();

            return View(viewModel);
        }
    }
}