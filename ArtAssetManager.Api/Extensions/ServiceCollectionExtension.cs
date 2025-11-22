using System.Security.Cryptography.X509Certificates;
using ArtAssetManager.Api.Data.Repositories;
using ArtAssetManager.Api.Errors;
using ArtAssetManager.Api.Interfaces;
using ArtAssetManager.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace ArtAssetManager.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IMaterialSetRepository, MaterialSetRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<ISavedSearchRepository, SavedSearchRepository>();
            return services;
        }
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Program>();
            return services;
        }
        //Boilerplate do ujednolicenia walidacji i error response
        public static IServiceCollection ErrorResponseConfig(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value?.Errors.Select(e => e.ErrorMessage).ToList() ?? new List<string>()
                );

                    var errorResponse = new ApiErrorResponse(
                        System.Net.HttpStatusCode.BadRequest,
                        "Wystąpiły błędy walidacji",
                        context.HttpContext.Request.Path,
                        errors
                    );

                    return new BadRequestObjectResult(errorResponse);
                };
            });
            return services;
        }
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<StartupInitializationService>();
            services.AddHostedService<ScannerService>();
            return services;
        }
    }
}