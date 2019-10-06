﻿using StealNews.Model.Entities;
using System.Collections.Generic;

namespace StealNews.Model.Models.Service
{
    public class PartOfNews
    {
        public IEnumerable<News> News { get; set; }

        public bool IsPageHaveLastNews { get; set; }
    }
}
