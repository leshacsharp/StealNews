using System;

namespace StealNews.Common.Helpers
{
    public static class DateHepler
    {
        private static string[] _months = new[] { "янв", "фев", "март", "апр", "ма", "июн", "июл", "авг", "сент", "окт", "ноя", "дек" };

        public static int GetMonthByName(string mounth)
        {
            if(mounth == null)
            {
                throw new ArgumentNullException(nameof(mounth));
            }

            var parsedMonth = mounth.Replace(" ", string.Empty).ToLower();
            var numberOfMounth = 0;

            for (int i = 0; i < _months.Length; i++)
            {
                if(parsedMonth.IndexOf(_months[i]) > 0)
                {
                    numberOfMounth = i + 1;
                    break;
                }
            }

            if(numberOfMounth == 0)
            {
                throw new ArgumentException("The mounth title is invalid", nameof(mounth));
            }

            return numberOfMounth;
        }
    }
}
