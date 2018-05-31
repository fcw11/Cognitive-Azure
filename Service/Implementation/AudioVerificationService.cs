using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Services.Entities.Audio;
using Services.Entities.JSON;
using Services.Entities.VerificationProfile;
using Services.Interfaces;
using VerificationProfile = Services.Entities.JSON.Audio.VerificationProfile;

namespace Services.Implementation
{
    public class AudioVerificationService : IAudioVerificationService
    {
        public IConfiguration Configuration { get; set; }

        public ICloudTableService CloudTableService { get; set; }

        public AudioVerificationService(IConfiguration configuration, ICloudTableService cloudTableService)
        {
            Configuration = configuration;
            CloudTableService = cloudTableService;
        }

        public async Task<Guid> CreateProfile(AudioProfile profile)
        {
            var url = Configuration["AudioAnalyticsAPI"] + "verificationProfiles";

            var key = Configuration["AudioAnalyticsKey"];

            var tableName = Configuration["AudioVerificationProfileTable"];

            var payload = $"{{\"locale\" : \"{ profile.SelectedLocale.ToLower() }\"}}";

            var byteData = Encoding.UTF8.GetBytes(payload);

            using (var content = new ByteArrayContent(byteData))
            {
                var response = await CognitiveServicesHttpClient.HttpPost(content, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JSONHelper.FromJson<VerificationProfile>(responseBytes);

                    profile.Id = Guid.Parse(result.IdentificationProfileId);

                    profile.RowKey = profile.Id.ToString();

                    profile.PartitionKey = profile.Id.ToString();

                    await CloudTableService.Insert(profile, tableName);

                    return profile.Id;
                }

                throw new Exception($"Failed request : { responseBytes } ");
            }
        }

        public async Task<string> EnrollProfile(EnrollVerificationProfile model)
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }verificationProfiles/{ model.Id }/enroll";

            var key = Configuration["AudioAnalyticsKey"];

            using (var inputStream = model.Audio.OpenReadStream())
            {
                var response = await CognitiveServicesHttpClient.HttpPost(inputStream, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                return responseBytes;
            }
        }

        public async Task<EnrollmentStatus> CheckEnrollmentStatus(Guid id)
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles/{ id }";

            var key = Configuration["AudioAnalyticsKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JSONHelper.FromJson<EnrollmentStatus>(responseBytes);

                return result;
            }

            throw new Exception($"Failed request : { responseBytes } ");
        }

        public async Task<EnrollmentPhrases[]> GetVerificationPhrases(string locale)
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }verificationPhrases?locale={ locale }";

            var key = Configuration["AudioAnalyticsKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JSONHelper.FromJson<EnrollmentPhrases[]>(responseBytes);

                return result;
            }

            return new EnrollmentPhrases[]{};
        }

        public async Task<IQueryable<AudioProfile>> GetProfiles()
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles";

            var key = Configuration["AudioAnalyticsKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var enrollments = JSONHelper.FromJson<EnrollmentStatus[]>(responseBytes);

                var table = Configuration["AudioVerificationProfileTable"];

                var audioProfiles = await CloudTableService.Retrieve<AudioProfile>(table);

                foreach (var enrollment in enrollments)
                {
                    var profile = audioProfiles.SingleOrDefault(x => x.Id.ToString() == enrollment.IdentificationProfileId);
                    
                    if (profile != null) profile.EnrollmentStatus = enrollment;
                }

                return audioProfiles;
            }

            throw new Exception($"Failed request : { responseBytes } ");
        }

        public async Task DeleteProfiles()
        {
            var profiles = await GetProfiles();

            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles/";

            var key = Configuration["AudioAnalyticsKey"];

            foreach (var profile in profiles)
            {
                var profileUrl = url + profile.Id;
                await CognitiveServicesHttpClient.HttpDelete(profileUrl, key);
            }
        }

        public async Task<string> IdentifySpeaker(IdentifyProfile model)
        {
            var profiles = await GetProfiles();

            var identificationProfileIds = string.Join(",", profiles.Where(x => x.EnrollmentStatus != null &&  x.EnrollmentStatus.EnrollmentStatusEnrollmentStatus == "Enrolled" ).Select(x => x.Id));

            var url = $"{ Configuration["AudioAnalyticsAPI"] }identify?identificationProfileIds={ identificationProfileIds }";

            var key = Configuration["AudioAnalyticsKey"];

            using (var stream = model.Audio.OpenReadStream())
            {
                var response = await CognitiveServicesHttpClient.HttpPost(stream, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                   return response.Headers.GetValues("Operation-Location").FirstOrDefault();
                }

                throw new Exception($"Failed request : { responseBytes } ");
            }
        }

        public async Task<string> PollIdentifySpeaker(string url)
        {
            var key = Configuration["AudioAnalyticsKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            return await response.Content.ReadAsStringAsync();
        }
    }
}