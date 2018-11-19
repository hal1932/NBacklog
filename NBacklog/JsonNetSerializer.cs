using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System.IO;

namespace NBacklog
{
    internal class JsonNetSerializer : ISerializer, IDeserializer
    {
        public string ContentType { get; set; } = "application/json";
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public JsonNetSerializer()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        public T Deserialize<T>(IRestResponse response)
        {
            var content = response.Content;

            using (var text = new StringReader(content))
            using (var jsonTextReader = new JsonTextReader(text))
            {
                return _serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public string Serialize(object obj)
        {
            using (var text = new StringWriter())
            using (var writer = new JsonTextWriter(text))
            {
                _serializer.Serialize(writer, obj);
                return text.ToString();
            }
        }

        private Newtonsoft.Json.JsonSerializer _serializer;
    }
}
