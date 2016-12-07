db.map.drop();
db.map.createIndex({ IsPrime: -1, _id: 1 }, { unique: true, background: true });
//db.map.createIndex({ Location: 1 }, { unique: true, background: true });