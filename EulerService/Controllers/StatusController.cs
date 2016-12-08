using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Http;
using EulerService.Models;

namespace EulerService.Controllers
{
    public class StatusController : ApiController
    {
        private readonly UlamElementRepository _repo;
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public StatusController()
        {
            _repo = new UlamElementRepository();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Index()
        {
            var count = _repo.Count();
            var top = _repo.Newest();

            var content = new
            {
                Count = await count,
                Top = await top
            };

            return Json(content, SerializerSettings);
        }

        [HttpGet]
        [Route("api/status/count")]
        public async Task<IHttpActionResult> Count()
        {
            var content = await _repo.Count();

            return Json(content, SerializerSettings);
        }

        [HttpGet]
        [Route("api/status/{id}")]
        public async Task<IHttpActionResult> Index(long id)
        {
            var content = await _repo.Find(id);

            return Json(content, SerializerSettings);
        }

        [HttpGet]
        [Route("api/status/top")]
        public async Task<IHttpActionResult> Top()
        {
            var content = await _repo.Newest();

            return Json(content, SerializerSettings);
        }

        [HttpGet]
        [Route("api/status/prime")]
        public async Task<IHttpActionResult> Prime()
        {
            var content = await _repo.LargestPrime();

            return Json(content, SerializerSettings);
        }
    }
}
