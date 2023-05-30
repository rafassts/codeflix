﻿using Codeflix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Codeflix.Catalog.EndToEndTests.Base;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //pega as configurações do appsettings
        builder.UseEnvironment("EndToEndTest");
        
        builder.ConfigureServices(services => {
            
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            //sempre criar um novo banco
            var context = scope.ServiceProvider.GetService<CodeflixCatalogDbContext>();
            ArgumentNullException.ThrowIfNull(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }
}
