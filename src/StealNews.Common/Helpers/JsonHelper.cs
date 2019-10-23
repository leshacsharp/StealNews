using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StealNews.Common.Helpers
{
    public class JsonHelper
    {
        public async static Task<T> GetAsync<T>(string propertyName, string json)
        {
            if(propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while(await jsonReader.ReadAsync())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName && ((string)jsonReader.Value).Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        await jsonReader.ReadAsync();

                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }
            }

            return default;
        }
    }
}
