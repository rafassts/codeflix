﻿using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using System.Reflection;

namespace Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        //alteração mediatr 12
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddRepositories();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        return services;
    }

}
