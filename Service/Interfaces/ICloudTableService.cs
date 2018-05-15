using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Entities;

namespace Services.Interfaces
{
    public interface ICloudTableService
    {
        Task AddComment(Guid id, string comment, double score);

        Task<IQueryable<ImageComment>> RetrieveComments(Guid imageId);

        IQueryable<Image> RetrieveImages();

        Image RetrieveImage(Guid id);

        Task UploadImageInformationToTable(IFormFile file, Guid imageId, Uri blobUri);
    }
}