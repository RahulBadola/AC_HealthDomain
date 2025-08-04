using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using type_lookup_service.Model;
using type_lookup_service.Services;

namespace type_lookup_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericLookupController : ControllerBase
    {
        private readonly IDataService _data;
        public GenericLookupController(IDataService data)
        {
            _data = data;
        }

        [HttpGet]
        public JsonResult GetMany([FromHeader] List<string> typeNames)
        {
            var result = _data.GetDataForListMap(typeNames);

            return new JsonResult(result);
        }

        [HttpPost]
        public JsonResult GetById([FromBody] List<SearchModel> searchModel)
        {
            var result = _data.GetTypeDataById(searchModel);

            return new JsonResult(result);
        }
    }
}
