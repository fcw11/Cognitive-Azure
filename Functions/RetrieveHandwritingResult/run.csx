#r "Microsoft.WindowsAzure.Storage"

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task Run(string item, CloudTable inputTable, ICollector<string> queueItem, TraceWriter log)
{
    log.Info($"C# Queue trigger function processed: {item}");

    string cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    var client = new HttpClient();

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

    string contentString;
    int i = 0;
    do
    {
        System.Threading.Thread.Sleep(1000);

        var response = await client.GetAsync(item);

        contentString = await response.Content.ReadAsStringAsync();

        log.Info($"{i} {contentString}");

        i++;
    }
    while (i < 5 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

    log.Info(contentString);
}
