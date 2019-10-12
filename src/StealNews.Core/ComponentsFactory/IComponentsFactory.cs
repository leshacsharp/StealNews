using StealNews.Core.Parser.Abstraction;
using StealNews.Core.SourceGenerator.Abstraction;
using StealNews.Core.SourceValidators.Abstraction;

namespace StealNews.Core.ComponentsFactory
{
    public interface IComponentsFactory
    {
        ISourceGenerator CreateSourceGenertor();
        ISourceValidator CreateSourceValidator();
        INewsParser CreateNewsParser();
    }
}
