using Codeflix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Codeflix.Catalog.EndToEndTests.Base;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("EndToEndTest");
        
        builder.ConfigureServices(services => {
            
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            //deleta e cria um novo banco para os testes
            var context = scope.ServiceProvider.GetService<CodeflixCatalogDbContext>();
            ArgumentNullException.ThrowIfNull(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }
}
