using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using System.Web;


namespace DashDocs.Services
{
    public class BlobStorageService
    {
        public async Task<string> UploadDocumentAsync(HttpPostedFileBase documentFile, Guid customerId, Guid documentId)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DocumentStore"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(customerId.ToString().ToLower());
            await container.CreateIfNotExistsAsync();

            var blobRelativePath = documentId.ToString().ToLower() + "/" + Path.GetFileName(documentFile.FileName).ToLower();

            var block = container.GetBlockBlobReference(blobRelativePath);

            await block.UploadFromStreamAsync(documentFile.InputStream);
            return blobRelativePath;
        }

        public async Task<KeyValuePair<string, MemoryStream>> DownloadDocumentAsync(Guid documentId, Guid customerId)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DocumentStore"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(customerId.ToString().ToLower());

            var dbContext = new DashDocsContext();
            var document = await dbContext.Documents.SingleAsync(d => d.Id == documentId);

            var block = container.GetBlockBlobReference(document.BlobPath);

            var stream = new MemoryStream();
            await block.DownloadToStreamAsync(stream);

            var content = new KeyValuePair<string, MemoryStream>(document.DocumentName, stream);

            return content;
        }
    }
}