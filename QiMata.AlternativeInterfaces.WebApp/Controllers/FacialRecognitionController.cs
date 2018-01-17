using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;

namespace QiMata.AlternativeInterfaces.WebApp.Controllers
{
    [Route("api/FaceRecognition")]
    public class FacialRecognitionController : Controller
    {
        private FaceServiceClient _faceServiceClient;

        public FacialRecognitionController()
        {
            _faceServiceClient = new FaceServiceClient("ef04718690124a54bf6e0ef254343f00",
                "https://westus.api.cognitive.microsoft.com/spid/v1.0");
        }

        [HttpPost]
        public async Task<IActionResult> PostCompareFace()
        {
            var ids = await _faceServiceClient.ListFaceListsAsync();
            var personGroupId = ids.FirstOrDefault();

            if (personGroupId == null)
            {
                return base.StatusCode(500, "Person group not created");
            }

            var faces = await _faceServiceClient.DetectAsync(Request.Body);
            var faceIds = faces.Select(face => face.FaceId).ToArray();

            var results = await _faceServiceClient.IdentifyAsync(personGroupId.FaceListId, faceIds);
            var result = results[0].Candidates[0].PersonId;

            var person = await _faceServiceClient.GetPersonAsync(personGroupId.FaceListId, result);

            return Ok(person);
        }

        [HttpPost("persongroup")]
        public async Task<IActionResult> PostCreateGroup()
        {
            var personGroupId = Guid.NewGuid().ToString().Replace("-","");
            await _faceServiceClient.CreateFaceListAsync(personGroupId, "Just Me");

            await _faceServiceClient.AddFaceToFaceListAsync(personGroupId, Request.Body);

            await _faceServiceClient.TrainPersonGroupAsync(personGroupId);

            return Ok();
        }
    }
}
