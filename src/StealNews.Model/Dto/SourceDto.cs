using StealNews.Model.Entities;

namespace StealNews.Model.Dto
{
    public class SourceDto
    {
        public string SiteTitle { get; set; }

        public string SiteUrl { get; set; }

        public string SiteLogo { get; set; }

        public int Count { get; set; }
    }
}
