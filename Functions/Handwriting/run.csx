#r "Microsoft.WindowsAzure.Storage"
#load "..\Model\Image.csx"

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task Run(Stream myBlob, string name, ICollector<string> queueItem, TraceWriter log)
{
    log.Info("Start");
    await MakeRequest(myBlob, name, queueItem, log);
    log.Info("Finish");
}

static async Task MakeRequest(Stream myBlob, string name, ICollector<string> queueItem, TraceWriter log)
{
    string cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    string answer = string.Empty;

    using (HttpContent content = new StreamContent(myBlob))
    {
        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"] + "recognizeText?handwriting=true";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

            var response = await client.PostAsync(visionUri, content);

            answer = response.Headers.GetValues("Operation-Location").FirstOrDefault();

            log.Info(answer);

            queueItem.Add(answer);
        }
    }
}