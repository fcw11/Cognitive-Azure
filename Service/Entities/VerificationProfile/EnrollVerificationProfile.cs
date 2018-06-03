using System;
using Microsoft.AspNetCore.Http;

namespace Services.Entities.VerificationProfile
{
    public class EnrollVerificationProfile
    {
        public Guid Id { get; set; }

        public IFormFile Audio { get; set; }

        public EnrollmentPhrases[] VerificationPhrases { get; set; }
    }
}
