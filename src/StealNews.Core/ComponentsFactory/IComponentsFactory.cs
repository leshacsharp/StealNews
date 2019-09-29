using StealNews.Core.Parser.Abstraction;
using StealNews.Core.SourceGenerator.Abstraction;

namespace StealNews.Core.ComponentsFactory
{
    public interface IComponentsFactory
    {
        ISourceGenerator CreateSourceGenertor();
        INewsParser CreateNewsParser();
    }
}
