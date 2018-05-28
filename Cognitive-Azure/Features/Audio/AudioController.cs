using System;
using System.Linq;
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
            var model = new AudioProfile();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProfile(AudioProfile model)
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
            var profiles = await AudioService.GetProfiles();

            profiles = profiles.Where(x => x.EnrollmentStatus != null && x.EnrollmentStatus.EnrollmentStatusEnrollmentStatus == "Enrolled");

            return View(profiles);
        }

        [HttpPost]
        public async Task<JsonResult> IdentifySpeaker(IdentifyProfile model)
        {
            if (ModelState.IsValid)
            {
                var result = await AudioService.IdentifySpeaker(model);

                return new JsonResult(result);
            }

            return new JsonResult(string.Empty);
        }

        [HttpGet]
        public async Task<JsonResult> PollIdentifySpeaker(string url)
        {
            if (ModelState.IsValid)
            {
               var result = await AudioService.PollIdentifySpeaker(url);

                return new JsonResult(result);
            }

            return new JsonResult(string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSpeakers()
        {
            await AudioService.DeleteProfiles();

            return View();
        }
    }
}