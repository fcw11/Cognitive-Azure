using System;
using Microsoft.AspNetCore.Http;

namespace Services.Entities.VerificationProfile
{
    public class VerifySpeaker
    {
        public Guid Id { get; set; }

        public IFormFile Audio { get; set; }
    }
}
