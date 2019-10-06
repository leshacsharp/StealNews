using System.Collections.Generic;

namespace StealNews.Model.Models.Parser
{
    public class Images
    {
        public string MainImage { get; set; }

        public IEnumerable<string> AdditionalImages { get; set; }
    }
}
