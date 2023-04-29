using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PokemonApi.Data;
using PokemonApi.DbAccess;
using PokemonApi.Middleware;
using WatchDog;
using WatchDog.src.Enums;

namespace PokemonApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IConfiguration configuration = builder.Configuration;

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IPokemonStatsDbAccess, PokemonStatsSqlDbAccess>();
            builder.Services.AddSingleton<IPokemonStatsRepository, PokemonStatsSqlRepo>();
            builder.Services.AddSingleton<IPokemonImagesRepository, PokemonImagesMongoRepo>();

            builder.Services.AddWatchDogServices(settings =>
            {
                settings.IsAutoClear = true;
                settings.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Every6Hours;
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("PokemonCache");
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:55559", "https://localhost:55559"));
            });

            builder.Services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("PokemonStatsDatabase"))
                .AddMongoDb(configuration.GetConnectionString("PokemonImagesDatabase"))
                .AddRedis(configuration.GetConnectionString("PokemonCache"));

            var app = builder.Build();

            app.AddGlobalExceptionHandling();
            app.UseWatchDogExceptionLogger();
            app.UseCors("AllowSpecificOrigin");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseWatchDog(opt =>
            {
                opt.WatchPageUsername = "SA";
                opt.WatchPagePassword = "StrongPassword@1";
                opt.CorsPolicy = "AllowSpecificOrigin";
            });

            //app.UseHttpsRedirection();
            app.MapHealthChecks("/health",
                new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

            app.UseRouting();
            app.MapControllers();
            app.Run();
        }
    }
}