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

            IComponentsFactory componentsFactory = null;

            switch(sourceTitle)
            {
                case "belta.by": componentsFactory = new BeltaComponentsFactory(); break;
                case "bbc.com": componentsFactory = new BBCComponentFactory(); break;
            }

            return componentsFactory;
        }
    }
}
