using System.Collections.Generic;

namespace StealNews.Model.Models.Service.News
{
    public class PartOfNews
    {
        public IEnumerable<Entities.News> News { get; set; }

        public bool IsPageHaveLastNews { get; set; }
    }
}
