using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System;
using System.Text;

public static void Run(Stream myBlob, string name, TraceWriter log)
{
        log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        MakeRequest(myBlob, log);
}

static async void MakeRequest(Stream myBlob, TraceWriter log)
{
    string cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    var client = new HttpClient();

    // Request headers
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

    // Request parameters
    HttpResponseMessage response;

    using (HttpContent content = new StreamContent(myBlob))
    {
        var uri = "https://northeurope.api.cognitive.microsoft.com/vision/v1.0/describe?maxCandidates=1";

        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
        response = client.PostAsync(uri, content).Result;
    } 

    var answer = await response.Content.ReadAsStringAsync();
    log.Info("Hello World");
    log.Info(answer);
}

