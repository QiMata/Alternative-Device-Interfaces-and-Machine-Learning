using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;

namespace QiMata.AlternativeInterfaces.WebApp.Controllers
{
    [Route("api/SpeakerRecognition")]
    public class SpeakerRecognitionController : Controller
    {
        private readonly SpeakerIdentificationServiceClient _identificationServiceClient;

        public SpeakerRecognitionController()
        {
            _identificationServiceClient = new SpeakerIdentificationServiceClient("{ Put your key in here }");
        }

        [HttpGet("profiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            return Ok(await _identificationServiceClient.GetProfilesAsync());
        }

        [HttpPost("identify")]
        public async Task<IActionResult> PostIdentify([FromQuery]IEnumerable<string> ids, [FromQuery]bool shortAudio)
        {
            return Ok(await _identificationServiceClient.IdentifyAsync(Request.Body,
                ids.Select(x => new Guid(x)).ToArray(), shortAudio));
        }

        [HttpPost("identity/check")]
        public async Task<IActionResult> PostCheckIdentificationStatus([FromBody]OperationLocation operation)
        {
            return Ok(await _identificationServiceClient.CheckIdentificationStatusAsync(operation));
        }

        [HttpPost("profile")]
        public async Task<IActionResult> PostCreateProfile([FromBody] string locale)
        {
            return Ok(await _identificationServiceClient.CreateProfileAsync(locale));
        }

        [HttpPost("enroll/{id}")]
        public async Task<IActionResult> PostEnroll(Guid id, [FromQuery] bool shortAudio)
        {
            return Ok(await _identificationServiceClient.EnrollAsync(Request.Body, id, shortAudio));
        }

        [HttpPost("enroll/check")]
        public async Task<IActionResult> PostCheckEnrollStatus([FromBody]OperationLocation operation)
        {
            return Ok(await _identificationServiceClient.CheckEnrollmentStatusAsync(operation));
        }
    }
}
