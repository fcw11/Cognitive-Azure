using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Services.Entities.JSON.Handwriting;

namespace Funcs.Functions
{
    public static class RetrieveHandwritingResult
    {
        [FunctionName("RetrieveHandwritingResult")]
        public static async Task Run(
            [QueueTrigger("Handwriting")] string item,
         //   [Table("images")] CloudTable cloudTable,
            TraceWriter log)
        {
            log.Info("Start");

            var cogKey = ConfigurationManager.AppSettings["CognitiveService"];

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

            string contentString;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(1000);

                var response = await client.GetAsync(item);

                contentString = await response.Content.ReadAsStringAsync();

                var a = Services.Entities.JSON.JSONHelper.FromJson<Handwriting>(contentString);

                log.Info($"{i} {contentString}");

                i++;
            }
            while (i < 5 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

            log.Info("Finish");
        }
    }
}