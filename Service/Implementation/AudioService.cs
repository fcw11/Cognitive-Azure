﻿using System;
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

        public async Task<Guid> CreateProfile(CreateProfile profile)
        {
            var url = Configuration["AudioAnalyticsAPI"] + "identificationProfiles";

            var key = Configuration["AudioAnalyticsKey"];

            var tableName = Configuration["AudioProfileTable"];

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

        public  async Task EnrollProfile(EnrollProfile model)
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles/{ model.Id }/enroll";

            var key = Configuration["AudioAnalyticsKey"];

            using (var stream = model.Audio.OpenReadStream())
            {
                var response = await CognitiveServicesHttpClient.HttpPost(stream, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JSONHelper.FromJson<IdentificationProfile>(responseBytes);
                }

                throw new Exception($"Failed request : { responseBytes } ");
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

        public async Task<EnrollmentStatus[]> GetProfiles()
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles";

            var key = Configuration["AudioAnalyticsKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JSONHelper.FromJson<EnrollmentStatus[]>(responseBytes);

                return result;
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
                var profileUrl = url + profile.IdentificationProfileId;
                await CognitiveServicesHttpClient.HttpDelete(profileUrl, key);
            }
        }
    }
}