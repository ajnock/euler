using System.Threading.Tasks;
using MongoDB.Driver;
using Ulam;

namespace EulerService.Models
{
    public class UlamElementRepository
    {
        private readonly IMongoCollection<UlamElement> _map;

        public UlamElementRepository()
        {
            var client = new MongoClient();
            _map = client.GetDatabase("Ulam").GetCollection<UlamElement>("map");
        }

        public async Task<UlamElement> NewestNumber()
        {
            var options = new FindOptions<UlamElement>
            {
                Limit = 1,
                Sort = new JsonSortDefinition<UlamElement>("{ _id : -1 }")
            };

            var cursor = await _map.FindAsync(JsonFilterDefinition<UlamElement>.Empty, options);
            var last = await cursor.FirstOrDefaultAsync();

            return last;
        }

        public async Task<long> Count()
        {
            var count = await _map.CountAsync(FilterDefinition<UlamElement>.Empty);
            return count;
        }

        public async Task<long> PrimesCount()
        {
            var filter = new JsonFilterDefinition<UlamElement>("{ IsPrime : true }");
            var count = await _map.CountAsync(filter);
            return count;
        }

        public async Task<UlamElement> Find(long id)
        {
            var cursor = await _map.FindAsync(new JsonFilterDefinition<UlamElement>("{ Value : " + id + " }"));
            var element = await cursor.FirstOrDefaultAsync();

            return element;
        }

        public async Task<UlamElement> LargestPrime()
        {
            var filter = new JsonFilterDefinition<UlamElement>("{ IsPrime : true }");
            var options = new FindOptions<UlamElement>
            {
                Limit = 1,
                Sort = new JsonSortDefinition<UlamElement>("{ Value : -1 }")
            };

            var cursor = await _map.FindAsync(filter, options);
            var last = await cursor.FirstOrDefaultAsync();

            return last;
        }

        public async Task<UlamElement> LargestNumber()
        {
            var filter = JsonFilterDefinition<UlamElement>.Empty;
            var options = new FindOptions<UlamElement>
            {
                Limit = 1,
                Sort = new JsonSortDefinition<UlamElement>("{ Value : -1 }")
            };

            var cursor = await _map.FindAsync(filter, options);
            var last = await cursor.FirstOrDefaultAsync();

            return last;
        }
    }
}