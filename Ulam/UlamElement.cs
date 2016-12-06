using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

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
        public UlamElement(long p, long x, long y, bool isPrime = true)
        {
            Location = new Cordinate
            {
                X = x,
                Y = y
            };
            Value = p;
            IsPrime = isPrime && p % 2 == 1 && p % 3 != 0;
        }

        [BsonId]
        public ObjectId Id { get; set; }


        [BsonElement]
        public long Value { get; set; }

        [BsonElement]
        public Cordinate Location { get; set; }

        [BsonElement]
        public bool IsPrime { get; set; }

        [BsonIgnore]
        public DateTime CreatedAt
        {
            get
            {
                return Id.CreationTime.ToLocalTime();
            }
        }
    }
}
