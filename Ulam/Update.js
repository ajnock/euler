var minPrime = 2;
var prime = db.map.find({
    $and: [
    { IsPrime: true },
    { Value: { $gt: minPrime } }
    ]
}).sort({ Value: 1 }).limit(1).toArray()[0];

db.map.updateMany({
    $and: [
    { IsPrime: true },
    { Value: { $gt: prime.Value } },
    { Value: { $mod: [prime.Value, 0] } }
    ]
}, { $set: { IsPrime: false } })


