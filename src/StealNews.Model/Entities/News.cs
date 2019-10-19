using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace StealNews.Model.Entities
{
    public class News : IMongoEntity
    {
        public ObjectId Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Text { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MainImage { get; set; }

        public IEnumerable<string> Images { get; set; }

        public Category Category { get; set; }

        public Source Source { get; set; }

        public override bool Equals(object obj)
        {
            var news = obj as News;

            if(news == null)
                return false;

            return news.Url == this.Url;   
        }
    }
}
