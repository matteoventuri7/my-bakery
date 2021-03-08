using System.Net.Http;

namespace Bakery.Client
{
    public class NoAuthHttpClient
    {
        public readonly HttpClient Client;

        public NoAuthHttpClient(HttpClient httpClient)
        {
            this.Client = httpClient;
        }

        public static implicit operator HttpClient(NoAuthHttpClient x) => x.Client;
    }
}
