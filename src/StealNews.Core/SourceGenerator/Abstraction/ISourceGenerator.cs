﻿using StealNews.Core.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.Core.SourceGenerator.Abstraction
{
    public interface ISourceGenerator
    {
        Task<IEnumerable<string>> GenerateAsync(Source source, int count, int skip = 0);
    }
}
