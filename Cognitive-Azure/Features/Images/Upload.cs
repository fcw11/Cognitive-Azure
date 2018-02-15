using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Cognitive_Azure.Features.Images
{
    public class Upload
    {
        [Required]
        [Display(Name = "Please select a file")]
        [System.ComponentModel.DataAnnotations.DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [FileExtensions(Extensions = (".png,.jpg,.jpeg,.gif"))]
        public string ImageName => Image != null ? Image.FileName : string.Empty;
    }
}
