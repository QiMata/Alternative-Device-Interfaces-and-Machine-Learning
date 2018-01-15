using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using Newtonsoft.Json;

namespace QiMata.AlternativeInterfaces.Services
{
    class SpeakerRecognitionService
    {
        private HttpClient _client;

        public SpeakerRecognitionService(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }


        public async Task<Profile[]> GetAllProfilesAsync()
        {
            var getResponse = await _client.GetAsync("api/SpeakerRecognition/profiles");

            getResponse.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Profile[]>(await getResponse.Content.ReadAsStringAsync());
        }

        public async Task<OperationLocation> IdentifyAsync(Stream audioStream, Guid[] ids, bool shortAudio = false)
        {
            var streamContent = new StreamContent(audioStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var postResponse = await
                _client.PostAsync(
                    $"api/SpeakerRecognition/identify/?ids={ids.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y)}&shortAudio={shortAudio}",
                    streamContent);

            postResponse.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<OperationLocation>(await postResponse.Content.ReadAsStringAsync());
        }

        public async Task<IdentificationOperation> CheckIdentificationStatusAsync(OperationLocation id)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(id));
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var postResponse = await _client.PostAsync("api/SpeakerRecognition/identity/check", stringContent);

            postResponse.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IdentificationOperation>(await postResponse.Content.ReadAsStringAsync());
        }

        public async Task<CreateProfileResponse> CreateProfileAsync(string locale)
        {
            var postResponse = await _client.PostAsync("api/SpeakerRecognition/profile", new StringContent(locale));

            postResponse.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<CreateProfileResponse>(await postResponse.Content.ReadAsStringAsync());
        }

        public async Task<OperationLocation> EnrollAsync(Stream audioStream, Guid profileId, bool shortAudio = false)
        {
            var streamContent = new StreamContent(audioStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var postResponse = await _client.PostAsync($"api/SpeakerRecognition/enroll/{profileId}", streamContent);

            postResponse.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<OperationLocation>(await postResponse.Content.ReadAsStringAsync());
        }

        public async Task<EnrollmentOperation> CheckEnrollmentStatusAsync(OperationLocation enrollment)
        {
            var postResponse = await _client.PostAsync("api/SpeakerRecognition/enroll/check", new StringContent(JsonConvert.SerializeObject(enrollment)));

            postResponse.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<EnrollmentOperation>(await postResponse.Content.ReadAsStringAsync());
        }
    }
}
