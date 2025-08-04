using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using type_lookup_service.Model;
using type_lookup_service.Services;

namespace type_lookup_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly IDataService _data;
        public LookupController(IDataService data)
        {
            _data = data;
        }

        [HttpPost]
        public async Task<object> GetData([FromBody] SearchModel searchModel)
        {
            var result = await _data.GetLookupDataAsync(searchModel);

            return result;
        }
    }
}
