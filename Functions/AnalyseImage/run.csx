#r "Microsoft.WindowsAzure.Storage"
#load "..\Model\Image.csx"

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

public static void Run(Stream myBlob, string name, CloudTable inputTable, TraceWriter log)
{
    log.Info("Start");
    MakeRequest(myBlob, inputTable, name, log);
    log.Info("Finish");
}

static async void MakeRequest(Stream myBlob, CloudTable inputTable, string name, TraceWriter log)
{
    string cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    var client = new HttpClient();

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

    string answer = string.Empty;

    using (HttpContent content = new StreamContent(myBlob))
    {
        var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"];

        var uri = visionUri + "describe?maxCandidates=1";

        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var response = client.PostAsync(uri, content).Result;

        answer = await response.Content.ReadAsStringAsync();
    }

    var tableQuery = new TableQuery<Image>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));

    var tableQueryResult = inputTable.ExecuteQuerySegmentedAsync(tableQuery, null).Result;

    var image = tableQueryResult.Results.Single();

    image.Description = answer;

    var updateOperation = TableOperation.Replace(image);

    inputTable.Execute(updateOperation); ;
}
