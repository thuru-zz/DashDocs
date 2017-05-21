using DashDocs.ControllerBase;
using DashDocs.Models;
using DashDocs.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DashDocs.Controllers
{
    [Authorize]
    public class DocumentController : DashDocsControllerBase
    {
        public async Task<ActionResult> Index()
        {
            Guid documentId = Guid.Empty;

            if (Request.QueryString["documentId"] != null && Guid.TryParse(Request.QueryString["documentId"], out documentId))
            {
                var dbContext = new DashDocsContext();
                var document = dbContext.Documents.Single(d => d.Id == documentId);

                var docucmentDbContext = new DocumentDbService();
                var comments = await docucmentDbContext.GetCommentsAsync(documentId, DashDocsClaims.CustomerId);

                var result = new KeyValuePair<Document, List<Comment>>(document, comments);
                return View(result);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Comment(string content, Guid documentId)
        {
            var comment = new Comment
            {
                Content = content,
                Author = DashDocsClaims.DisplayName,
                CustomerId = DashDocsClaims.CustomerId,
                DocuemtnId = documentId,
                Id = Guid.NewGuid().ToString()
            };
            var docucmentDbContext = new DocumentDbService();
            await docucmentDbContext.CreateCommentAsync(comment);

            return RedirectToAction("Index", new { documentId = documentId });
        }
    }
}