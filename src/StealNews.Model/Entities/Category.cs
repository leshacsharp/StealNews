using System.Collections.Generic;

namespace StealNews.Model.Entities
{
    public class Category
    {
        public string Title { get; set; }

        public string Image { get; set; }

        public IEnumerable<string> SubCategories { get; set; }
    }
}
