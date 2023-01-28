using RestSharp.Authenticators;
using RestSharp;
using SharpGlesysClient.Module;

namespace SharpGlesysClient
{
    public  partial class GlesysClient
    {
        public RestClient RestClient { get; }
        public Domain Domain { get; }

        public GlesysClient(string url, string username, string apiKey)
        {
            RestClient = new RestClient(url)
            {
                Authenticator = new HttpBasicAuthenticator(username, apiKey),
                AcceptedContentTypes = new []{"application/json"}
            };

            Domain = new Domain(this);
        }

    }
}