using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using System.Collections.Generic;

namespace Project
{
    public class MongoDbRepository : IRepository
    {
        private readonly IMongoCollection<Map> _mapCollection;

        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection;

        public MongoDbRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("game");
            _mapCollection = database.GetCollection<Map>("maps");
            _bsonDocumentCollection = database.GetCollection<BsonDocument>("players");
        }

        public async Task<Map> FindMap(string mapId)
        {
            var mapFilter = Builders<Map>.Filter.Eq(m => m.id, mapId);
            return await _mapCollection.Find(mapFilter).FirstAsync();
        }
        public async Task<Map> CreateMap(int size, string name)
        {
            Map map = new Map()
            {
                tiles = new List<List<MapTile>>(),
                id = name
            };
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    map.tiles[x][y] = new MapTile();
                }
            }
            await _mapCollection.InsertOneAsync(map);
            return map;
        }
        public async Task<Player> CreatePlayer(string mapId, Player player)
        {
            Map map = await FindMap(mapId);
            Random rnd = new Random();
            int randomX = rnd.Next(0, map.tiles.Count());
            int randomY = rnd.Next(0, map.tiles.Count());

            var filter = Builders<Map>.Filter.Eq(m => m.tiles[randomX][randomY].entity, null);
            if (filter == null)
            {
                throw new NotImplementedException();
            }
            var update = Builders<Map>.Update.Set(m => m.tiles[randomX][randomY].entity, player);

            await _mapCollection.UpdateOneAsync(filter, update);
            return player;
        }

    }

}
