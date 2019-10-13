using StealNews.Model.Dto;
using System.Collections.Generic;

namespace StealNews.Model.Models.Service.Configuration
{
    public class AppConfiguration
    {
        public IEnumerable<CategoryDto> Categories{ get; set; }

        public IEnumerable<SourceDto> Sources { get; set; }
    }
}
