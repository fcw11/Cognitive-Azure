using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class CognitiveServicesHttpClient
    {
        public static async Task<HttpResponseMessage> HttpGet(string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.GetAsync(uri);
            }
        }

        public static async Task<HttpResponseMessage> HttpPut(HttpContent content, string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.PutAsync(uri, content);
            }
        }

        public static async Task<HttpResponseMessage> HttpPost(HttpContent content, string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.PostAsync(uri, content);
            }
        }

        public static async Task<HttpResponseMessage> HttpPostImage(HttpContent content, string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

                return await client.PostAsync(uri, content);
            }
        }

        public static async Task<HttpResponseMessage> HttpPostAudio(Stream stream, string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.PostAsync(uri,
                    new MultipartFormDataContent
                    {
                        {
                            new StreamContent(stream),
                            "Data",
                            "testFile_" + DateTime.Now.ToString("u")
                        }
                    });
            }
        }

        public static async Task<HttpResponseMessage> HttpDelete(string uri, string cogKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

                return await client.DeleteAsync(uri);
            }
        }
    }
}