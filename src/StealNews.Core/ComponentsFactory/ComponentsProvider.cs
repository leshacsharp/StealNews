using System;

namespace StealNews.Core.ComponentsFactory
{
    public class ComponentsProvider
    {
        public static IComponentsFactory CreateComponentsFactory(string sourceTitle)
        {
            if(sourceTitle == null)
            {
                throw new ArgumentNullException(nameof(sourceTitle));
            }

            return sourceTitle switch
            {
                "belta.by" => new BeltaComponentsFactory(),
                _    => null
            };
        }
    }
}
