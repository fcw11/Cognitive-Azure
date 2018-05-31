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
        public async Task<ActionResult> CreateProfile(AudioProfile model)
        {
            if (ModelState.IsValid)
            {
                var profileId = await AudioVerificationService.CreateProfile(model);

                return RedirectToAction("EnrollProfile", new { id = profileId, model.Locale });
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

        [HttpPost]
        public async Task<JsonResult> EnrollProfile(EnrollVerificationProfile model)
        {
            if (ModelState.IsValid)
            {
                var result = await AudioVerificationService.EnrollProfile(model);
                return new JsonResult(result);
            }

            return new JsonResult(string.Empty);
        }

        //[HttpGet]
        //public async Task<JsonResult> CheckEnrollmentStatus(Guid id)
        //{
        //    var result = await AudioService.CheckEnrollmentStatus(id);

        //    return new JsonResult(result);
        //}

        //[HttpGet]
        //public async Task<IActionResult> IdentifySpeaker()
        //{
        //    var profiles = await AudioService.GetProfiles();

        //    profiles = profiles.Where(x => x.EnrollmentStatus != null && x.EnrollmentStatus.EnrollmentStatusEnrollmentStatus == "Enrolled");

        //    return View(profiles);
        //}

        //[HttpPost]
        //public async Task<JsonResult> IdentifySpeaker(IdentifyProfile model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await AudioService.IdentifySpeaker(model);

        //        return new JsonResult(result);
        //    }

        //    return new JsonResult(string.Empty);
        //}

        //[HttpGet]
        //public async Task<JsonResult> PollIdentifySpeaker(string url)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await AudioService.PollIdentifySpeaker(url);

        //        return new JsonResult(result);
        //    }

        //    return new JsonResult(string.Empty);
        //}

        //[HttpGet]
        //public async Task<IActionResult> DeleteSpeakers()
        //{
        //    await AudioService.DeleteProfiles();

        //    return View();
        //}
    }
}