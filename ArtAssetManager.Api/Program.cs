using ArtAssetManager.Api.Config;
using ArtAssetManager.Api.Data;
using ArtAssetManager.Api.Data.Repositories;
using ArtAssetManager.Api.DTOs;
using ArtAssetManager.Api.Entities;
using ArtAssetManager.Api.Interfaces;
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
builder.Services.Configure<ScannerSettings>(builder.Configuration.GetSection("ScannerSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
