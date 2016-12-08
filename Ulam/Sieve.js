function getNextPrime(p) {
    var obj = db.getSiblingDB("Ulam").map.find({
        $and: [
        { IsPrime: true },
        { _id: { $gt: p } }
        ]
    }).sort({ _id: 1 }).limit(1);

    return obj;
}

// start sieving at 3 because that's
var prime = getNextPrime(1);
var max = db.getSiblingDB("Ulam").map.find().sort({ _id: -1 }).limit(1)._id;
while (prime != null && prime._id <= max) {
    print("Sieving " + prime);

    db.getSiblingDB("Ulam").map.updateMany({
        $and: [
            { IsPrime: true },
            { _id: { $gt: prime._id } },
            { _id: { $mod: [prime._id, 0] } }
        ]
    },
        { $set: { IsPrime: false } });

    db.getSiblingDB("Ulam").primes.insert(prime);

    prime = getNextPrime(prime._id);
}
