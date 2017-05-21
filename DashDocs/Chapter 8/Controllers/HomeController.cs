using DashDocs.ControllerBase;
using DashDocs.Helpers;
using DashDocs.Models;
using DashDocs.Services;
using DashDocs.ViewModels;
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
        public async Task<ActionResult> Index()
        {
            var redisService = new RedisService();
            var collection = await redisService.GetRecentDocumentsForCustomerAsync(DashDocsClaims.CustomerId);

            return View(collection);
        }

        public async Task<ActionResult> Upload(HttpPostedFileBase document)
        {
            var blobStorageService = new BlobStorageService();
            var documentId = Guid.NewGuid();

            var path = await blobStorageService.UploadDocumentAsync(document, DashDocsClaims.CustomerId, documentId);

            var dbContext = new DashDocsContext();

            var documentModel = new Document
            {
                Id = documentId,
                DocumentName = Path.GetFileName(document.FileName).ToLower(),
                OwnerId = DashDocsClaims.UserId,
                CreatedOn = DateTime.UtcNow,
                BlobPath = path
            };

            dbContext.Documents.Add(documentModel);
            await dbContext.SaveChangesAsync();

            var doc = new DocumentViewModel
            {
                DocumentId = documentModel.Id,
                Owner = DashDocsClaims.DisplayName,
                CreatedOn = documentModel.CreatedOn,
                DocumentName = documentModel.DocumentName,
            };

            var redisService = new RedisService();
            await redisService.UpdateDocumentCacheAsync(DashDocsClaims.CustomerId, doc);

            return RedirectToAction("Index");
        }

        public async Task<FileResult> Download(Guid documentId)
        {
            var dbContext = new DashDocsContext();
            var document = await dbContext.Documents.SingleAsync(d => d.Id == documentId);

            var blobStorageService = new BlobStorageService();
            var content = await blobStorageService.DownloadDocumentAsync(documentId, DashDocsClaims.CustomerId);

            content.Value.Position = 0;
            return File(content.Value, System.Net.Mime.MediaTypeNames.Application.Octet, content.Key);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "DashDocs Application";

            return View();
        }
    }
}