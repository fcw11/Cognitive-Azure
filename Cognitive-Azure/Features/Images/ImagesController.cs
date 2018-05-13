using System;
using System.Linq;
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
        public JsonResult AddComment(string comment)
        {
            if (ModelState.IsValid)
            {
                TextService.GetScore(comment);
                return new JsonResult("good");
            }

            return new JsonResult("Bad");
        }
    }
}