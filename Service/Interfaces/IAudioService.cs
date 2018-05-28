using System;
using System.Linq;
using System.Threading.Tasks;
using Services.Entities.Audio;

namespace Services.Interfaces
{
    public interface IAudioService
    {
        Task<Guid> CreateProfile(AudioProfile profile);
        Task EnrollProfile(EnrollProfile model);
        Task<EnrollmentStatus> CheckEnrollmentStatus(Guid id);
        Task<IQueryable<AudioProfile>> GetProfiles();
        Task DeleteProfiles();
        Task<string> IdentifySpeaker(IdentifyProfile model);
        Task<string> PollIdentifySpeaker(string url);
    }
}