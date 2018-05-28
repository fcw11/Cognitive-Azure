using Microsoft.AspNetCore.Http;

namespace Services.Entities.Audio
{
    public class IdentifyProfile
    {
        public IFormFile Audio { get; set; }
    }
}
