using DashDocs.ViewModels;
using Microsoft.PowerBI.Api.V1;
using Microsoft.PowerBI.Security;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DashDocs.Services
{
    public class PowerBIService
    {
        private IPowerBIClient _powerBiClient;
        private string _accessKey;

        public PowerBIService()
        {
            _accessKey = ConfigurationManager.AppSettings["PowerBI:AccessKey"].ToString();
            var powerBiUrl = ConfigurationManager.AppSettings["PowerBI:ApiUrl"].ToString();

            var credentials = new TokenCredentials(_accessKey, "AppKey");

            _powerBiClient = new PowerBIClient(credentials);
            _powerBiClient.BaseUri = new Uri(powerBiUrl);
        }

        public async Task<ReportViewModel> GetDocumentDownloadReportAsync()
        {
            var workspaceCollection = ConfigurationManager.AppSettings["PowerBI:WorkspaceCollection"].ToString();
            var workspaceId = ConfigurationManager.AppSettings["PowerBI:Workspace"].ToString();
            var reportId = ConfigurationManager.AppSettings["PowerBI:ReportId"].ToString();

            var reportsResponse = await _powerBiClient.Reports.GetReportsAsync(workspaceCollection, workspaceId);
            var report = reportsResponse.Value.FirstOrDefault(r => r.Id == reportId);
            var embedToken = PowerBIToken.CreateReportEmbedToken(workspaceCollection, workspaceId, reportId);

            

            var viewModel = new ReportViewModel
            {
                Report = report,
                AccessToken = embedToken.Generate(_accessKey)
            };

            return viewModel;
        }

    }
}