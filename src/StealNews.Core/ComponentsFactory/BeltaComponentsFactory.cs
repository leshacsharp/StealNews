using StealNews.Core.Parser.Abstraction;
using StealNews.Core.Parser.Implementation;
using StealNews.Core.SourceGenerator.Abstraction;
using StealNews.Core.SourceGenerator.Implementation;

namespace StealNews.Core.ComponentsFactory
{
    public class BeltaComponentsFactory : IComponentsFactory
    {
        public ISourceGenerator CreateSourceGenertor()
        {
            return new BeltaSourceGenerator();
        }

        public INewsParser CreateNewsParser()
        {
            return new BeltaParser();
        }
    }
}
