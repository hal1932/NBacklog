using RestSharp;

namespace NBacklog.OAuth2
{
    internal static class RestRequestExtensions
    {
        public static void SetOAuth2Credentials(this RestRequest request, OAuth2Credentials credentials)
        {
            request.AddHeader("Authorization", credentials.AccessToken);
        }
    }
}
