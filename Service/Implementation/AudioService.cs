﻿using System;
using System.IO;
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
                var response = await CognitiveServicesHttpClient.HttpResponseMessage(content, url, key);

                if (response.IsSuccessStatusCode)
                {
                    var responseBytes = await response.Content.ReadAsStringAsync();

                    var result = JSONHelper.FromJson<IdentificationProfile>(responseBytes);

                    profile.Id = Guid.Parse(result.IdentificationProfileId);

                    profile.RowKey = profile.Id.ToString();

                    profile.PartitionKey = profile.Id.ToString();

                    await CloudTableService.Insert(profile, tableName);

                    return profile.Id;
                }
            }

            throw new Exception("Error creating profile");
        }

        public  async Task EnrollProfile(EnrollProfile model)
        {
            var url = $"{ Configuration["AudioAnalyticsAPI"] }identificationProfiles/{ model.Id }/enroll?shortAudio=true";

            var key = Configuration["AudioAnalyticsKey"];

            var a = Convert.FromBase64String(model.Audio);

            var str = System.Text.Encoding.Default.GetString(a);

         //   var file1 = File.Open(@"D:\Cognitive-Azure\Service\Implementation\test.wav", FileMode.Open);

            using (var stream = GenerateStreamFromString(str))
            {
                var response = await CognitiveServicesHttpClient.HttpResponseMessage(stream, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JSONHelper.FromJson<IdentificationProfile>(responseBytes);
                }
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}