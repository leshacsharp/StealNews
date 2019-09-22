using StealNews.Core.Parser.Implementation;
using System;

namespace StealNews.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new BeltaParser();
            var news = parser.ParseAsync("https://www.belta.by/economics/view/maz-i-aksiom-grupp-vypustili-samogo-moschnogo-v-sng-zheleznogo-drovoseka-362840-2019/").GetAwaiter().GetResult();
        }
    }
}
