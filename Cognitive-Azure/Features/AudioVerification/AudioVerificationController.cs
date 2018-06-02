using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Entities.Audio;
using Services.Entities.VerificationProfile;
using Services.Interfaces;

namespace Cognitive_Azure.Features.AudioVerification
{
    public class AudioVerificationController : Controller
    {
        public IAudioVerificationService AudioVerificationService { get; set; }

        public AudioVerificationController(IAudioVerificationService audioVerificationService)
        {
            AudioVerificationService = audioVerificationService;
        }

        [HttpGet]
        public IActionResult CreateProfile()
        {
            var model = new VerificationProfile();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProfile(VerificationProfile model)
        {
            if (ModelState.IsValid)
            {
                var profileId = await AudioVerificationService.CreateProfile(model);

                return RedirectToAction("EnrollProfile", new { id = profileId, locale = model.SelectedLocale });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EnrollProfile(Guid id, string locale)
        {
            var model = new EnrollVerificationProfile { Id = id };

            model.VerificationPhrases = await AudioVerificationService.GetVerificationPhrases(locale);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> EnrollProfile(EnrollVerificationProfile model)
        {
            if (ModelState.IsValid)
            {
                var result = await AudioVerificationService.EnrollProfile(model);
                return new JsonResult(result);
            }

            return new JsonResult(string.Empty);
        }

        public async Task<IActionResult> VerifySpeaker()
        {
            var speakers = await AudioVerificationService.GetEnrolledProfiles();

            return View(speakers);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> VerifySpeaker(VerifySpeaker model)
        {
            if (ModelState.IsValid)
            {
                var result = await AudioVerificationService.VerifySpeaker(model);
                return new JsonResult(result);
            }

            return new JsonResult(string.Empty);
        }

        [HttpGet]
        public async Task<JsonResult> DeleteSpeakers()
        {
            await AudioVerificationService.DeleteProfiles();

            return new JsonResult(string.Empty);
        }
    }
}