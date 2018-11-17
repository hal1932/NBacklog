using System.Threading.Tasks;

namespace NBacklog.OAuth2
{
    public abstract class OAuth2Client
    {
        public abstract Task AuthorizeAsync(OAuth2App app);

        protected async Task AuthorizeAsync(OAuth2App app, OAuth2EndPoint endPoint)
        {
            _app = app;
            _endPoint = endPoint;
            _credentials = await OAuth2Broker.AuthorizeAsync(app, endPoint).ConfigureAwait(false);
        }

        protected async Task<string> GetAccessTokenAsync()
        {
            await UpdateCredentialsAsync();
            return _credentials.AccessToken;
        }

        protected async Task UpdateCredentialsAsync()
        {
            _credentials = await OAuth2Broker.UpdateCredentialsAsync(_credentials, _app, _endPoint).ConfigureAwait(false);
        }

        OAuth2App _app;
        OAuth2EndPoint _endPoint;
        private OAuth2Credentials _credentials;
    }
}
