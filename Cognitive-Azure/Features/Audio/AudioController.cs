using Microsoft.AspNetCore.Mvc;

namespace Cognitive_Azure.Features.Audio
{
    public class AudioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}