using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Services.Entities.Audio;
using Services.Entities.JSON;
using Services.Entities.JSON.Audio;
using Services.Interfaces;

namespace Services.Implementation
{
    public class AudioIdentificationService : IAudioIdentificationService
    {
        public IConfiguration Configuration { get; set; }

        public ICloudTableService CloudTableService { get; set; }

        public AudioIdentificationService(IConfiguration configuration, ICloudTableService cloudTableService)
        {
            Configuration = configuration;
            CloudTableService = cloudTableService;
        }

        public async Task<Guid> CreateProfile(AudioProfile profile)
        {
            var url = Configuration["AudioAnalyticsAPI"] + "identificationProfiles";

            var key = Configuration["AudioAnalyticsKey"];

            var tableName = Configuration["AudioIdentificationProfileTable"];

            var payload = $"{{\"locale\" : \"{ profile.SelectedLocale.ToLower() }\"}}";

            var byteData = Encoding.UTF8.GetBytes(payload);

            using (var content = new ByteArrayContent(byteData))
            {
                var response = await CognitiveServicesHttpClient.HttpPost(content, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JSONHelper.FromJson<IdentificationProfile>(responseBytes);

                    profile.Id = Guid.Parse(result.IdentificationProfileId);

                    profile.RowKey = profile.Id.ToString();

                    profile.PartitionKey = profile.Id.ToString();

                    await CloudTableService.Insert(profile, tableName);

                    return profile.Id;
                }

                throw new Exception($"Failed request : { responseBytes } ");
            }
        }

        public async Task EnrollProfile(EnrollProfile model)
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles/{ model.Id }/enroll";

            var key = Configuration["AudioAnalyticsKey"];

            using (var stream = model.Audio.OpenReadStream())
            {
                await CognitiveServicesHttpClient.HttpPostAudio(stream, url, key);
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

        public async Task<IQueryable<AudioProfile>> GetProfiles()
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles";

            var key = Configuration["AudioAnalyticsKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var enrollments = JSONHelper.FromJson<EnrollmentStatus[]>(responseBytes);

                var table = Configuration["AudioIdentificationProfileTable"];

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

        public async Task<IdentificationResponse> IdentifySpeaker(IdentifyProfile model)
        {
            var profiles = await GetProfiles();

            var identificationProfileIds = string.Join(",", profiles.Where(x => x.EnrollmentStatus != null &&  x.EnrollmentStatus.EnrollmentStatusEnrollmentStatus == "Enrolled" ).Select(x => x.Id));

            var url = $"{ Configuration["AudioAnalyticsAPI"] }identify?identificationProfileIds={ identificationProfileIds }";

            var key = Configuration["AudioAnalyticsKey"];

            using (var stream = model.Audio.OpenReadStream())
            {
                var response = await CognitiveServicesHttpClient.HttpPostAudio(stream, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();

                    return new IdentificationResponse { OperationLocation = operationLocation };
                }
                
                return JSONHelper.FromJson<IdentificationResponse>(responseBytes);
            }
        }

        public async Task<IdentificationStatus> PollIdentifySpeaker(string url)
        {
            var key = Configuration["AudioAnalyticsKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JSONHelper.FromJson<IdentificationStatus>(responseBytes);

                return result;
            }

            throw new Exception($"Failed request : { responseBytes } ");
        }
    }
}