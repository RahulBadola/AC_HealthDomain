using time_track_dshp.Models.Dto;
using time_track_dshp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace time_track_dshp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncBackController : ControllerBase
    {
        private readonly ILogger<SyncBackController> _logger;
        private readonly ISyncBackService _syncBackService;

        public SyncBackController(ILogger<SyncBackController> logger, ISyncBackService syncBackService)
        {
            _logger = logger;
            _syncBackService = syncBackService;
        }

        [HttpPost("TimeTrack")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SyncTimeTrackAsync(ChangeSet<TimeTrack> timeTracks)
        {
            await _syncBackService.PublishSyncBackRecordAsync("TimeTrack", JsonConvert.SerializeObject(timeTracks));
            _logger.LogDebug("timeTracks syncBack record published.");
            return Ok();
        }

        [HttpPost("TimeTrackActivityMap")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SyncTimeTrackActivityMapAsync(ChangeSet<TimeTrackActivityMap> timeTrackActivityMaps)
        {
            await _syncBackService.PublishSyncBackRecordAsync("TimeTrackActivityMap", JsonConvert.SerializeObject(timeTrackActivityMaps));
            _logger.LogDebug("timeTrackActivityMaps syncBack record published.");
            return Ok();
        }
    }
}
