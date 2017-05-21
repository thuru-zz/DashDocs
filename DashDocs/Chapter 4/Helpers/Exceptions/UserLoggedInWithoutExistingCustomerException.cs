using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashDocs.Helpers.Exceptions
{
    public class UserLoggedInWithoutExistingCustomerException : Exception
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}