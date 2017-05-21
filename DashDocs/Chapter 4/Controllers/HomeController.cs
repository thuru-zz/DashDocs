using DashDocs.ControllerBase;
using DashDocs.Helpers;
using DashDocs.Models;
using DashDocs.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DashDocs.Controllers
{
    [Authorize]
    public class HomeController : DashDocsControllerBase
    {
        public ActionResult Index()
        {
            var customerId = DashDocsClaims.CustomerId;

            var dbContext = new DashDocsContext();
            var documents = from document in dbContext.Documents
                            join user in dbContext.Users on document.OwnerId equals user.Id
                            where user.CustomerId == DashDocsClaims.CustomerId
                            orderby document.CreatedOn descending
                            select document;

            return View(documents.Include(d => d.Owner).Take(10).ToList());
        }

        public async Task<ActionResult> Upload(HttpPostedFileBase document)
        {
            var blobStorageService = new BlobStorageService();
            var documentId = Guid.NewGuid();

            var path = await blobStorageService.UploadDocument(document, DashDocsClaims.CustomerId, documentId);

            var dbContext = new DashDocsContext();
            dbContext.Documents.Add(new Document
            {
                Id = documentId,
                DocumentName = Path.GetFileName(document.FileName).ToLower(),
                OwnerId = DashDocsClaims.UserId,
                CreatedOn = DateTime.UtcNow,
                BlobPath = path
            });
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<FileResult> Download(Guid documentId)
        {
            var dbContext = new DashDocsContext();
            var document = await dbContext.Documents.SingleAsync(d => d.Id == documentId);

            var blobStorageService = new BlobStorageService();
            var content = await blobStorageService.DownloadDocument(documentId, DashDocsClaims.CustomerId);

            return File(content.Item1, System.Net.Mime.MediaTypeNames.Application.Octet, content.Item2);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "DashDocs Application";

            return View();
        }
    }
}