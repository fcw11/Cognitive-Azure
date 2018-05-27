using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Functions
{
    public class CognitiveServicesHttpClient : Services.Implementation.CognitiveServicesHttpClient
    {
        public static async Task<HttpResponseMessage> PostVisionRequest(HttpContent content, string parameters)
        {
            var cogKey = ConfigurationManager.AppSettings["CognitiveVisionKey"];

            var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"];

            var uri = visionUri + parameters;

            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

            return await HttpPost(content, uri, cogKey);
        }

        public static async Task<HttpResponseMessage> PostFaceRequest(HttpContent content, string parameters)
        {
            var cogKey = ConfigurationManager.AppSettings["CognitiveFacesKey"];

            var visionUri = ConfigurationManager.AppSettings["CognitiveFacesUri"];

            var uri = visionUri + parameters;

            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

            return await HttpPost(content, uri, cogKey);
        }
    }
}