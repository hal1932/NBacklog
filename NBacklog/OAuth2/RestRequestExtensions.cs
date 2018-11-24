using RestSharp;

namespace NBacklog.OAuth2
{
    internal static class RestRequestExtensions
    {
        public static void SetOAuth2AccessToken(this RestRequest request, string accessToken)
        {
            request.AddHeader("Authorization", $"Bearer {accessToken}");
        }
    }
}
