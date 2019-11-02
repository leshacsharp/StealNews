using System.Collections.Generic;

namespace StealNews.Core.Settings
{
    public class SourceConfiguration
    {
        public IEnumerable<Source> Sources { get; set; }

        public int CountGeneratedNewsFor1Time { get; set; }

        public int MaxScaningNewsIfLastNewsNotFound { get; set; }
    }
}
