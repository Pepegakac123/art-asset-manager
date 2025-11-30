using System.Text.Json;
using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Data;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Errors;
using ArtAssetManager.Api.Extensions;
using ArtAssetManager.Api.Hubs;
using ArtAssetManager.Api.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<AssetDbContext>(options =>
    {
        options.UseSqlite("Data Source=assets.db;Foreign Keys=True");
    });
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
    builder.Services.AddFluentValidation();
    builder.Services.AddRepositories();
    builder.Services.ErrorResponseConfig();

    builder.Services.Configure<ScannerSettings>(builder.Configuration.GetSection("ScannerSettings"));
    builder.Services.AddHostedServices();
    builder.Services.AddSignalR();
    builder.Services.AddSingleton<IScannerTrigger, ScannerTriggerService>();

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
    builder.Services.AddControllers();
    builder.WebHost.UseUrls("http://*:5270");
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp",
         policy => policy
             .WithOrigins("http://localhost:5173")
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials());
    });

    var app = builder.Build();
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler("/error");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // app.UseHttpsRedirection(); Błedy z signalR na localhost
    app.UseDefaultFiles();
    app.UseStaticFiles();
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("AllowReactApp");
    }
    app.MapControllers();

    app.Map("/error", (HttpContext context) =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        var isDevelopment = app.Environment.IsDevelopment();

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        if (exception != null)
        {
            logger.LogError(exception, "An error occurred");
        }
        if (isDevelopment && exception != null)
        {
            logger.LogError(exception.Message, "Error occurred");
            logger.LogError(exception.StackTrace, "Stack trace of error occurre");
        }

        return Results.Json(new ApiErrorResponse(System.Net.HttpStatusCode.InternalServerError, "An error occurred", context.Request.Path), statusCode: (int)System.Net.HttpStatusCode.InternalServerError);
    });

    app.MapHub<ScanHub>("/hubs/scan");
    app.MapFallbackToFile("index.html");
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AssetDbContext>();
            context.Database.Migrate();
            Console.WriteLine("✅ Baza danych zaktualizowana pomyślnie.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Błąd podczas migracji bazy: {ex.Message}");
        }
    }
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("=============================================");
    Console.WriteLine("   ART ASSET MANAGER - GOTOWY DO PRACY");
    Console.WriteLine("=============================================");
    Console.WriteLine("   Otwórz przeglądarkę i wejdź na:");
    Console.WriteLine("   👉 http://localhost:5270");
    Console.WriteLine("=============================================");
    Console.ResetColor();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
