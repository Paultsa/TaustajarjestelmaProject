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
                tiles = new MapTile[size][],
                id = name
            };
            for (int x = 0; x < size; x++)
            {
                map.tiles[x] = new MapTile[size];
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
            player.id = Guid.NewGuid().ToString();
            Map map = await FindMap(mapId);
            Random rnd = new Random();
            int randomX = rnd.Next(0, map.tiles.Length);
            int randomY = rnd.Next(0, map.tiles.Length);

            var filter = Builders<Map>.Filter.Eq(m => m.tiles[randomX][randomY].obj, null);
            if (filter == null)
            {
                throw new NotImplementedException();
            }
            var update = Builders<Map>.Update.Set(m => m.tiles[randomX][randomY].obj, player);

            await _mapCollection.UpdateOneAsync(filter, update);
            return player;
        }

        public async Task<string[,]> PrintMap(string map_id)
        {
            var current_map = await FindMap(map_id);
            var map_w = current_map.tiles.Length;
            var map_h = current_map.tiles.Length;

            string[,] map = new string[map_w, map_h];

            //Get map data
            for (int y = 0; y < map_h; y++)
            {
                for (int x = 0; x < map_w; x++)
                {
                    IMapObject prop = current_map.tiles[x][y].obj;
                    if (prop != null)
                    {
                        switch (prop.type)
                        {
                            case Type.player: map[x, y] = "@  "; break;
                            case Type.enemy: map[x, y] = "#  "; break;
                            case Type.item: map[x, y] = "¤  "; break;

                        }
                    }
                    else
                    {
                        map[x, y] = ".  ";
                    }
                }
            }

            Console.WriteLine();
            //Print map to console
            for (int y = 0; y < map_h; y++)
            {

                for (int x = 0; x < map_w; x++)
                {
                    string content = map[x, y];
                    Console.Write(content);

                    if (x == map_w - 1)
                    {
                        Console.Write("|" + (y + 1));
                    }
                }
                Console.WriteLine();
            }

            for (int x = 0; x < map_w; x++)
            {
                Console.Write((x + 1) + "  ");
            }

            return null;
        }
    }

}
