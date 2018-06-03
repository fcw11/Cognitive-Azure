using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Entities.Audio;
using Services.Interfaces;

namespace Cognitive_Azure.Features.AudioIdentification
{
    public class AudioIdentificationController : Controller
    {
        public IAudioIdentificationService AudioIdentificationService { get; set; }

        public AudioIdentificationController(IAudioIdentificationService audioIdentificationService)
        {
            AudioIdentificationService = audioIdentificationService;
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
               var profileId = await AudioIdentificationService.CreateProfile(model);

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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> EnrollProfile(EnrollProfile model)
        {
            if (ModelState.IsValid)
            {
                await AudioIdentificationService.EnrollProfile(model);
            }

            return new JsonResult(string.Empty);
        }

        [HttpGet]
        public async Task<JsonResult> CheckEnrollmentStatus(Guid id)
        {
            var result = await AudioIdentificationService.CheckEnrollmentStatus(id);

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> IdentifySpeaker()
        {
            var profiles = await AudioIdentificationService.GetProfiles();

            profiles = profiles.Where(x => x.EnrollmentStatus != null && x.EnrollmentStatus.EnrollmentStatusEnrollmentStatus == "Enrolled");

            return View(profiles);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> IdentifySpeaker(IdentifyProfile model)
        {
            if (ModelState.IsValid)
            {
                var result = await AudioIdentificationService.IdentifySpeaker(model);

                return new JsonResult(result);
            }

            return new JsonResult(string.Empty);
        }

        [HttpGet]
        public async Task<JsonResult> PollIdentifySpeaker(string url)
        {
            if (ModelState.IsValid)
            {
               var result = await AudioIdentificationService.PollIdentifySpeaker(url);

                return new JsonResult(result);
            }

            return new JsonResult(string.Empty);
        }

        [HttpGet]
        public async Task<JsonResult> DeleteProfiles()
        {
            await AudioIdentificationService.DeleteProfiles();

            return new JsonResult(string.Empty);
        }
    }
}