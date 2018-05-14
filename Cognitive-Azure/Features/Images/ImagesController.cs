using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Cognitive_Azure.Features.Images
{
    public class ImagesController : Controller
    {
        public ICloudStorageService CloudStorageService { get; set; }

        public ITextService TextService { get; set; }

        public ImagesController(ICloudStorageService cloudStorageService, ITextService textService)
        {
            CloudStorageService = cloudStorageService;
            TextService = textService;
        }

        public IActionResult Index()
        {
            var items = CloudStorageService.RetrieveImages();

            return View(items.ToList());
        }

        public IActionResult View(Guid id)
        {
            var item = CloudStorageService.RetrieveImage(id);

            return View(item);
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
                CloudStorageService.UploadImage(model.Image);

                return RedirectToAction("Index", "Images");
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> AddComment(string comment)
        {
            if (ModelState.IsValid)
            {
                var score = await TextService.GetScore(comment);

                return new JsonResult(new { score });
            }

            return new JsonResult("Invalid request");
        }
    }
}