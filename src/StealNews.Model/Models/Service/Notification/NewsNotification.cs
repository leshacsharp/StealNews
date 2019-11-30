using System;
using System.Linq;

namespace StealNews.Model.Models.Service.Notification
{
    public class NewsNotification
    {
        public string CategoryTitle { get; set; }
        public string CategoryImage { get; set; }
        public int CountNews { get; set; }
        public string CategoryCode
        {
            get
            {
                if (string.IsNullOrEmpty(CategoryTitle))
                {
                    throw new ArgumentNullException(nameof(CategoryTitle), $"Cannot create {nameof(CategoryCode)} because {nameof(CategoryTitle)} is empty");
                }

                var ASCICodes = CategoryTitle.ToCharArray().Select(l => (int)l);
                return string.Join("-", ASCICodes);
            }
        }
    }
}
