using StealNews.Model.Entities;
using System.Collections.Generic;

namespace StealNews.Model.Models.Service.Configuration
{
    public class AppConfiguration
    {
        public IEnumerable<Category> Categories{ get; set; }

        public IEnumerable<Source> Sources { get; set; }
    }
}
