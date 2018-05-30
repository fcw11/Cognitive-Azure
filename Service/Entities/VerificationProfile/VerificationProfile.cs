using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.WindowsAzure.Storage.Table;
using Services.Entities.Audio;

namespace Services.Entities.VerificationProfile
{
    public class VerificationProfile : TableEntity, IValidatableObject
    {
        public VerificationProfile()
        {
            Locale = new[]
            {
                new SelectListItem {Value = "en-US", Text = "American English"}
            };
        }

        public Guid Id { get; set; }

        [Required, MinLength(4), Display(Name = "Profile name")]
        public string Name { get; set; }

        [Required]
        public string SelectedLocale { set; get; }

        public EnrollmentStatus EnrollmentStatus { get; set; }

        public IEnumerable<SelectListItem> Locale { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(SelectedLocale))
            {
                if (Locale.All(x => x.Value != SelectedLocale))
                {
                    yield return new ValidationResult("Invalid Locale", new[] { "SelectedLocale" });
                }
            }
        }
    }
}