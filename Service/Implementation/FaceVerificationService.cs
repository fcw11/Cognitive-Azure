using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Entities.FaceVerification;
using Services.Entities.JSON;
using Services.Interfaces;

namespace Services.Implementation
{
    public class FaceVerificationService : IFaceVerificationService
    {
        public IConfiguration Configuration { get; set; }

        public ICloudTableService CloudTableService { get; set; }

        public FaceVerificationService(IConfiguration configuration, ICloudTableService cloudTableService)
        {
            Configuration = configuration;
            CloudTableService = cloudTableService;
        }

        public async Task CreatePersonGroup()
        {
            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}";

            var key = Configuration["CognitiveFacesKey"];

            var payload = "{ \"name\": \"group1\"}";

            var byteData = Encoding.UTF8.GetBytes(payload);

            using (var content = new ByteArrayContent(byteData))
            {
                await CognitiveServicesHttpClient.HttpPut(content, url, key);
            }
        }

        public async Task<PersonGroups[]> GetPersonGroups()
        {
            var url = Configuration["CognitiveFacesUri"] + $"/persongroups";

            var key = Configuration["CognitiveFacesKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JSONHelper.FromJson<PersonGroups[]>(responseBytes);
            }

            throw new Exception($"Failed request : { responseBytes } ");
        }

        public async Task<Guid> CreateProfile(FaceVerificationProfile model)
        {
            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}/persons";

            var key = Configuration["CognitiveFacesKey"];

            var payload = "{ \"name\": \"" + model.Name + "\"}";

            var byteData = Encoding.UTF8.GetBytes(payload);

            using (var content = new ByteArrayContent(byteData))
            {
                var response = await CognitiveServicesHttpClient.HttpPost(content, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JSONHelper.FromJson<CreatePersonResponse>(responseBytes);

                    return result.PersonId;
                }

                throw new Exception($"Failed request : {responseBytes} ");
            }
        }

        public async Task AddFace(FaceVerificationEnrollProfile model)
        {
            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}/persons/{model.PersonId}/persistedFaces";

            var key = Configuration["CognitiveFacesKey"];

            using (var stream = model.Image.OpenReadStream())
            {
                using (HttpContent content = new StreamContent(stream))
                {
                   await CognitiveServicesHttpClient.HttpPostImage(content, url, key);
                }
            }
        }

        public async Task<FaceVerificationProfile> GetProfile(Guid id)
        {
            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}/persons/{ id }";

            var key = Configuration["CognitiveFacesKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JSONHelper.FromJson<FaceVerificationProfile>(responseBytes);
            }


            throw new Exception($"Failed request : {responseBytes} ");
        }

        public async Task<IQueryable<FaceVerificationProfile>> GetPersons()
        {
            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}/persons";

            var key = Configuration["CognitiveFacesKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JSONHelper.FromJson<List<FaceVerificationProfile>>(responseBytes).AsQueryable();
            }

            throw new Exception($"Failed request : {responseBytes} ");
        }

        public async Task DeleteProfiles()
        {
            var profiles = await GetPersons();

            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}/persons/";

            var key = Configuration["CognitiveFacesKey"];

            foreach (var profile in profiles)
            {
                var deleteUrl = url + profile.PersonId;

                await CognitiveServicesHttpClient.HttpDelete(deleteUrl, key);
            }
        }

        public async Task Train()
        {
            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}/train";

            var key = Configuration["CognitiveFacesKey"];

            var byteData = Encoding.UTF8.GetBytes(String.Empty);

            using (var content = new ByteArrayContent(byteData))
            {
                await CognitiveServicesHttpClient.HttpPost(content, url, key);
            }
        }

        public async Task<TrainingStatus> GetTrainingStatus()
        {
            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + $"/persongroups/{groupId}/training";

            var key = Configuration["CognitiveFacesKey"];

            var response = await CognitiveServicesHttpClient.HttpGet(url, key);

            var responseBytes = await response.Content.ReadAsStringAsync();

            return JSONHelper.FromJson<TrainingStatus>(responseBytes);
        }

        public async Task<VerifyFaceResponse> VerifyFace(VerifyFace model)
        {
            var face = await DetectFace(model.Image);

            var groupId = Configuration["PersonGroupId"];

            var url = Configuration["CognitiveFacesUri"] + "/verify";

            var key = Configuration["CognitiveFacesKey"];

            var payload = "{ \"faceId\": \"" + face.First().FaceId + "\", \"personId\": \"" + model.Id + "\", \"personGroupId\": \"" + groupId  + "\"}";

            var byteData = Encoding.UTF8.GetBytes(payload);

            using (var content = new ByteArrayContent(byteData))
            {
                var response = await CognitiveServicesHttpClient.HttpPost(content, url, key);

                var responseBytes = await response.Content.ReadAsStringAsync();

                return JSONHelper.FromJson<VerifyFaceResponse>(responseBytes);
            }
        }

        public async Task<Face[]> DetectFace(IFormFile file)
        {
            var url = Configuration["CognitiveFacesUri"] + $"/detect?returnFaceId=true";

            var key = Configuration["CognitiveFacesKey"];

            using (var stream = file.OpenReadStream())
            {
                using (HttpContent content = new StreamContent(stream))
                {
                    var response = await CognitiveServicesHttpClient.HttpPostImage(content, url, key);

                    var responseBytes = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    { 
                        return JSONHelper.FromJson<Face[]>(responseBytes);
                    }

                    throw new Exception($"Failed request : {responseBytes} ");
                }
            }
        }
    }
}