using MongoDB.Bson.Serialization.Attributes;

namespace Ulam
{
    public class Cordinate
    {
        [BsonElement]
        public long X;
        [BsonElement]
        public long Y;
    }

    [BsonIgnoreExtraElements]
    public class UlamElement
    {
        public UlamElement(long p, long x, long y, bool isPrime)
        {
            Location = new Cordinate
            {
                X = x,
                Y = y
            };
            Value = p;
            IsPrime = isPrime;
        }

        public UlamElement(long p, long x, long y)
        {
            Location = new Cordinate
            {
                X = x,
                Y = y
            };
            Value = p;
            IsPrime = p % 2 == 1 && p % 3 != 0;
        }

        [BsonId]
        public long Value { get; set; }

        [BsonElement]
        public Cordinate Location { get; set; }

        [BsonElement]
        public bool IsPrime { get; set; }
    }
}
