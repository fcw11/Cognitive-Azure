using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> CreateProfile(CreateProfile profile)
        {
            if (ModelState.IsValid)
            {
                await AudioService.CreateProfile(profile);
            }

            return View(profile);
        }
    }
}