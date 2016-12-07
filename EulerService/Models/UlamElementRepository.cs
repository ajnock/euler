using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Ulam;

namespace EulerService.Models
{
    public class UlamElementRepository
    {
        private readonly IMongoCollection<UlamElement> _map;
        private readonly IMongoCollection<BsonDocument> _primes;

        public UlamElementRepository()
        {
            var client = new MongoClient();
            _map = client.GetDatabase("Ulam").GetCollection<UlamElement>("map");
            _primes = client.GetDatabase("Ulam").GetCollection<BsonDocument>("primes");
        }

        public async Task<UlamElement> Newest()
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

        public async Task<UlamElement> Find(long id)
        {
            var cursor = await _map.FindAsync(new JsonFilterDefinition<UlamElement>("{ _id : " + id + " }"));
            var element = await cursor.FirstOrDefaultAsync();

            return element;
        }

        public async Task<long> LargestPrime()
        {
            var options = new FindOptions<BsonDocument>
            {
                Limit = 1,
                Sort = new JsonSortDefinition<BsonDocument>("{ _id : -1 }")
            };

            var cursor = await _primes.FindAsync(JsonFilterDefinition<BsonDocument>.Empty, options);
            var last = await cursor.FirstOrDefaultAsync();

            return last["_id"].AsInt64;
        }
    }
}