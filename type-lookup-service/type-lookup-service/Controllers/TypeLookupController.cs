using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using type_lookup_service.Services;

namespace type_lookup_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeLookupController : ControllerBase
    {
        private readonly IDataService _data;
        public TypeLookupController(IDataService data)
        {
            _data = data;
        }

        [HttpGet]
        public JsonResult GetMany([FromHeader] List<string> typeNames)
        {
            var result = _data.GetDataForList(typeNames);

            return new JsonResult(result);
        }
    }
}
