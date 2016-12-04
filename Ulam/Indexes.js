db.map.createIndex({ LongValue: -1 }, { background: true, unique: true });
db.primes.createIndex({ LongValue: 1 }, { background: true, unique: true });
db.primes.createIndex({ StringValue: 1 }, { background: true, unique: true });
