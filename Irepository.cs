using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace Project
{
    public interface IRepository
    {
        Task<Map> CreateMap(int size, string map);
        Task<string[,]> PrintMap(string map_id);

        Task<Player> CreatePlayer(string mapId, Player player);

        Task<Player> MovePlayer(string mapId, string playerId, Direction dir);
    }
}