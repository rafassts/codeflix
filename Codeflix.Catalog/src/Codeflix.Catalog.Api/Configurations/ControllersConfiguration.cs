using Codeflix.Catalog.Api.Configurations.Policies;
using Codeflix.Catalog.Api.Filters;

namespace Codeflix.Catalog.Api.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddAndConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers(opt => opt.Filters.Add(typeof(ApiGlobalExceptionFilter)))
            .AddJsonOptions(jOps =>
            {
                jOps.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCasePolicy();
            });


        services.AddDocumentation();
        return services;
    }

    private static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static WebApplication UseDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}
