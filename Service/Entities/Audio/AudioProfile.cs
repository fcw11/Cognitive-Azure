using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.WindowsAzure.Storage.Table;

namespace Services.Entities.Audio
{
    public class AudioProfile : TableEntity, IValidatableObject
    {

        public AudioProfile()
        {
            Locale = new[]
            {
                new SelectListItem {Value = "es-ES", Text = "Castilian Spanish"},
                new SelectListItem {Value = "en-US", Text = "American English"},
                new SelectListItem {Value = "fr-FR", Text = "Standard French"},
                new SelectListItem {Value = "zh-CN", Text = "Mandarin Chinese"},
            };
        }

        public Guid Id { get; set; }

        [Required, MinLength(4), Display(Name = "Profile name")]
        public string Name { get; set; }

        [Required]
        public string SelectedLocale { set; get; }

        public Guid SelectedProfile { get; set; }

        public EnrollmentStatus EnrollmentStatus { get; set; }

        public IEnumerable<SelectListItem> Locale { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(SelectedLocale))
            {
                if (Locale.All(x => x.Value != SelectedLocale))
                {
                    yield return new ValidationResult("Invalid Locale", new []{ "SelectedLocale" });
                }
            }
        }
    }
}