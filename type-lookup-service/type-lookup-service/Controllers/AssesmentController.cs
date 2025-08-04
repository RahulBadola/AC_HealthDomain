using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using type_lookup_service.Model;
using type_lookup_service.Services;

namespace type_lookup_service.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AssesmentController : ControllerBase
    {
        private readonly IAssessmentTemplateService _assesmentTemplateService;

        public AssesmentController(IAssessmentTemplateService assesmentTemplateService)
        {
            _assesmentTemplateService = assesmentTemplateService;
        }

        [HttpGet]
        [Route("{assessmentTemplateId:guid}")]
        [ProducesResponseType(typeof(AssessmentTemplate), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AssessmentTemplate>> GetAssessmentTemplateAsync(Guid assessmentTemplateId)
        {
            var result = await _assesmentTemplateService.GetAssessmentTemplateById(assessmentTemplateId);

            if (result == null)
                return NotFound();
            else if (result.HasError)
                return StatusCode((int)HttpStatusCode.InternalServerError);
            else
                return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(AssessmentTemplate), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AssessmentTemplate>> GetAssessmentTemplatesAsync()
        {
            var result = await _assesmentTemplateService.GetAssessmentTemplates();

            return Ok(result);
        }
    }
}
