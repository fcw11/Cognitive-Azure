using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Cognitive_Azure.Features.Images
{
    public class ImagesController : Controller
    {
        public IImageService ImageService { get; set; }

        public ITextService TextService { get; set; }

        public ImagesController(IImageService imageService, ITextService textService)
        {
            ImageService = imageService;
            TextService = textService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await ImageService.RetrieveImages();

            return View(items.ToList());
        }

        public async Task<IActionResult> View(Guid id)
        {
            var item = await ImageService.RetrieveImage(id);

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
                ImageService.UploadImage(model.Image);

                return RedirectToAction("Index", "Images");
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> AnalyseComment(string comment)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(comment)) 
            {
                var score = await TextService.GetScore(comment);

                var phrases = await TextService.GetKeyPhrases(comment);

                return new JsonResult(new { score, phrases });
            }

            return new JsonResult(new { score = 0, phrases = string.Empty, ModelState.Values });
        }

        public async Task<IActionResult> AddComment(Guid id, string comment)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(comment))
            {
                var score = await TextService.GetScore(comment);

                var phrase = await TextService.GetKeyPhrases(comment);

                await ImageService.AddComment(id, comment, score, phrase);
            }

            return RedirectToAction("View", new { id });
        }
    }
}