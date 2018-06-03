using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Entities.FaceVerification;
using Services.Interfaces;

namespace Cognitive_Azure.Features.FaceVerification
{
    public class FaceVerificationController : Controller
    {
        public IFaceVerificationService FaceVerificationService { get; set; }

        public FaceVerificationController(IFaceVerificationService faceVerificationService)
        {
            FaceVerificationService = faceVerificationService;
        }

        public async Task<IActionResult> VerifyFace()
        {
            var groups = await FaceVerificationService.GetPersons();

            return View(groups);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> VerifyFace(VerifyFace model)
        {
            if (ModelState.IsValid)
            {
                 var response = await FaceVerificationService.VerifyFace(model);
                return new JsonResult(response);
            }

            return new JsonResult("");
        }
        
        [HttpGet]
        public IActionResult CreateProfile()
        {
            var model = new FaceVerificationProfile();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProfile(FaceVerificationProfile model)
        {
            if (ModelState.IsValid)
            {
                var profileId = await FaceVerificationService.CreateProfile(model);

                return RedirectToAction("EnrollProfile", new { id = profileId });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EnrollProfile(Guid id)
        {
            var profile = await FaceVerificationService.GetProfile(id);

            var model = new FaceVerificationEnrollProfile
            {
                PersonId = profile.PersonId,
                Name = profile.Name,
                FaceCount = profile.PersistedFaceIds.Count
            };
            
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollProfile(FaceVerificationEnrollProfile model)
        {
            if (ModelState.IsValid)
            {
                await FaceVerificationService.AddFace(model);

                return RedirectToAction("EnrollProfile", new { id = model.PersonId });
            }

            var profile = await FaceVerificationService.GetProfile(model.PersonId);
            model.Name = profile.Name;
            model.FaceCount = profile.PersistedFaceIds.Count;

            return View(model);
        }

        public async Task<IActionResult> TrainProfiles()
        {
            await FaceVerificationService.Train();

            return View();
        }

        public async Task<JsonResult> GetTrainingStatus()
        {
            var response = await FaceVerificationService.GetTrainingStatus();

            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<JsonResult> DeleteProfiles()
        {
            await FaceVerificationService.DeleteProfiles();

            return new JsonResult(string.Empty);
        }
    }
}