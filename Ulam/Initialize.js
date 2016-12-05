db.map.drop();
db.primes.drop();

db.map.createIndex({ IsPrime: -1, Value: 1 }, { background: true, unique: true });
db.primes.createIndex({ Value: 1 }, { background: true, unique: true });
