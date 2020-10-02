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
            player.creationTime = DateTime.Now;
            player.type = Type.player;
            player.score = 0;
            player.health = 50;
            player.level = 1;
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
            for (int y = map_h - 1; y >= 0; y--)
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

        public async Task<IMapObject> MovePlayer(string mapId, string playerId, Direction dir)
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
            int[] temp;
            switch (dir)
            {
                case Direction.up:
                    o = checkOutOfBounds(map, playerPosition, 0, 1);

                    temp = new int[] { playerPosition[0], playerPosition[1] + 1 };
                    if (canMove((Player)p, o, ref map, temp))
                    {
                        playerPosition[1]++;
                        map.postitions[playerId] = playerPosition;

                        var update = Builders<Map>.Update.
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1] - 1].obj, null).
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1]].obj, p).
                            Set(m => m.postitions, map.postitions);

                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return p;
                    }
                    else
                    {
                        var update = Builders<Map>.Update.Set(m => m.tiles[playerPosition[0]][playerPosition[1] + 1].obj, map.tiles[playerPosition[0]][playerPosition[1] + 1].obj);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return map.tiles[playerPosition[0]][playerPosition[1] + 1].obj;
                    }

                case Direction.right:
                    o = checkOutOfBounds(map, playerPosition, 1, 0);

                    temp = new int[] { playerPosition[0] + 1, playerPosition[1] };
                    if (canMove((Player)p, o, ref map, temp))
                    {
                        playerPosition[0]++;
                        map.postitions[playerId] = playerPosition;

                        var update = Builders<Map>.Update.
                            Set(m => m.tiles[playerPosition[0] - 1][playerPosition[1]].obj, null).
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1]].obj, p).
                            Set(m => m.postitions, map.postitions);

                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return p;
                    }
                    else
                    {
                        var update = Builders<Map>.Update.Set(m => m.tiles[playerPosition[0] + 1][playerPosition[1]].obj, map.tiles[playerPosition[0] + 1][playerPosition[1]].obj);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return map.tiles[playerPosition[0] + 1][playerPosition[1]].obj;
                    }

                case Direction.down:
                    o = checkOutOfBounds(map, playerPosition, 0, -1);

                    temp = new int[] { playerPosition[0], playerPosition[1] - 1 };
                    if (canMove((Player)p, o, ref map, temp))
                    {
                        playerPosition[1]--;
                        map.postitions[playerId] = playerPosition;

                        var update = Builders<Map>.Update.
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1] + 1].obj, null).
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1]].obj, p).
                            Set(m => m.postitions, map.postitions);

                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return p;
                    }
                    else
                    {
                        var update = Builders<Map>.Update.Set(m => m.tiles[playerPosition[0]][playerPosition[1] - 1].obj, map.tiles[playerPosition[0]][playerPosition[1] - 1].obj);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return map.tiles[playerPosition[0]][playerPosition[1] - 1].obj;
                    }

                case Direction.left:
                    o = checkOutOfBounds(map, playerPosition, -1, 0);
                    temp = new int[] { playerPosition[0] - 1, playerPosition[1] };
                    if (canMove((Player)p, o, ref map, temp))
                    {
                        playerPosition[0]--;
                        map.postitions[playerId] = playerPosition;

                        var update = Builders<Map>.Update.
                            Set(m => m.tiles[playerPosition[0] + 1][playerPosition[1]].obj, null).
                            Set(m => m.tiles[playerPosition[0]][playerPosition[1]].obj, p).
                            Set(m => m.postitions, map.postitions);

                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return p;
                    }
                    else
                    {
                        var update = Builders<Map>.Update.Set(m => m.tiles[playerPosition[0] - 1][playerPosition[1]].obj, map.tiles[playerPosition[0] - 1][playerPosition[1]].obj);
                        var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
                        await _mapCollection.UpdateOneAsync(filter, update);
                        return map.tiles[playerPosition[0] - 1][playerPosition[1]].obj;
                    }
            }
            return p;

        }
        public IMapObject checkOutOfBounds(Map map, int[] playerPosition, int x, int y)
        {
            int size = map.tiles[0].Length;
            if (x < size && y < size && x >= 0 && y >= 0)
            {
                if (map.tiles[playerPosition[0] + x][playerPosition[1] + y] != null)
                {
                    return map.tiles[playerPosition[0] + x][playerPosition[1] + y].obj;
                }
                else
                {
                    return null;
                }
            }
            throw new OutOfBoundsException();

        }
        public bool canMove(Player p, IMapObject o, ref Map map, int[] objPos)
        {
            if (o != null)
            {
                switch (o.type)
                {
                    case Type.enemy:
                    case Type.player:
                        ICharacter enemy = (ICharacter)o;
                        enemy.health -= p.damage;
                        if (enemy.health <= 0)
                        {
                            p = (Player)LevelUp(p);
                            map.tiles[objPos[0]][objPos[1]].obj = null;
                            return true;
                        }
                        map.tiles[objPos[0]][objPos[1]].obj = enemy;
                        return false;
                    case Type.item:
                        p.list_items.Append(o);
                        return true;
                }
            }
            return true;
        }

        public ICharacter LevelUp(Player player)
        {
            player.level++;
            player.score += 135;
            player.health += 10;
            return player;
        }
    }

}
