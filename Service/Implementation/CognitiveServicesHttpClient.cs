using System.Net.Http;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class CognitiveServicesHttpClient
    {
        public static async Task<HttpResponseMessage> HttpResponseMessage(HttpContent content, string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.PostAsync(uri, content);
            }
        }
    }
}