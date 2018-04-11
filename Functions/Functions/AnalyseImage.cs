using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Funcs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Entities.JSON;
using Services.Entities.JSON.Analyse;

namespace Functions.Functions
{
    public static class AnalyseImage
    {
        [FunctionName("AnalyseImage")]
        public static async Task Run(
            [BlobTrigger("images/{name}")] Stream trigger,
            [Table("images")] CloudTable cloudTable,
            [Blob("faces/{name}", FileAccess.Write)]
            Stream outputStream,
            string name,
            TraceWriter log)
        {
            var o = new MemoryStream();
            trigger.CopyTo(o);
            trigger.CopyTo(outputStream);

            using (var ms = new MemoryStream())
            {
                trigger.Position = 0;

                trigger.CopyTo(ms);

                ms.Position = 0;

                using (HttpContent content = new StreamContent(ms))
                {
                    var parameters = "analyze" +
                                     "?visualFeatures=Categories,Tags,Description,Faces,ImageType,Color,Adult" +
                                     "&details=Celebrities,Landmarks";

                    var response = await CognitiveServicesHttpClient.PostRequest(content, parameters);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBytes = await response.Content.ReadAsStringAsync();

                        await cloudTable.Update(name, responseBytes, (image, text) => { image.Analyse = text; });

                        var describe = JSONHelper.FromJson<Analyse>(responseBytes);

                        if (describe.Faces.Any())
                        {
                            try
                            {
                                using (var bitmap = new Bitmap(o))
                                {
                                    using (var graph = Graphics.FromImage(bitmap))
                                    {
                                        var outline = new Pen(Brushes.Red, 2);
                                        var text = new Pen(Color.White, 2);

                                        foreach (var face in describe.Faces)
                                        {
                                            graph.DrawRectangle(outline, face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height);

                                            var font = new Font(FontFamily.GenericSerif, 16, FontStyle.Bold);
                                            graph.DrawString($"{face.Gender} {face.Age}", font, text.Brush, face.FaceRectangle.Left, face.FaceRectangle.Top);
                                        }
                                    }
                                    bitmap.Save(outputStream, ImageFormat.Jpeg);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }

            log.Info("Finish");
        }
    }
}