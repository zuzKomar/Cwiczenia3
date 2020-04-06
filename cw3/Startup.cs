using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using cw3.DAL;
using cw3.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cw3
{
    public class Startup
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17301;Integrated Security=True";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)//globalny panel konfiguracji aplikacji
        {
            //metody definiujące LifeTime(cykl życia) naszych obiektów
            
            //services.AddScoped() w ramach tej samej komunikacji http dla tej samej sesji bedzie zwracana ta smaa instancja
            services.AddSingleton<IDbService, MockDbService>(); //bedzie tworzona TYLKO JEDNA instancja takiej klasy i ona bedzie zwracana

            services.AddTransient<IStudentDbService, SqlServerStudentDbService>(); 
            services.AddControllers(); //zarejestrowanie kontrolerow z widokami i stronami
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbService dbService)
        {
         

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //zwraca stronę z dokładnym opisem błędów
            }
            
            app.UseHttpsRedirection();
            
            app.UseRouting();
            
            app.Use(async (contex, task) =>
            {
                if (!contex.Request.Headers.ContainsKey("Index"))
                {
                    contex.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await contex.Response.WriteAsync("Nie podałeś indeksu");
                    return;
                }

                string index = contex.Request.Headers["Index"].ToString();

                using (var con = new SqlConnection(ConString))
                using(var com = new SqlCommand())
                {
                    com.Connection = con;
                    con.Open();
                    
                    com.CommandText = "Select 1 from Student where indexNumber =" +index;
                    
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        await contex.Response.WriteAsync("Student o podanym numerze indeksu nie istnieje!");
                        return;
                    }

                }
                
                await task();
            });
          

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}