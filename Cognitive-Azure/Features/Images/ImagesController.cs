using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cognitive_Azure.Features.Images
{
    public class ImagesController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public ImagesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Upload(Upload model)
        {
            if (ModelState.IsValid)
            {
                var accountName = Configuration["BlobAccountName"];
                var key = Configuration["BlobStorageAPIKey"];
                var containerName = Configuration["ImageContainerName"];

                var storageAccount = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(accountName, key),true);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
                using (var stream = model.Image.OpenReadStream())
                {
                    blockBlob.UploadFromStreamAsync(stream).Wait();
                }
            }

            return View(model);
        }
    }
}