function getNextPrime(p) {
    var obj = db.map.find({
        $and: [
        { IsPrime: true },
        { Value: { $gt: p } }
        ]
    }).sort({ Value: 1 }).limit(1);

    if (obj) {
        return obj[0].Value;
    } else {
        return null;
    }
}

// create the query index
db.map.createIndex({ Value: 1 }, { unique: true });
// create new index which will also help us in our queries
db.map.createIndex({ IsPrime: -1, Value: 1 }, { unique: true });

// start sieving at 3 because that's
var prime = getNextPrime(2);
while (prime != null && prime <= 9223372036854775807 / 3) {
    print("Sieving " + prime)

    db.map.updateMany({
        $and: [
        { IsPrime: true },
        { Value: { $gt: prime } },
        { Value: { $mod: [prime, 0] } }
        ]
    }, { $set: { IsPrime: false } })

    prime = getNextPrime(prime);
}

