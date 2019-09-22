using System.Collections.Generic;

namespace StealNews.Model.Entities
{
    public class Category
    {
        public string Title { get; set; }

        public IEnumerable<Category> SubCategories { get; set; }
    }
}
