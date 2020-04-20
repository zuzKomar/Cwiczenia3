﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using cw3.DAL;
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

        public async Task InvokeAsynk(HttpContext httpContext, IDbService service)
        {
           httpContext.Request.EnableBuffering();

           if (httpContext.Request != null)
           {
               string sciezka = httpContext.Request.Path;  //weatherforecast/cos
               string queryString = httpContext.Request?.QueryString.ToString();
               string metoda = httpContext.Request.Method.ToString();
               string bodyStr = "";

               using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
               {
                   bodyStr = await reader.ReadToEndAsync();
               }
               //logowanie do pliku
           }

           await _next(httpContext);
        }
    }
}