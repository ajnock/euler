using MongoDB.Driver;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Ulam;

namespace EulerService.Controllers
{
    public class StatusController : ApiController
    {
        private readonly IMongoClient _client;

        public StatusController()
        {
            _client = new MongoClient();
        }

        public StatusController(IMongoClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<JsonResult<UlamElement>> Index()
        {
            var map = _client.GetDatabase("Ulam").GetCollection<UlamElement>("map");
            var options = new FindOptions<UlamElement>()
            {
                Limit = 1,
                Sort = new JsonSortDefinition<UlamElement>("{ _id : -1 }")
            };
            using (var cursor = await map.FindAsync(JsonFilterDefinition<UlamElement>.Empty, options))
            {
                var last = await cursor.FirstOrDefaultAsync();

                JsonSerializerSettings serializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
                return Json(last, serializerSettings);
            }
        }
    }
}
