using BlockedCountriesAPI.Repositories.IRepository;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Services;
using BlockedCountriesAPI.Services.IServices;
using BlockedCountriesAPI.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlockedCountriesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register repositories
            builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>();
            builder.Services.AddSingleton<IBlockedCountryRepository, BlockedCountryRepository>();

            // Register services
            builder.Services.AddSingleton<IGeoLocationService, GeoLocationService>();
            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddSingleton<ICountryBlockService, CountryBlockService>();

            // Register background services
            builder.Services.AddHostedService<TemporalBlockService>();

            // Register HttpClient for GeoLocationService
            builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>();

            // Add controllers
            builder.Services.AddControllers();

            // Swagger configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Build the application
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            // Run the application
            app.Run();
        }
    }
}
