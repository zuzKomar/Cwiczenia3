using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsynk(HttpContext httpContext)
        {
            await _next(httpContext);
        }
    }
}