using DashDocs.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DashDocs.Services
{
    public class DocumentDbService
    {
        private static readonly DocumentClient _documentClient;

        private static readonly string _database = "DashDocsComments";
        private static readonly string _collection = "comments";

        static DocumentDbService()
        {
            var uri = ConfigurationManager.AppSettings["DocumentDb:Uri"].ToString();
            var key = ConfigurationManager.AppSettings["DocumentDb:Key"].ToString();
            _documentClient = new DocumentClient(new Uri(uri), key);
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            comment.CommentDateTime = GetCommentDateTime();
            await _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_database, _collection), comment);
        }

        public async Task<List<Comment>> GetCommentsAsync(Guid documentId, Guid customerId)
        {
            var query = _documentClient.CreateDocumentQuery<Comment>(UriFactory.CreateDocumentCollectionUri(_database, _collection))
                .Where(c => c.DocuemtnId == documentId && c.CustomerId == customerId).OrderByDescending(c => c.CommentDateTime.Epoch).AsDocumentQuery();

            var comments = new List<Comment>();
            while (query.HasMoreResults)
            {
                comments.AddRange(await query.ExecuteNextAsync<Comment>());
            }
            return comments;
        }

        private CommentDateTime GetCommentDateTime()
        {
            var datetime = DateTime.UtcNow;
            return new CommentDateTime
            {
                DateStamp = datetime,
                Epoch = (int)((datetime - new DateTime(1987, 8, 8)).TotalSeconds)
            };
        }
    }

    
}