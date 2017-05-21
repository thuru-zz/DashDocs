using DashDocs.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;

namespace DashDocs.Services
{
    public class SearchService
    {
        private static ISearchIndexClient _searchIndexClient;

        static SearchService()
        {
            var searchServiceName = "dashdocs";
            var indexName = "documentindex";
            var searchApiKey = ConfigurationManager.AppSettings["Search:ServiveKey"].ToString();

            _searchIndexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(searchApiKey));
        }

        public async Task<List<DocumentIndex>> SearchAsync(string searchTerm, Guid customerId)
        {
            var parameters = new SearchParameters
            { 
                Filter = $"CustomerId eq '{customerId.ToString().ToLower()}'",
                OrderBy = new[] { "CreatedOn desc"}
            };

            var searchResult = await _searchIndexClient.Documents.SearchAsync<DocumentIndex>(searchTerm.ToLower(), parameters);

            return new List<DocumentIndex>(searchResult.Results.Select(s => s.Document));
        }
    }
}