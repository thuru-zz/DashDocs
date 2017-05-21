using DashDocs.Models;
using DashDocs.Services;
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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");

            var dbContext = new DashDocsContext();
            var documents = from document in dbContext.Documents
                            join user in dbContext.Users on document.OwnerId equals user.Id
                            where user.CustomerId == customerId
                            orderby document.CreatedOn descending
                            select document;

            return View(documents.Include(d => d.Owner).Take(10).ToList());
        }

        public async Task<ActionResult> Upload(HttpPostedFileBase document)
        {
            // Ids used in the seed method
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");
            Guid userId = Guid.Parse("2A37108E-56AF-4C18-99C4-415191591CD9");

            var blobStorageService = new BlobStorageService();
            var documentId = Guid.NewGuid();

            var path = await blobStorageService.UploadDocument(document, customerId, documentId);

            var dbContext = new DashDocsContext();
            dbContext.Documents.Add(new Document
            {
                Id = documentId,
                DocumentName = Path.GetFileName(document.FileName).ToLower(),
                OwnerId = userId,
                CreatedOn = DateTime.UtcNow,
                BlobPath = path
            });
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<FileResult> Download(Guid documentId)
        {
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");

            var dbContext = new DashDocsContext();
            var document = await dbContext.Documents.SingleAsync(d => d.Id == documentId);

            var blobStorageService = new BlobStorageService();
            var content = await blobStorageService.DownloadDocument(documentId, customerId);

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