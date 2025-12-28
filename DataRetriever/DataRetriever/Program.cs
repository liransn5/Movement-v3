using DataRetriever.Data.Repositories;
using DataRetriever.Services.Cache;
using DataRetriever.Services.Providers;
using DataRetriever.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DataRetriever;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSdscCache(builder.Configuration);
        builder.Services.AddRedisCache(builder.Configuration);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DataRetriever")));
        builder.Services.AddScoped<IDataRepository, SqlDataRepository>();

        RegisterProviderChain(builder.Services);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (db.Database.EnsureCreated())
            {
                logger.LogInformation("Database and tables created.");
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    /// <summary>
    /// Registers and composes the data retrieval provider chain.
    /// The chain is built in the following order:
    /// Redis cache → In-memory SDCS cache →  database.
    /// </summary>
    /// <remarks>
    /// Each provider attempts to resolve the request and forwards it to the next
    /// provider only if the data is not found locally.
    /// The returned <see cref="IDataProvider{TKey, TValue}"/> represents the head
    /// of the chain (Redis), allowing consumers to access the entire pipeline transparently.
    /// </remarks>
    private static void RegisterProviderChain(IServiceCollection services)
    {
        services.AddSingleton<SdcsProvider<string, string>>();
        services.AddSingleton<RedisProvider<string, string>>();
        services.AddScoped<DbProvider>();
        services.AddScoped<IDataProvider<string, string>>(sp =>
        {
            var redis = sp.GetRequiredService<RedisProvider<string, string>>();
            var sdcs = sp.GetRequiredService<SdcsProvider<string, string>>();
            var db = sp.GetRequiredService<DbProvider>();

            redis.SetNext(sdcs);
            sdcs.SetNext(db);

            return redis;
        });
    }
}
