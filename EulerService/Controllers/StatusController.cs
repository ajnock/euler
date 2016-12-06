using MongoDB.Driver;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using EulerService.Models;
using Ulam;

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
            var max = _repo.Max();

            var content = new
            {
                Count = await count,
                Top = await top,
                Max = await max
            };

            return Json(content, SerializerSettings);
        }

        [HttpGet]
        public async Task<JsonResult<UlamElement>> Top()
        {
            var content = await _repo.Newest();

            return Json(content, SerializerSettings);
        }

        [HttpGet]
        public async Task<JsonResult<UlamElement>> Max()
        {
            var content = await _repo.Max();

            return Json(content, SerializerSettings);
        }
    }
}
