using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Funcs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Entities.JSON;
using Services.Entities.JSON.Handwriting;

namespace Functions.Functions
{
    public static class RetrieveHandwritingResult
    {
        [FunctionName("RetrieveHandwritingResult")]
        public static async Task Run(
            [QueueTrigger("Handwriting")] string item,
            [Table("images")] CloudTable cloudTable,
            TraceWriter log)
        {
            log.Info("Start");

            var handwritingRequest = JSONHelper.FromJson<HandwritingRequest>(item);

            var cogKey = ConfigurationManager.AppSettings["CognitiveVisionKey"];

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

            var i = 0;
            var succeeded = false;
            do
            {
                System.Threading.Thread.Sleep(1000);

                var response = await client.GetAsync(handwritingRequest.OperationLocation);

                var contentString = await response.Content.ReadAsStringAsync();

                var result = JSONHelper.FromJson<Handwriting>(contentString);

                succeeded = result.Status == "Succeeded";

                log.Info(result.Status);
                
                if (succeeded)
                {
                    await cloudTable.Update(handwritingRequest.Key, contentString, (image, text) =>
                    {
                        image.Handwriting = text;
                    });
                }

                i++;
            }
            while (i < 5 && !succeeded);

            log.Info("Finish");
        }
    }
}
