using System;
using System.Threading.Tasks;
using Services.Entities.Audio;

namespace Services.Interfaces
{
    public interface IAudioService
    {
        Task<Guid> CreateProfile(CreateProfile profile);
        Task EnrollProfile(EnrollProfile model);
        Task<EnrollmentStatus> CheckEnrollmentStatus(Guid id);
    }
}