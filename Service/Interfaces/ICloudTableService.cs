using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Entities;

namespace Services.Interfaces
{
    public interface ICloudTableService
    {
        Task AddComment(Guid id, string comment, double score, string phrases);

        Task<IQueryable<Image>> RetrieveImages();

        Task<Image> RetrieveImage(Guid id);

        Task UploadImageInformationToTable(IFormFile file, Guid imageId, Uri blobUri);

        Task CreateTablesIfNotExist();
    }
}