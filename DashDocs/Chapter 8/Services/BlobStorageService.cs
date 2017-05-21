using DashDocs.Models;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace DashDocs.Services
{
    public class BlobStorageService
    {
        private const string KEY_URI = "https://dashdocsvault.vault.azure.net/keys/docencryptionkey/be79739348d649499662addcbc01740c";

        public async Task<string> UploadDocumentAsync(HttpPostedFileBase documentFile, Guid customerId, Guid documentId)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DocumentStore"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(customerId.ToString().ToLower());
            await container.CreateIfNotExistsAsync();

            var keyvaultResolver = new KeyVaultKeyResolver(GetToken);
            var key = await keyvaultResolver.ResolveKeyAsync(KEY_URI, CancellationToken.None);

            var encryptionPolicy = new BlobEncryptionPolicy(key, null);
            var requestOptions = new BlobRequestOptions { EncryptionPolicy = encryptionPolicy };

            var blobRelativePath = documentId.ToString().ToLower() + "/" + Path.GetFileName(documentFile.FileName).ToLower();

            var block = container.GetBlockBlobReference(blobRelativePath);

            await block.UploadFromStreamAsync(documentFile.InputStream, documentFile.InputStream.Length, null, requestOptions, null);

            return blobRelativePath;
        }

        public async Task<KeyValuePair<string, MemoryStream>> DownloadDocumentAsync(Guid documentId, Guid customerId)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DocumentStore"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(customerId.ToString().ToLower());

            var dbContext = new DashDocsContext();
            var document = await dbContext.Documents.SingleAsync(d => d.Id == documentId);

            var keyvaultResolver = new KeyVaultKeyResolver(GetToken);

            var encryptionPolicy = new BlobEncryptionPolicy(null, keyvaultResolver);
            var requestOptions = new BlobRequestOptions { EncryptionPolicy = encryptionPolicy };

            var block = container.GetBlockBlobReference(document.BlobPath);

            var stream = new MemoryStream();
            await block.DownloadToStreamAsync(stream, null, requestOptions, null);
            
            var content = new KeyValuePair<string, MemoryStream>(document.DocumentName, stream);

            // non blocking download data insert
            dbContext.DocumentDownloads.Add(
                new DocumentDownload
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    CustomerId = customerId,
                    DownloadedOn = DateTime.UtcNow.Date,
                    
                });
            dbContext.SaveChangesAsync();

            return content;
        }

        private async static Task<string> GetToken(string authority, string resource, string scope)
        {
            var appId = ConfigurationManager.AppSettings["KeyVault:AppId"].ToString();
            var appSecret = ConfigurationManager.AppSettings["KeyVault:AppSecret"].ToString();

            var authContext = new AuthenticationContext(authority);
            var clientCredentials = new ClientCredential(appId, appSecret);

            var result = authContext.AcquireTokenAsync(resource, clientCredentials).GetAwaiter().GetResult();
            return result.AccessToken;
        }
    }
}