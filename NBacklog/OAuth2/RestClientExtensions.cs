using RestSharp;

namespace NBacklog.OAuth2
{
    internal static class RestClientExtensions
    {
        public static void SetJsonNetSerializer(this RestClient client, JsonNetSerializer serializer = null)
        {
            serializer = serializer ?? new JsonNetSerializer();
            client.AddHandler("application/json", serializer);
            client.AddHandler("text/json", serializer);
            client.AddHandler("text/x-json", serializer);
            client.AddHandler("text/javascript", serializer);
            client.AddHandler("*+json", serializer);
        }

        public static void SetJsonNetSerializer(this RestRequest request, JsonNetSerializer serializer = null)
        {
            request.JsonSerializer = serializer ?? new JsonNetSerializer();
        }
    }
}
