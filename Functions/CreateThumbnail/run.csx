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
using Microsoft.WindowsAzure.Storage.Blob;

public static void Run(Stream input, CloudBlockBlob output, string name, TraceWriter log, CloudTable inputTable)
{
    log.Info("Startbbb");

    var cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    var client = new HttpClient();

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

    using (HttpContent content = new StreamContent(input))
    {
        var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"];

        var uri = visionUri + "generateThumbnail?width=200&height=150&smartCropping=true";

        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var response = client.PostAsync(uri, content).Result;

        var responseBytes = response.Content.ReadAsStreamAsync().Result;

        if (response.IsSuccessStatusCode)
        {
            output.UploadFromStreamAsync(responseBytes).Wait();
        }
    }

    var tableQuery = new TableQuery<Image>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));

    var tableQueryResult = inputTable.ExecuteQuerySegmentedAsync(tableQuery, null).Result;

    var image = tableQueryResult.Results.Single();

    image.ThumbUri = output.Uri.AbsoluteUri;

    var updateOperation = TableOperation.Replace(image);

    inputTable.Execute(updateOperation); ;

    log.Info("Finish");
}