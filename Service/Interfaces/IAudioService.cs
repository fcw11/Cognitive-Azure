using System.Threading.Tasks;
using Services.Entities.Audio;

namespace Services.Interfaces
{
    public interface IAudioService
    {
        Task CreateProfile(CreateProfile profile);
    }
}