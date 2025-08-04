using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using type_lookup_service.Model;
using type_lookup_service.Models;
using type_lookup_service.Services;
using type_lookup_service.Controllers;

namespace type_lookup_service.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationService _medicationService;
        public MedicationController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        [HttpPost]
        [Route("Medications/Search")]
        [ProducesResponseType(typeof(List<Medication>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<List<Medication>>> GetMedications([FromBody] MedicationSearchRequest medications)
        {

            var (response, data) =  await _medicationService.GetMedications(medications);

            return this.HandleDbResponse(response, data);
        }
        [HttpGet]
        [Route("{medicationId:guid}")]
        [ProducesResponseType(typeof(Medication), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Medication>> GetMedication(Guid medicationId)
        {
            var (response, data) = await _medicationService.GetMedicationById(medicationId);

            return this.HandleDbResponse(response, data);
        }
    }
}
