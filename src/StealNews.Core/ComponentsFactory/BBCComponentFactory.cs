using StealNews.Core.Parser.Abstraction;
using StealNews.Core.Parser.Implementation;
using StealNews.Core.SourceGenerator.Abstraction;
using StealNews.Core.SourceGenerator.Implementation;
using StealNews.Core.SourceValidators.Abstraction;
using StealNews.Core.SourceValidators.Implementation;

namespace StealNews.Core.ComponentsFactory
{
    public class BBCComponentFactory : IComponentsFactory
    {
        public ISourceGenerator CreateSourceGenertor()
        {
            return new BBCSourceGenerator();
        }

        public ISourceValidator CreateSourceValidator()
        {
            return new BBCSourceValidator();
        }

        public INewsParser CreateNewsParser()
        {
            return new BBCParser();
        }
    }
}
