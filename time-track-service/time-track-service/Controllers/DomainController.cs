using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Services;
using time_track_service.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AssureCare.MedCompass.DataAuthorization.Filters;
using AssureCare.MedCompass.DataAuthorization.Models;

namespace time_track_service.Controllers
{

    [Route("api/domain")]
    [ApiController]
    [FieldAuthorization, OperationAuthorization, RowAuthorization]
    public class DomainController : ControllerBase
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService, IRequestContextAccessor requestContextAccessor)
        {
            _domainService = domainService;
        }

        [HttpPost("TimeTracks")]
        [ProducesResponseType(typeof(TimeTrack), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequireOperation(typeof(TimeTrack), Operation.Add)]
        public async Task<ActionResult<TimeTrack>> CreateTimeTrackAsync([FromBody] TimeTrack Id)
        {

            var (response, data) = await _domainService.InsertTimeTrackAsync(Id);
            return this.HandleDbResponse(response, data);
        }

        [HttpGet("TimeTracks")]
        [ProducesResponseType(typeof(IEnumerable<TimeTrack>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequireOperation(typeof(TimeTrack), Operation.Get)]
        public async Task<ActionResult<IEnumerable<TimeTrack>>> ReadTimeTracksAsync([FromQuery] bool includeInactive)
        {
            var (response, data) = await _domainService.ReadTimeTracksAsync(includeInactive);
            return this.HandleDbResponse(response, data);

        }

        [HttpGet("TimeTracks/{id}")]
        [ProducesResponseType(typeof(TimeTrack), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequireOperation(typeof(TimeTrack), Operation.Get)]
        public async Task<ActionResult<TimeTrack>> ReadTimeTrackByIdAsync(Guid id)
        {
            var (response, data) = await _domainService.ReadTimeTrackByIdAsync(id);
            return this.HandleDbResponse(response, data);
        }

        [HttpPost("TimeTracks/{id}"), HttpPut("TimeTracks/{id}")]
        [ProducesResponseType(typeof(TimeTrack), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequireOperation(typeof(TimeTrack), Operation.Update)]
        public async Task<ActionResult<TimeTrack>> UpdateTimeTrackAsync(Guid id, [FromBody] TimeTrack timeTrack)
        {
            var (response, data) = await _domainService.UpdateTimeTrackByIdAsync(id, timeTrack);
            return this.HandleDbResponse(response, data);
        }

        [HttpDelete("TimeTracks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [RequireOperation(typeof(TimeTrack), Operation.Void)]
        public async Task<IActionResult> DeleteTimeTrackAsync(Guid id, Guid voidReasonId)
        {
            var response = await _domainService.VoidTimeTrackByIdAsync(id, voidReasonId);
            return this.HandleDbResponse(response);

        }

    }
}