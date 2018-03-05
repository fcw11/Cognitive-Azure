using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Cognitive_Azure.Features.Images
{
    public class Upload : IValidatableObject
    {
        [Required]
        [Display(Name = "Please select a file", Description = "ggg")]
        [System.ComponentModel.DataAnnotations.DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [FileExtensions(Extensions = (".JPEG, .PNG, .GIF, .BMP"))]
        public string ImageName => Image != null ? Image.FileName : string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Image.Length > 4194304)
            {
                yield return new ValidationResult("Image file size is too large", new[] {"Image"});
            }        

            var fileTypes = new[] {"image/png", "image/jpg", "image/png", "image/png" };

            if (fileTypes.Contains(Image.ContentType))
            {
                yield return new ValidationResult("Incorrect image format", new[] { "Image" });
            }
        }
    }
}
