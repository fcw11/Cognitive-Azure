using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Services.Entities.FaceVerification
{
    public class FaceVerificationEnrollProfile : IValidatableObject
    {
        public Guid PersonId { get; set; }

        [Required]
        [Display(Name = "Please select a file")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [FileExtensions(Extensions = (".jpeg, .jpg, .png, .gif, .bmp"))]
        public string ImageName => Image != null ? Image.FileName : string.Empty;

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Face count")]
        public int FaceCount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Image.Length > 4194304)
            {
                yield return new ValidationResult("Image file size is too large", new[] { "Image" });
            }

            var fileTypes = new[] { "image/png", "image/jpeg", "image/gif", "image/bmp" };

            if (!fileTypes.Contains(Image.ContentType))
            {
                yield return new ValidationResult("Unsupported image format", new[] { "Image" });
            }
        }
    }
}