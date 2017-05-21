using DashDocs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DashDocs.Services
{
    public class TableStorageService
    {
        public  void CreateLog(Guid tenantId, Guid userId, string ip, bool isSuccess, string exception)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["LogStorage"].ConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference($"DashDocs{DateTime.UtcNow.Year}");
            table.CreateIfNotExists();

            SignInLog entity = new SignInLog()
            {
                PartitionKey = tenantId.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                UserId = userId,
                IP = ip,
                IsSuccess = isSuccess,
                ExceptionMessage = exception
            };

            TableOperation insertOperation = TableOperation.Insert(entity);
            table.ExecuteAsync(insertOperation);            
        }

        public List<SignInLog> GetLogsForTenant(int year, string tenantId, bool status)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["LogStorage"].ConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference($"DashDocs{year}");

            var query = from log in table.CreateQuery<SignInLog>()
                        where log.PartitionKey == tenantId && log.IsSuccess == status
                        select log;

            return query.ToList();
        }


    }
}