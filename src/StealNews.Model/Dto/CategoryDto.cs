using System.Collections.Generic;

namespace StealNews.Model.Dto
{
    public class CategoryDto
    {
        public string Title { get; set; }

        public string Image { get; set; }

        public IEnumerable<string> SubCategories { get; set; }

        public int Count { get; set; }
    }
}
