using System.Text.Json;
using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Data;
using ArtAssetManager.Api.Data.Repositories;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Errors;
using ArtAssetManager.Api.Interfaces;
using ArtAssetManager.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AssetDbContext>(options =>
{
    options.UseSqlite("Data Source=assets.db;Foreign Keys=True");
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
            .ToList();

        var errorResponse = new ApiErrorResponse(
            System.Net.HttpStatusCode.BadRequest,
            "Wystąpiły błędy walidacji",
            context.HttpContext.Request.Path,
            errors
        );

        return new BadRequestObjectResult(errorResponse);
    };
});

builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IMaterialSetRepository, MaterialSetRepository>();

builder.Services.Configure<ScannerSettings>(builder.Configuration.GetSection("ScannerSettings"));
builder.Services.AddHostedService<StartupInitializationService>();
builder.Services.AddHostedService<ScannerService>();
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

app.Run();