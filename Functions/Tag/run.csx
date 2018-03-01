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

public static async Task Run(Stream myBlob, string name, CloudTable inputTable, TraceWriter log)
{
    log.Info("Start");
    await MakeRequest(myBlob, inputTable, name, log);
    log.Info("Finish");
}

static async Task MakeRequest(Stream myBlob, CloudTable inputTable, string name, TraceWriter log)
{
    string cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    string answer = string.Empty;

    using (HttpContent content = new StreamContent(myBlob))
    {
        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"] + "tag";

        var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

        var response = await client.PostAsync(visionUri, content);

        answer = await response.Content.ReadAsStringAsync();
    }

    var tableQuery = new TableQuery<Image>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));

    var tableQueryResult = await inputTable.ExecuteQuerySegmentedAsync(tableQuery, null);

    var image = tableQueryResult.Results.Single();

    image.Tag = answer;

    var updateOperation = TableOperation.Replace(image);

    await inputTable.ExecuteAsync(updateOperation); ;
}