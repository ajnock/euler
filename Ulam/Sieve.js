function getNextPrime(p) {
    var obj = db.map.find({
        $and: [
        { IsPrime: true },
        { _id: { $gt: p } }
        ]
    }).sort({ _id: 1 }).limit(1);

    if (obj) {
        return obj[0]._id;
    } else {
        return null;
    }
}

// start sieving at 3 because that's
var prime = getNextPrime(1);
while (prime != null && prime <= 9223372036854775807 / 3) {
    print("Sieving " + prime);

    db.map.updateMany({
        $and: [
            { IsPrime: true },
            { _id: { $gt: prime } },
            { _id: { $mod: [prime, 0] } }
        ]
    },
        { $set: { IsPrime: false } });

    db.primes.insert({ _id: prime });

    prime = getNextPrime(prime);
}
