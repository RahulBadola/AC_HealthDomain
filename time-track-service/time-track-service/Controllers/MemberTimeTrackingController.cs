using AssureCare.MedCompass.DataAuthorization.Filters;
using AssureCare.MedCompass.DataAuthorization.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Dto.Legacy;
using time_track_service.Model.Legacy;
using time_track_service.Services;

namespace time_track_service.Controllers
{
    [Produces("application/json")]
    [Route("api/members/{memberGuid:guid}/timetracking")]
    public class MemberTimeTrackingController : ControllerBase
    {
        private readonly IMemberTimeTrackingService _memberTimeTrackingService;

        public MemberTimeTrackingController(IMemberTimeTrackingService memberTimeTrackingService)
        {
            _memberTimeTrackingService = memberTimeTrackingService;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(LegacyMemberTimeTrackingsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [RequireOperation(typeof(LegacyMemberTimeTrackingModel), Operation.Get)]
        public async Task<ActionResult<LegacyMemberTimeTrackingsModel>> GetAsync(Guid memberGuid)
        {
            var (response, data) = await _memberTimeTrackingService.GetTimeTrackingsAsync(memberGuid);
            return this.HandleDbResponse(response, data);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(LegacyMemberTimeTrackingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [RequireOperation(typeof(LegacyMemberTimeTrackingModel), Operation.Get)]
        public async Task<ActionResult<LegacyMemberTimeTrackingModel>> GetByIdAsync(Guid memberGuid, Guid id)
        {
            var (response, data) = await _memberTimeTrackingService.GetTimeTrackingAsync(id);
            return this.HandleDbResponse(response, data);

        }

        [HttpPost]
        [ProducesResponseType(typeof(LegacyMemberTimeTrackingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<LegacyMemberTimeTrackingModel>> PostAsync(Guid memberGuid, [FromBody] LegacyMemberTimeTrackingModel value)
        {
            var (response, data) = await _memberTimeTrackingService.SaveTimeTrackingAsync(memberGuid, value);
            return this.HandleDbResponse(response, data);
        }

        [HttpPost,HttpPut, Route("{id:guid}")]
        [ProducesResponseType(typeof(LegacyMemberTimeTrackingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<LegacyMemberTimeTrackingModel>> PutAsync(Guid memberGuid, Guid id, [FromBody] LegacyMemberTimeTrackingModel value)
        {
            value.Id = id;
            var (response, data) = await _memberTimeTrackingService.SaveTimeTrackingAsync(memberGuid, value);
            return this.HandleDbResponse(response, data);
        }
    }
}
