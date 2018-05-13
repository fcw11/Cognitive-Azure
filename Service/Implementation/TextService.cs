using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;

namespace Services.Implementation
{
    public class TextService : ITextService
    {
        public IConfiguration Configuration { get; set; }

        public TextService(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void GetScore(string text)
        {
            var url = Configuration["TextAnalyticsAPI"];
            var key = Configuration["TextAnalyticsKey"];

            var something = $"{{ \"documents\": [ {{ \"language\": \"en\", \"id\": \"1\", \"text\": \"{text}\"}}]}}";

            var byteData = Encoding.UTF8.GetBytes(something);

            using (var content = new ByteArrayContent(byteData))
            {
                var response = CognitiveServicesHttpClient.HttpResponseMessage(content, url, key).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = response.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}