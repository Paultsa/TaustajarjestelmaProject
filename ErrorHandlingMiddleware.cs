using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Project
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(OutOfBoundsException o)
            {
                Console.WriteLine("OutOfBoundsException caught by ErrorHandlingMiddleware");
                Console.WriteLine("Player: " + o.playerName + " tried to move outside of the map at: " + o.moveIndex[0] + "," + o.moveIndex[1]);
                context.Response.StatusCode = 405;
                // Write response
                await context.Response.WriteAsync("OutOfBoundsException caught by ErrorHandlingMiddleware\n");
                await context.Response.WriteAsync("Player: " + o.playerName + " tried to move outside of the map at: " + o.moveIndex[0] + "," + o.moveIndex[1]);
            }
            catch(PlayerNotFoundException p)
            {
                Console.WriteLine("PlayerNotFoundException caught by ErrorHandlingMiddleware");
                Console.WriteLine("Player: " + p.playerId + " was not found on map: " + p.mapId);
                context.Response.StatusCode = 404;
                // Write response
                await context.Response.WriteAsync("PlayerNotFoundException caught by ErrorHandlingMiddleware\n");
                await context.Response.WriteAsync("Player: " + p.playerId + " was not found on map: " + p.mapId);
            }
        }
    }
}