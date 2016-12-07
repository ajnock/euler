db.map.drop();
db.map.createIndex({ Value: 1 }, { unique: true, background: true });
db.map.createIndex({ IsPrime: -1, Value: 1 }, { unique: true, background: true });
//db.map.createIndex({ Location: 1 }, { unique: true, background: true });