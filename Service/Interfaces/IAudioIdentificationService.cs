using System;
using System.Linq;
using System.Threading.Tasks;
using Services.Entities.Audio;

namespace Services.Interfaces
{
    public interface IAudioIdentificationService
    {
        Task<Guid> CreateProfile(AudioProfile profile);
        Task EnrollProfile(EnrollProfile model);
        Task<EnrollmentStatus> CheckEnrollmentStatus(Guid id);
        Task<IQueryable<AudioProfile>> GetProfiles();
        Task DeleteProfiles();
        Task<IdentificationResponse> IdentifySpeaker(IdentifyProfile model);
        Task<IdentificationStatus> PollIdentifySpeaker(string url);
    }
}