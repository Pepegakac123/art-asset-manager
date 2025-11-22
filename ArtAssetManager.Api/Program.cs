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

var builder = WebApplication.CreateBuilder(args);

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
    if (isDevelopment && exception != null)
    {
        logger.LogError(exception.Message, "Error occurred");
        logger.LogError(exception.StackTrace, "Stack trace of error occurre");
    }

    return Results.Json(new ApiErrorResponse(System.Net.HttpStatusCode.InternalServerError, "An error occurred", context.Request.Path), statusCode: (int)System.Net.HttpStatusCode.InternalServerError);
});

app.MapHub<ScanHub>("/hubs/scan");

app.Run();