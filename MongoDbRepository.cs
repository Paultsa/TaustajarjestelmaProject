using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;

namespace Project
{
    public class MongoDbRepository : IRepository
    {

        private readonly IMongoCollection<Map> _mapCollection;

        private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection;

        public MongoDbRepository()
        {
            BsonClassMap.RegisterClassMap<Player>();
            BsonClassMap.RegisterClassMap<Item>();
            BsonClassMap.RegisterClassMap<Enemy>();

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
                id = name,
                playerCount = 0
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

            //Random position
            var a_positions = map.postitions.Values.ToArray();
            int randomX, randomY;
            bool found = false;

            while (true)
            {
                randomX = rnd.Next(0, map.tiles.Length);
                randomY = rnd.Next(0, map.tiles.Length);
                found = false;

                for (var i = 0; i < a_positions.Length; i++)
                {
                    if (
                    a_positions[i][0] == randomX &&
                    a_positions[i][1] == randomY
                    )
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) { break; }
            }


            var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
            map.postitions.Add(player.id, new int[2] { randomX, randomY });
            var update = Builders<Map>.Update.Set(m => m.tiles[randomX][randomY].obj, player).Set(m => m.postitions, map.postitions).
            Inc("playerCount", 1);    //Increment playerCount by one every time a player is added IF ISSUE REPLACE WITH: (m => m.playerCount, 1)
            Console.WriteLine(map.postitions[player.id][0] + ", " + map.postitions[player.id][1]);
            await _mapCollection.UpdateOneAsync(filter, update);
            return player;
        }

        public async Task<MapCount[]> GetMapPopulations()
        {
            var levelCounts =
            await _mapCollection.Aggregate()
                                .Project(m => new MapCount { Id = m.id, PlayerCount = m.playerCount })
                                .SortByDescending(l => l.PlayerCount).ToListAsync();

            //Debug
            for (var i = 0; i < levelCounts.Count(); i++)
            {
                Console.WriteLine("MapID: " + levelCounts[i].Id + "- Players: " + levelCounts[i].PlayerCount);
            }

            return levelCounts.ToArray();
        }
        public async Task<Player[]> GetPlayersWithMinLevel(string mapId, int minLevel)
        {
            Map map = await FindMap(mapId);
            var positions = map.postitions.Values.ToArray();
            List<Player> playersWithMinLevel = new List<Player>();

            foreach(var p in positions)
            {
                
                Player temp = (Player)map.tiles[p[0]][p[1]].obj;
                
                if(temp.level >= minLevel)
                {
                    playersWithMinLevel.Add(temp);
                }
            }
            return playersWithMinLevel.ToArray();
        }

        public async Task<Player[]> GetPlayers(string mapId)
        {
            Map map = await FindMap(mapId);
            var positions = map.postitions.Values.ToArray();
            List<Player> players = new List<Player>();

            foreach(var p in positions)
            {
                Player temp = (Player)map.tiles[p[0]][p[1]].obj;
                players.Add(temp);
            }
            return players.ToArray();
        }




        public async Task<Enemy> CreateEnemy(string mapId, Enemy enemy)
        {
            enemy.id = Guid.NewGuid().ToString();
            enemy.type = Type.enemy;

            Map map = await FindMap(mapId);
            Random rnd = new Random();

            //Random position
            var a_positions = map.postitions.Values.ToArray();
            int randomX, randomY;
            bool found = false;

            while (true)
            {
                randomX = rnd.Next(0, map.tiles.Length);
                randomY = rnd.Next(0, map.tiles.Length);
                found = false;

                for (var i = 0; i < a_positions.Length; i++)
                {
                    if (
                    a_positions[i][0] == randomX &&
                    a_positions[i][1] == randomY
                    )
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) { break; }
            }

            var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
            map.postitions.Add(enemy.id, new int[2] { randomX, randomY });
            var update = Builders<Map>.Update.Set(m => m.tiles[randomX][randomY].obj, enemy).Set(m => m.postitions, map.postitions);
            Console.WriteLine(map.postitions[enemy.id][0] + ", " + map.postitions[enemy.id][1]);
            await _mapCollection.UpdateOneAsync(filter, update);
            return enemy;
        }

        public async Task<Item> CreateItem(string mapId, Item item)
        {
            item.id = Guid.NewGuid().ToString();
            item.type = Type.item;

            Map map = await FindMap(mapId);
            Random rnd = new Random();

            //Random position
            var a_positions = map.postitions.Values.ToArray();
            int randomX, randomY;
            bool found = false;

            while (true)
            {
                randomX = rnd.Next(0, map.tiles.Length);
                randomY = rnd.Next(0, map.tiles.Length);
                found = false;

                for (var i = 0; i < a_positions.Length; i++)
                {
                    if (
                    a_positions[i][0] == randomX &&
                    a_positions[i][1] == randomY
                    )
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) { break; }
            }

            var filter = Builders<Map>.Filter.Eq(m => m.id, mapId);
            map.postitions.Add(item.id, new int[2] { randomX, randomY });
            var update = Builders<Map>.Update.Set(m => m.tiles[randomX][randomY].obj, item).Set(m => m.postitions, map.postitions);
            Console.WriteLine(map.postitions[item.id][0] + ", " + map.postitions[item.id][1]);
            await _mapCollection.UpdateOneAsync(filter, update);
            return item;
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
                throw new PlayerNotFoundException(playerId, mapId);
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
                        if (o != null && o.type == Type.player)
                        {
                            update = Builders<Map>.Update.Inc("playerCount", -1);
                            await _mapCollection.UpdateOneAsync(filter, update);
                        }
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
                        if (o != null && o.type == Type.player)
                        {
                            update = Builders<Map>.Update.Inc("playerCount", -1);
                            await _mapCollection.UpdateOneAsync(filter, update);
                        }
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
                        if (o != null && o.type == Type.player)
                        {
                            update = Builders<Map>.Update.Inc("playerCount", -1);
                            await _mapCollection.UpdateOneAsync(filter, update);
                        }
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
                        if (o != null && o.type == Type.player)
                        {
                            update = Builders<Map>.Update.Inc("playerCount", -1);
                            await _mapCollection.UpdateOneAsync(filter, update);
                        }
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
            if (playerPosition[0] + x < size &&
                playerPosition[1] + y < size &&
                playerPosition[0] + x >= 0 &&
                playerPosition[1] + y >= 0)
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
            var p = (Player)map.tiles[playerPosition[0]][playerPosition[1]].obj;
            throw new OutOfBoundsException(p.name, playerPosition);

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
                        Console.WriteLine("Player done " + p.damage + " damage to " + enemy.name);
                        if (enemy.health <= 0)
                        {
                            p = (Player)LevelUp(p);
                            map.tiles[objPos[0]][objPos[1]].obj = null;
                            Console.WriteLine("Enemy died");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Enemy has " + enemy.health + " health remaining");
                        }
                        map.tiles[objPos[0]][objPos[1]].obj = enemy;
                        return false;

                    case Type.item:
                        p.list_items.Append(o);
                        Item i = (Item)o;
                        p.damage += i.damage;
                        Console.WriteLine("Found item with " + i.damage + " damage");
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

    public class MapCount
    {
        public string Id { get; set; }
        public int PlayerCount { get; set; }

    };

}
