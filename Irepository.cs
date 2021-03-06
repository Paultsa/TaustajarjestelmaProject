using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace Project
{
    public interface IRepository
    {
        Task<Map> DeleteMap(string mapId);
        Task<Map> CreateMap(int size, string map);
        Task<string[,]> PrintMap(string map_id);

        Task<Player> CreatePlayer(string mapId, Player player);
        Task<Enemy> CreateEnemy(string mapId, Enemy enemy);
        Task<Item> CreateItem(string mapId, Item item);
        Task<MapCount[]> GetMapPopulations();
        Task<Player[]> GetPlayers(string mapId);
        Task<Player[]> GetPlayersWithMinLevel(string mapId, int minLevel);
        Task<IMapObject> MovePlayer(string mapId, string playerId, Direction dir);
    }
}