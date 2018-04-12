using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Funcs
{
    public class CognitiveServicesHttpClient
    {
        public static async Task<HttpResponseMessage> PostVisionRequest(HttpContent content, string parameters)
        {
            var cogKey = ConfigurationManager.AppSettings["CognitiveVisionKey"];

            var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"];

            var uri = visionUri + parameters;

            return await HttpResponseMessage(content, uri, cogKey);
        }

        public static async Task<HttpResponseMessage> PostFaceRequest(HttpContent content, string parameters)
        {
            var cogKey = ConfigurationManager.AppSettings["CognitiveFacesKey"];

            var visionUri = ConfigurationManager.AppSettings["CognitiveFacesUri"];

            var uri = visionUri + parameters;

            return await HttpResponseMessage(content, uri, cogKey);
        }

        private static async Task<HttpResponseMessage> HttpResponseMessage(HttpContent content, string uri, string cogKey)
        {
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.PostAsync(uri, content);
            }
        }
    }
}