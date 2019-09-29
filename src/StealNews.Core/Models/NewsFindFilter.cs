using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace StealNews.Core.Models
{
    public class NewsFindFilter
    {
        public string KeyWord { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public IEnumerable<string> Sources { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public ObjectId? AfterId { get; set; }
    }
}
