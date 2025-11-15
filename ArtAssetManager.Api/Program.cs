using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Data;
using ArtAssetManager.Api.Data.Repositories;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Interfaces;
using ArtAssetManager.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AssetDbContext>(options =>
{
    options.UseSqlite("Data Source=assets.db;Foreign Keys=True");
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IMaterialSetRepository, MaterialSetRepository>();

builder.Services.Configure<ScannerSettings>(builder.Configuration.GetSection("ScannerSettings"));
builder.Services.AddHostedService<StartupInitializationService>();
builder.Services.AddHostedService<ScannerService>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDevClient",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseExceptionHandler("/error");



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowDevClient");
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

    return Results.Json(new
    {
        status = 500,
        message = "An error occurred",
        path = context.Request.Path,
        timestamp = DateTime.UtcNow,
        error = isDevelopment ? exception?.Message : null,
        stackTrace = isDevelopment ? exception?.StackTrace : null

    }, statusCode: 500);
});

app.Run();