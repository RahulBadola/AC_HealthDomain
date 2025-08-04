using Microsoft.AspNetCore.Mvc;
using System.Net;
using time_track_service.Model;
using time_track_service.Services;

namespace time_track_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadTestController : ControllerBase
    {
        private readonly ILoadTestDataService _loadTestDataService;
        private readonly ServicesSettings _servicesSettings;
        public LoadTestController(ILoadTestDataService loadTestDataService, ServicesSettings servicesSettings)
        {
            _loadTestDataService = loadTestDataService;
            _servicesSettings = servicesSettings;
        }
        [HttpPost("clean_CallLogCollection")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult CleanCollection()
        {
            if (!_servicesSettings.AllowLoadTestActions) return Forbid();

            _loadTestDataService.CleanCollection();

            return NoContent();
        }

        [HttpPost("hydrate_CallLogCollection")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public ActionResult HydrateCollection()
        {
            if (!_servicesSettings.AllowLoadTestActions) return Forbid();

            _loadTestDataService.PopulateTestData();

            return NoContent();
        }
    }
}
