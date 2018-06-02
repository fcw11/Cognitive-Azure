using System;
using System.Linq;
using System.Threading.Tasks;
using Services.Entities.Audio;
using Services.Entities.VerificationProfile;

namespace Services.Interfaces
{
    public interface IAudioVerificationService
    {
        Task<Guid> CreateProfile(VerificationProfile profile);
        Task<string> EnrollProfile(EnrollVerificationProfile model);
        Task<EnrollmentStatus> CheckEnrollmentStatus(Guid id);
        Task<EnrollmentPhrases[]> GetVerificationPhrases(string locale);
        Task<IQueryable<AudioProfile>> GetEnrolledProfiles();
        Task DeleteProfiles();
        Task<string> IdentifySpeaker(IdentifyProfile model);
        Task<string> PollIdentifySpeaker(string url);
        Task<VerifySpeakerResponse> VerifySpeaker(VerifySpeaker model);
    }
}