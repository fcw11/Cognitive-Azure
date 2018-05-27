using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Entities.Audio;
using Services.Interfaces;

namespace Cognitive_Azure.Features.Audio
{
    public class AudioController : Controller
    {
        public IAudioService AudioService { get; set; }

        public AudioController(IAudioService audioService)
        {
            AudioService = audioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateProfile()
        {
            var model = new CreateProfile();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProfile(CreateProfile model)
        {
            if (ModelState.IsValid)
            {
               var profileId = await AudioService.CreateProfile(model);

                return RedirectToAction("EnrollProfile", new { id = profileId });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EnrollProfile(Guid id)
        {
            var model = new EnrollProfile { Id = id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnrollProfile(EnrollProfile model)
        {
            if (ModelState.IsValid)
            {
                var audio = Request.Form.Files["audio"];
                model.Audio = audio;

                await AudioService.EnrollProfile(model);
            }
            

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> CheckEnrollmentStatus(Guid id)
        {
            var result = await AudioService.CheckEnrollmentStatus(id);

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> IdentifySpeaker()
        {
            await AudioService.GetProfiles();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSpeakers()
        {
            await AudioService.DeleteProfiles();

            return View();
        }
    }
}