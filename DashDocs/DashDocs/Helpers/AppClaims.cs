using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashDocs.Helpers
{
    internal class AppClaims
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
    }
}