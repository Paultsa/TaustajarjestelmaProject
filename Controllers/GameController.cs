using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Collections.Generic;


namespace Project
{
    [ApiController]
    public class GameController : ControllerBase
    {

        private readonly ILogger<GameController> _logger;
        private readonly IRepository _irepository;


        public GameController(ILogger<GameController> logger, IRepository irepository)
        {
            _logger = logger;
            _irepository = irepository;
        }

        [HttpGet]
        [Route("printmap/{map_id}")]
        public Task<string[,]> PrintMap(string map_id)
        {
            return _irepository.PrintMap(map_id);
        }

        [HttpPost]
        [Route("createmap/{name}/{size:int}")]
        public async Task<Map> CreateMap(int size, string name)
        {
            var map = await _irepository.CreateMap(size, name);
            await _irepository.PrintMap(map.id);
            return map;
        }

        [HttpPost]
        [Route("{mapId}/createplayer")]
        public async Task<Player> CreatePlayer(string mapId, [FromBody] Player player)
        {

            var p = await _irepository.CreatePlayer(mapId, player);
            await _irepository.PrintMap(mapId);
            return p;

        }

        [HttpPost]
        [Route("{mapId}/{playerId}/move/{dir}")]
        public async Task<IMapObject> MovePlayer(string mapId, string playerId, string dir)
        {
            Direction direction = (Direction)Enum.Parse(typeof(Direction), dir);
            var p = await _irepository.MovePlayer(mapId, playerId, direction);
            await _irepository.PrintMap(mapId);
            return p;
        }
    }
}