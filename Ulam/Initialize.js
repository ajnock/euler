db.map.drop();
db.map.createIndex({ Value: 1 }, { unique: true, background : true });
