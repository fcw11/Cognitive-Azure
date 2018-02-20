using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System;
using System.Text;

public static void Run(Stream myBlob, string name, TraceWriter log)
{
    log.Info("Start");
    MakeRequest(myBlob, log);
    log.Info("Finish");
}

static async void MakeRequest(Stream myBlob, TraceWriter log)
{
    string cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    var client = new HttpClient();

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

    using (HttpContent content = new StreamContent(myBlob))
    {
        var uri = "https://northeurope.api.cognitive.microsoft.com/vision/v1.0/describe?maxCandidates=1";

        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var response = client.PostAsync(uri, content).Result;

        var answer = await response.Content.ReadAsStringAsync();

        log.Info("Hello World");

        log.Info(answer);
    }
}