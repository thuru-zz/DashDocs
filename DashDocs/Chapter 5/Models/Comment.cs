using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashDocs.Models
{
    public class Comment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "documentId")]
        public Guid DocuemtnId { get; set; }
        [JsonProperty(PropertyName = "customerId")]
        public Guid CustomerId { get; set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; } 
        [JsonProperty(PropertyName = "commentDateTime")]
        public CommentDateTime CommentDateTime { get; set; }
    }

    public class CommentDateTime
    {
        [JsonProperty(PropertyName = "dateStamp")]
        public DateTime DateStamp { get; set; }
        [JsonProperty(PropertyName = "epoch")]
        public int Epoch { get; set; }
    }
}