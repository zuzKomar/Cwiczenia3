using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using cw3.DAL;
using cw3.Handlers;
using cw3.Middlewares;
using cw3.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace cw3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)//globalny panel konfiguracji aplikacji
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true, //czy token nie wygasł
                    ValidateAudience = true,
                    ValidAudience = "Students",
                    ValidIssuer = "Gakko",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
                };
            });


            services.AddTransient<IDbService, MockDbService>(); 
            services.AddSingleton<IStudentDbService, SqlServerStudentDbService>();
            services.AddControllers() //zarejestrowanie kontrolerow z widokami i stronami
            .AddXmlSerializerFormatters();   //content negotiation
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SqlServerStudentDbService service) //konfigurowanie middlewarow
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //zwraca stronę z dokładnym opisem błędów
            }
            
           
            app.UseHttpsRedirection();
            
            app.UseMiddleware<LoggingMiddleware>();
            
            app.Use(async (contex, task) =>
            {
                if (!contex.Request.Headers.ContainsKey("IndexNumber"))
                {
                    contex.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await contex.Response.WriteAsync("Indeks nie został podany");
                    return;
                }
            
                var index = contex.Request.Headers["IndexNumber"].ToString();
                
                if(!service.CheckIndexNumber(index))
                {
                    contex.Response.StatusCode = StatusCodes.Status404NotFound;
                    await contex.Response.WriteAsync("Student o takim indeksie nie istnieje");
                    return;
                }
                
                await task();
            });

            app.UseRouting(); //kiedy przychodzi zadanie get na api/students---studentsController---getStudents()
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}