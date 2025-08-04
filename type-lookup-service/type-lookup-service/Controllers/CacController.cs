using Microsoft.AspNetCore.Mvc;
using type_lookup_service.Services;

namespace type_lookup_service.Controllers
{
    [Route("api/cac")] // This must be called api/cac or nightswatch won't allow it to be called.
    [ApiController]
    public class CacController : ControllerBase
    {
        private readonly ConfigAsCodeService _configService;

        public CacController(ConfigAsCodeService configService)
        {
            _configService = configService;
        }

        // GET: api/config
        [HttpGet]
        public ContentResult Get()
        {
            return Content(_configService.LoadConfig(), "application/json");
        }
    }
}
