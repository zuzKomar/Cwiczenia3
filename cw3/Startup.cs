using cw3.DAL;
using cw3.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cw3
{
    public class Startup
    {
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //zwraca stronę z dokładnym opisem błędów
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}