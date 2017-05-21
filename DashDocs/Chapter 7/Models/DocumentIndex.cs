using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashDocs.Models
{
    public class DocumentIndex
    {
        public Guid DocumentId { get; set; }
        public Guid CustomerId { get; set; }
        public string DocumentName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}