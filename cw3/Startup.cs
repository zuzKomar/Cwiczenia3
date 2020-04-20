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
         //   services.AddAuthentication("AuthenticationBasic")
          //      .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("AuthenticationBasic", null);

          services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true, 
                      ValidateLifetime = true,
                      ValidAudience = "Students",
                      ValidIssuer = "Ja",
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))

                  };
              });

            services.AddScoped<IStudentDbService, SqlServerStudentDbService>(); //w ramach tej samej komunikacji http dla tej samej sesji bedzie zwracana ta smaa instancja
            services.AddSingleton<IDbService, MockDbService>(); //bedzie tworzona TYLKO JEDNA instancja takiej klasy i ona bedzie zwracana

           // services.AddTransient<IStudentDbService, SqlServerStudentDbService>();
            services.AddControllers(); //zarejestrowanie kontrolerow z widokami i stronami
            //.AddXmlSerializerFormatters();   //content negotiation
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentDbService service) //konfigurowanie middlewarow
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //zwraca stronę z dokładnym opisem błędów
            }

            app.UseMiddleware<LoggingMiddleware>();

           
             
            app.Use(async (contex, task) =>
            {
                if (!contex.Request.Headers.ContainsKey("IndexNumber"))
                {
                    contex.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await contex.Response.WriteAsync("Nie podano numeru indeksu");
                    return;
                }
            
                string index = contex.Request.Headers["IndexNumber"].ToString();

                var student = service.GetStudent(index);
                if(student == null)
                {
                    contex.Response.StatusCode = StatusCodes.Status404NotFound;
                    await contex.Response.WriteAsync("Student not found");
                    return;
                }
                
                await task();
            });

            app.UseHttpsRedirection();
            app.UseRouting(); //kiedy przychodzi zadanie get na api/students---studentsController---getStudents()
           // app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}