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
using Microsoft.WindowsAzure.Storage.Blob;

public static async Task Run(Stream input, CloudBlockBlob output, string name, TraceWriter log, CloudTable inputTable)
{
    log.Info("Start");

    await CreateThumbnail(input, output, name, log, inputTable);

    log.Info("Finish");
}

static async Task CreateThumbnail(Stream input, CloudBlockBlob output, string name, TraceWriter log, CloudTable inputTable)
{
    var cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    using (HttpContent content = new StreamContent(input))
    {
        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"];

        var uri = visionUri + "generateThumbnail?width=200&height=150&smartCropping=true";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

            var response = await client.PostAsync(uri, content);

            var responseBytes = await response.Content.ReadAsStreamAsync();

            if (response.IsSuccessStatusCode)
            {
                await output.UploadFromStreamAsync(responseBytes);
            }
        }
    }

    var tableQuery = new TableQuery<Image>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));

    var tableQueryResult = await inputTable.ExecuteQuerySegmentedAsync(tableQuery, null);

    var image = tableQueryResult.Results.Single();

    image.ThumbUri = output.Uri.AbsoluteUri;

    var updateOperation = TableOperation.Replace(image);

    await inputTable.ExecuteAsync(updateOperation); ;
}