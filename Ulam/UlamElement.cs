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

        [BsonIgnore]
        internal ulong Value { get; private set; }

        public UlamElement()
        {
        }

        public UlamElement(ulong p, long x, long y, bool isPrime = false)
        {
            Location = new Cordinate()
            {
                X = x,
                Y = y
            };
            Value = p;
            IsPrime = isPrime;
        }

        [BsonElement]
        public string StringValue
        {
            get
            {
                return Value.ToString();
            }
        }

        [BsonElement]
        public long LongValue
        {
            get
            {
                return Value.ToLong();
            }
            set
            {
                Value = value.ToULong();
            }
        }

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
