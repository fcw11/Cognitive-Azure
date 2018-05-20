using System;
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
    public class AudioService : IAudioService
    {
        public IConfiguration Configuration { get; set; }

        public ICloudTableService CloudTableService { get; set; }

        public AudioService(IConfiguration configuration, ICloudTableService cloudTableService)
        {
            Configuration = configuration;
            CloudTableService = cloudTableService;
        }

        public async Task CreateProfile(CreateProfile profile)
        {
            var url = Configuration["AudioAnalyticsAPI"] + "identificationProfiles";
            var key = Configuration["AudioAnalyticsKey"];

            var tableName = Configuration["AudioProfileTable"];

            var payload = $"{{\"locale\" : \"{ profile.SelectedLocale.ToLower() }\"}}";

            var byteData = Encoding.UTF8.GetBytes(payload);

            using (var content = new ByteArrayContent(byteData))
            {
                var response = await CognitiveServicesHttpClient.HttpResponseMessage(content, url, key);

                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = await response.Content.ReadAsStringAsync();

                    var result = JSONHelper.FromJson<IdentificationProfile>(responseBytes);

                    profile.Id = Guid.Parse(result.IdentificationProfileId);

                    await CloudTableService.Insert(profile, tableName);
                }
            }
        }
    }
}