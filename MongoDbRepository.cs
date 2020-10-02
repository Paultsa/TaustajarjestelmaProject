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
                postitions = new Dictionary<string, int[]>(),
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

            var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
            if (filter == null)
            {
                throw new NotImplementedException();
            }

            map.postitions.Add(player.id, new int[2] { randomX, randomY });
            var update = Builders<Map>.Update.Set(m => m.tiles[randomX][randomY].obj, player).Set(m => m.postitions, map.postitions);
            Console.WriteLine(map.postitions[player.id][0] + ", " + map.postitions[player.id][1]);
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
            for (int y = map_h; y > 0; y--)
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

        public async Task<Player> MovePlayer(string mapId, string playerId, Direction dir)
        {
            Map map = await FindMap(mapId);
            int[] playerPosition;
            if (map.postitions.ContainsKey(playerId))
            {
                playerPosition = map.postitions[playerId];
            }
            else
            {
                //Exception here
                Console.WriteLine("Exception135");
                return null;
            }
            IMapObject p = map.tiles[playerPosition[0]][playerPosition[1]].obj;
            IMapObject o;
            switch (dir)
            {
                case Direction.up:
                    o = checkOutOfBounds(map, playerPosition, 0, 1);
                    if (canMove((Player)p, o))
                    {
                        map.tiles[playerPosition[0]][playerPosition[1]].obj = null;
                        map.postitions[playerId][1]++;
                        var update = Builders<Map>.Update.
                        Set(m => map.tiles[playerPosition[0]][playerPosition[1]].obj, null).
                        Set(m => m.tiles[playerPosition[0]][playerPosition[1] + 1].obj, p);//.
                                                                                           //Set(m => m.postitions, map.postitions);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return (Player)p;
                    }
                    else
                    {

                    }
                    break;

                case Direction.right:
                    o = checkOutOfBounds(map, playerPosition, 1, 0);
                    if (canMove((Player)p, o))
                    {
                        map.tiles[playerPosition[0]][playerPosition[1]].obj = null;
                        map.postitions[playerId][0]++;
                        var update = Builders<Map>.Update.
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1]].obj, null).
                            Set(m => m.tiles[playerPosition[0] + 1][playerPosition[1]].obj, p);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return (Player)p;
                    }
                    else
                    {

                    }
                    break;

                case Direction.down:
                    o = checkOutOfBounds(map, playerPosition, 0, -1);
                    if (canMove((Player)p, o))
                    {
                        map.tiles[playerPosition[0]][playerPosition[1]].obj = null;
                        map.postitions[playerId][1]--;
                        var update = Builders<Map>.Update.
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1]].obj, null).
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1] - 1].obj, p);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return (Player)p;
                    }
                    else
                    {

                    }
                    break;

                case Direction.left:
                    o = checkOutOfBounds(map, playerPosition, -1, 0);
                    if (canMove((Player)p, o))
                    {
                        map.tiles[playerPosition[0]][playerPosition[1]].obj = null;
                        map.postitions[playerId][0]--;
                        var update = Builders<Map>.Update.
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1]].obj, null).
                            Set(m => m.tiles[playerPosition[0] - 1][playerPosition[1]].obj, p);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return (Player)p;

                    }
                    else
                    {

                    }
                    break;

            }
            Console.WriteLine("Exception226");
            return null;

        }
        public IMapObject checkOutOfBounds(Map map, int[] playerPosition, int x, int y)
        {
            if (map.tiles[playerPosition[0] + x][playerPosition[1] + y] != null)
            {
                return map.tiles[playerPosition[0] + x][playerPosition[1] + y].obj;
            }
            else
            {
                //OutOfBoundsException
                return null;
            }
        }
        public bool canMove(Player p, IMapObject o)
        {
            if (o != null)
            {
                switch (o.type)
                {
                    case Type.enemy:
                        return false;
                    case Type.item:
                        p.list_items.Append(o);
                        return true;
                    case Type.player:
                        return false;
                }
            }

            return true;
        }
    }

}
