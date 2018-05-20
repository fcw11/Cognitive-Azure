using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Entities;

namespace Services.Interfaces
{
    public interface IImageService
    {
        Task UploadImage(IFormFile file);

        Task UploadImageInformationToTable(IFormFile file, Guid imageId, Uri blobUri);

        Task<IQueryable<Image>> RetrieveImages();

        Task<Image> RetrieveImage(Guid id);

        Task AddComment(Guid id, string comment, double score, string phrases);

        Task<IQueryable<ImageComment>> RetrieveComments(Guid id);
    }
}
