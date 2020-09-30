using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Collections.Generic;



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
    [Route("printmap")]
    public Task<string[,]> PrintMap()
    {
        Task<string[,]> map = _irepository.PrintMap();
        return map;
    }
}
