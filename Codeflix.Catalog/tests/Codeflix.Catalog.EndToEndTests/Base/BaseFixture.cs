using Bogus;
using Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Codeflix.Catalog.EndToEndTests.Base;

public class BaseFixture
{
    protected Faker Faker { get; set; }
    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
    public HttpClient HttpClient { get; set; }
    public ApiClient ApiClient { get; set; }
    private readonly string _dbConnectionString;

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
     
        var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));

        ArgumentNullException.ThrowIfNull(configuration);

        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb") ?? "";
    }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase("e2e-tests-db")
                .Options);

        //var context = new CodeflixCatalogDbContext(
        //  new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
        //  .UseMySql(_dbConnectionString,ServerVersion.AutoDetect(_dbConnectionString))
        //  .Options);

        return context;
    }

    //o xunit.runner evita o paralelismo, e o idisposable limpa o banco para o outro
    public void CleanPersistence()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}