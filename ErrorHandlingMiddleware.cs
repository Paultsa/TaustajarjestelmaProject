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
            catch(OutOfBoundsException)
            {
                Console.WriteLine("OutOfBoundsException caught by ErrorHandlingMiddleware");
            }
        }
    }
}