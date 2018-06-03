using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Entities.FaceVerification;

namespace Services.Interfaces
{
    public interface IFaceVerificationService
    {
        Task CreatePersonGroup();

        Task<PersonGroups[]> GetPersonGroups();

        Task<Guid> CreateProfile(FaceVerificationProfile model);

        Task AddFace(FaceVerificationEnrollProfile model);

        Task<FaceVerificationProfile> GetProfile(Guid id);

        Task<IQueryable<FaceVerificationProfile>> GetPersons();

        Task DeleteProfiles();

        Task Train();

        Task<TrainingStatus> GetTrainingStatus();

        Task<VerifyFaceResponse> VerifyFace(VerifyFace model);

        Task<Face[]> DetectFace(IFormFile file);
    }
}