using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using time_track_service.Model;
using time_track_service.Model.Legacy;
using time_track_service.Services;
using AssureCare.MedCompass.DataAuthorization.Filters;
using AssureCare.MedCompass.DataAuthorization.Models;
using time_track_service.Model.Dto.Legacy;

namespace time_track_service.Controllers
{
    [Produces("application/json")]
    [Route("api/staff/{staffGuid:guid}/timetracking")]
    [ApiController]
    public class StaffTimeTrackingController : ControllerBase
    {
        private readonly IStaffTimeTrackingService _staffTimeTrackingService;

        public StaffTimeTrackingController(IStaffTimeTrackingService staffTimeTrackingService)
        {
            _staffTimeTrackingService = staffTimeTrackingService;

        }

        [HttpGet]
        [ProducesResponseType(typeof(LegacyTimeTrackingsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [RequireOperation(typeof(LegacyTimeTrackingModel), Operation.Get)]
        public async Task<ActionResult<LegacyTimeTrackingsModel>> GetAsync(Guid staffGuid)
        {
            var (response, data) = await _staffTimeTrackingService.GetTimeTrackingsAsync(staffGuid);
            return this.HandleDbResponse(response, data);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(LegacyTimeTrackingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [RequireOperation(typeof(LegacyTimeTrackingModel), Operation.Get)]
        public async Task<ActionResult<LegacyTimeTrackingModel>> GetByIdAsync(Guid staffGuid, Guid id)
        {
            var (response, data) = await _staffTimeTrackingService.GetTimeTrackingAsync(id);
            return this.HandleDbResponse(response, data);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LegacyTimeTrackingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<LegacyTimeTrackingModel>> PostAsync(Guid staffGuid, [FromBody] LegacyTimeTrackingModel value)
        {
            var (response, data) = await _staffTimeTrackingService.SaveTimeTrackingAsync(staffGuid, value);
            return this.HandleDbResponse(response, data);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(LegacyTimeTrackingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<LegacyTimeTrackingModel>> PutAsync(Guid staffGuid, Guid id, [FromBody] LegacyTimeTrackingModel value)
        {
            value.Id = id;
            return await PostAsync(staffGuid, value);
            
        }

        [HttpGet]
        [Route("config/startenddaterequired")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<bool>> RequireStartAndEndDatesAsync(string staffGuid)
        {
            var (response, data) = await _staffTimeTrackingService.IsRequireStartAndEndDatesAsync();
            return this.HandleDbResponse(response, data);
        }

        [HttpGet]
        [Route("config/serviceunits")]
        
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<int>> GetServiceUnitMaxAsync(string staffGuid)
        {
            var (response, data) = await _staffTimeTrackingService.SelectServiceUnitMaxValueAsync();
            return this.HandleDbResponse(response, data);
        }
    }
}
