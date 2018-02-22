#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System;
using System.Text;

public static void Run(Stream input, Stream output, string name, TraceWriter log, CloudTable inputTable)
{
    log.Info("Start");

    var cogKey = ConfigurationManager.AppSettings["CognitiveService"];

    var client = new HttpClient();

    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cogKey);

    using (HttpContent content = new StreamContent(input))
    {
        var visionUri = ConfigurationManager.AppSettings["CognitiveVisionUri"];

        var uri = visionUri + "generateThumbnail?width=200&height=150&smartCropping=true";

        content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        var response = client.PostAsync(uri, content).Result;

        var responseBytes = response.Content.ReadAsByteArrayAsync().Result;

        if (response.IsSuccessStatusCode)
        {
            output.Write(responseBytes, 0, responseBytes.Length);
        }
    }

    var tableQuery = new TableQuery<Image>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, name));

    var tableQueryResult = inputTable.ExecuteQuerySegmentedAsync(tableQuery, null).Result;

    var image = tableQueryResult.Results.Single();

    image.ThumbUri = "Hello World";

    var updateOperation = TableOperation.Replace(image);

    inputTable.Execute(updateOperation); ;

    log.Info("Finish");
}

public class Image : TableEntity
{
    public string Name { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Uri { get; set; }

    public string ThumbUri { get; set; }
}