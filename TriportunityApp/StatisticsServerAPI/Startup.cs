using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.DataAccess.Repositories;
using StatisticsServerAPI.Services;

namespace WebApiRabbitMQ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<MQUserService>(); 
            services.AddSingleton<MQRideService>();
            services.AddSingleton<Database>();

            services.AddScoped<ILoginEventRepository, LoginEventRepository>();
            services.AddScoped<IRideEventRepository, RideEventRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            var serviceProvider = app.ApplicationServices;
            serviceProvider.GetRequiredService<MQUserService>();
            serviceProvider.GetRequiredService<MQRideService>();
        }
    }
}