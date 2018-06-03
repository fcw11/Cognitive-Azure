using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Services.Entities.FaceVerification
{
    public class VerifyFace : IValidatableObject
    {
        [Required(ErrorMessage = "Please select a person"), Display(Name = "Person")]
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Please attach a file"), Display(Name = "Image")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [FileExtensions(Extensions = (".jpeg, .jpg, .png, .gif, .bmp"))]
        public string ImageName => Image != null ? Image.FileName : string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Image.Length > 4194304)
            {
                yield return new ValidationResult("Image file size is too large", new[] { "Image" });
            }

            var fileTypes = new[] { "image/png", "image/jpeg", "image/gif", "image/bmp" };

            if (Image.Length > 0 && !fileTypes.Contains(Image.ContentType))
            {
                yield return new ValidationResult("Unsupported image format", new[] { "Image" });
            }
        }
    }
}
