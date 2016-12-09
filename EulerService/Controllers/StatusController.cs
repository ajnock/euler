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
            var totalCount = _repo.Count();
            var primesCount = _repo.PrimesCount();
            var largestNumber = _repo.LargestNumber();
            var largestPrime = _repo.LargestPrime();
            var newest = _repo.NewestNumber();

            var content = new
            {
                PrimesCount = await primesCount,
                TotalCount = await totalCount,
                Newest = await newest,
                LargestPrime = await largestPrime,
                LargestNumber = await  largestNumber
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
            var content = await _repo.NewestNumber();

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
