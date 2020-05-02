using System.IO;
using System.Text;
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

        public async Task InvokeAsync (HttpContext httpContext)
        {
           httpContext.Request.EnableBuffering();

           if (httpContext.Request != null)
           {
               string sciezka = httpContext.Request.Path;  //api/students
               string queryString = httpContext.Request?.QueryString.ToString();
               string metoda = httpContext.Request.Method;     //GET, POST
               string bodyStr = "";

               using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
               {
                   bodyStr = await reader.ReadToEndAsync();
                   httpContext.Request.Body.Position = 0;
               }
               //logowanie do pliku
               string[] lines = {metoda, sciezka, queryString, bodyStr};
               System.IO.File.AppendAllLines("requestsLog.txt",lines);
           }

           if (_next != null) await _next(httpContext);
        }
    }
}