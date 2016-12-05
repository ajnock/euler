using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EulerMath;

namespace Ulam
{
    public class Cordinate
    {
        public long X { get; set; }
        public long Y { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class UlamElement
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public UlamElement()
        {
        }

        public UlamElement(long p, long x, long y, bool isPrime = true)
        {
            Location = new Cordinate()
            {
                X = x,
                Y = y
            };
            Value = p;
            IsPrime = isPrime && p % 2 == 1;
        }

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
