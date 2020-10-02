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
        //localhost:5000/createmap/Map_1/9
        [Route("createmap/{name}/{size:int}")]
        public async Task<Map> CreateMap(int size, string name)
        {
            var map = await _irepository.CreateMap(size, name);
            await _irepository.PrintMap(map.id);
            return map;
        }

        [HttpPost]
        //localhost:5000/Map_1/createplayer
        /*
        {
        "name" : "Matti",
        "damage" : 1
        }
        */
        [Route("{mapId}/createPlayer")]
        public async Task<Player> CreatePlayer(string mapId, [FromBody] Player player)
        {

            var p = await _irepository.CreatePlayer(mapId, player);
            await _irepository.PrintMap(mapId);
            return p;
        }

        [HttpPost]
        //localhost:5000/Map_1/createItem
        /*
        {
        "name" : "Miekka",
        "damage" : 1
        }
        */
        [Route("{mapId}/createItem")]
        public async Task<Item> CreateItem(string mapId, [FromBody] Item item)
        {

            var i = await _irepository.CreateItem(mapId, item);
            await _irepository.PrintMap(mapId);
            return i;
        }

        [HttpPost]
        /*
        //localhost:5000/Map_1/createEnemy
        {
        "name" : "Örkki",
        "damage" : 1,
        "health" : 5
        }
        */
        [Route("{mapId}/createEnemy")]
        public async Task<Enemy> CreateEnemy(string mapId, [FromBody] Enemy enemy)
        {

            var e = await _irepository.CreateEnemy(mapId, enemy);
            await _irepository.PrintMap(mapId);
            return e;
        }

        [HttpPost]
        //localhost:5000/Map_1/*id*/move/left
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