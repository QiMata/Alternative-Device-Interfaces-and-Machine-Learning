using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using Newtonsoft.Json;
using QiMata.AlternativeInterfaces.Services.Models;

namespace QiMata.AlternativeInterfaces.Services
{
    class FaceService
    {
        private HttpClient _client;

        public FaceService(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<Person> DetectPerson(Stream imageStream)
        {
            var streamContent = new StreamContent(imageStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var postResponse = await
                _client.PostAsync($"api/FaceRecognition",streamContent);

            postResponse.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Person>(await postResponse.Content.ReadAsStringAsync());
        }

        public async Task CreateGroup(Stream imageStream)
        {
            var streamContent = new StreamContent(imageStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var postResponse = await
                _client.PostAsync($"api/FaceRecognition/persongroup", streamContent);

            postResponse.EnsureSuccessStatusCode();
        }
    }
}
