using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DashDocs.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }

        public string DocumentName { get; set; }

        public DateTime CreatedOn { get; set; }

        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }

        public string BlobPath { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }

    }
}