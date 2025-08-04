using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Services;
using time_track_service.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace time_track_service.Controllers
{
    [Route("api/hydration")]
    [ApiController]
    public class HydrationController : ControllerBase
    {
        private readonly IHydrationService _hydrationService;
        private readonly ServicesSettings _servicesSettings;
        private readonly IRequestContextAccessor _requestContextAccessor;
    
        private bool ValidateAuthorization()
        {   
            return string.Equals(
                _requestContextAccessor.RequestContext.HydrationSyncKey,
                _servicesSettings.HydrationSecurityKey);
        }

        public HydrationController(IHydrationService hydrationService, ServicesSettings servicesSettings, IRequestContextAccessor requestContextAccessor)
        {
            _hydrationService = hydrationService;
            _servicesSettings = servicesSettings;
            _requestContextAccessor = requestContextAccessor;
        }

        [HttpPost("TimeTracks")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> InsertTimeTracks([FromBody] List<TimeTrack> timeTracks)
        {
            if (!ValidateAuthorization()) return Unauthorized();
            
            var response = await _hydrationService.InsertTimeTracksAsync(timeTracks);

            if (response == DbResponse.Error) return StatusCode((int)HttpStatusCode.InternalServerError);

            return Ok();
        }

        [HttpPost("TimeTracks/{id}"), HttpPut("TimeTracks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateTimeTrack(Guid id, [FromBody] TimeTrack timeTrack)
        {
            if (!ValidateAuthorization()) return Unauthorized();

            var response = await _hydrationService.UpdateTimeTrackAsync(id, timeTrack);

            if (response == DbResponse.Error) return StatusCode((int)HttpStatusCode.InternalServerError);

            return Ok();
        }

    }
}
