using StatisticsServerAPI.DataAccess.MemoryDatabase;
using StatisticsServerAPI.DataAccess.Repositories;
using StatisticsServerAPI.MQServices;
using StatisticsServerAPI.Services;

namespace StatisticsServerAPI
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

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRideService, RideService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRideRepository, RideRepository>();

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