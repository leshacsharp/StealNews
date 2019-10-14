using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace StealNews.Model.Models.Service.News
{
    public class NewsFindFilter
    {
        public string KeyWord { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public IEnumerable<string> Sources { get; set; }

        public IEnumerable<string> MainCategories { get; set; }

        public IEnumerable<string> SubCategories { get; set; }

        public int Count { get; set; }

        public int Skip { get; set; }
    }
}
