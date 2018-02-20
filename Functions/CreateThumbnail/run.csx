using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System;
using System.Text;

public static void Run(Stream input, Stream output, string name, TraceWriter log)
{
    log.Info("Start");

    string cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    var client = new HttpClient();

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

    using (HttpContent content = new StreamContent(input))
    {
        string requestParameters = "width=200&height=150&smartCropping=true";

        var uri = "https://northeurope.api.cognitive.microsoft.com/vision/v1.0/generateThumbnail?" + requestParameters;

        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var response = client.PostAsync(uri, content).Result;

        var responseBytes = response.Content.ReadAsByteArrayAsync().Result;

        if (response.IsSuccessStatusCode)
        {
            output.Write(responseBytes, 0, responseBytes.Length);
        }
    }

    log.Info("Finish");
}