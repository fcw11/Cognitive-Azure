using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Funcs
{
    public class CognitiveServicesHttpClient
    {
        public static async Task<HttpResponseMessage> PostRequest(HttpContent content, string parameters)
        {
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

            var cogKey = ConfigurationManager.AppSettings["CognitiveService"];

            var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"];

            var uri = visionUri + parameters;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.PostAsync(uri, content);
            }
        }
    }
}
