using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DashDocs.Models
{
    public class DocumentDownload
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }

        public DateTime DownloadedOn { get; set; }

        public Guid CustomerId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
    }
}