using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashDocs.Models
{
    public class SignInLog : TableEntity
    {
        public SignInLog()
        {
        }

        public Guid UserId { get; set; }
        public string IP { get; set; }
        public bool IsSuccess { get; set; }
        public string ExceptionMessage { get; set; }
    }
}