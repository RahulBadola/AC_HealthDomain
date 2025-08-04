using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Services;
using time_track_service.Utils;

namespace time_track_service.Controllers
{
    [Route("api/sync")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ISyncService _hydrationService;
        private readonly ServicesSettings _servicesSettings;
        private readonly IRequestContextAccessor _requestContextAccessor;

        private bool ValidateAuthorization()
        {
            return string.Equals(
                _requestContextAccessor.RequestContext.HydrationSyncKey,
                _servicesSettings.HydrationSecurityKey);
        }

        public SyncController(ISyncService hydrationService, ServicesSettings servicesSettings, IRequestContextAccessor requestContextAccessor)
        {
            _hydrationService = hydrationService;
            _servicesSettings = servicesSettings;
            _requestContextAccessor = requestContextAccessor;
        }

        [HttpPost("TimeTrack")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpsertTimeTrack([FromBody] List<TimeTrack> timeTrack, CancellationToken cancellationToken)
        {
            if (!ValidateAuthorization()) return Unauthorized();

            var response = await _hydrationService.UpsertAsync(timeTrack, cancellationToken);

            if (!response) return StatusCode((int)HttpStatusCode.InternalServerError);
            return Ok();
        }
    }
}
