using System.Collections.Generic;

namespace StealNews.Core.Parser.Models
{
    public class Images
    {
        public string MainImage { get; set; }

        public IEnumerable<string> AdditionalImages { get; set; }
    }
}
