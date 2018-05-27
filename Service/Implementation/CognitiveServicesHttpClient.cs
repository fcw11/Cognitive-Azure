using System;
using System.IO;
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

        public static async Task<HttpResponseMessage> HttpResponseMessage(string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.GetAsync(uri);
            }
        }

        public static async Task<HttpResponseMessage> HttpResponseMessage(Stream audioStream, string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.PostAsync(uri,
                    new MultipartFormDataContent
                    {
                        {
                            new StreamContent(audioStream),
                            "Data",
                            "testFile_" + DateTime.Now.ToString("u")
                        }
                    });
            }
        }
    }
}