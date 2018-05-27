using System;
using Microsoft.AspNetCore.Http;

namespace Services.Entities.Audio
{
    public class EnrollProfile
    {
        public Guid Id { get; set; }

        public IFormFile Audio { get; set; }
    }
}
