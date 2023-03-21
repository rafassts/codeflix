using Bogus;
using Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Codeflix.Catalog.IntegrationTests.Base;
public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
    }

    //problema do paralelismo dos testes do xunit - xunit.runner.json (properties - copy always)
    //dependendo do caso, pode ser melhor criar vários bancos para usar o paralelismo,
    //concatenando um id rand no nome do banco (problema se houver muitos e muitos testes)
    public CodeflixCategoryDbContext CreateDbContext(bool preserveData = false)
    {
        var context = new CodeflixCategoryDbContext(
            new DbContextOptionsBuilder<CodeflixCategoryDbContext>()
                .UseInMemoryDatabase("integration-tests-db")
                .Options
            );

        //poderiamos usar o idisposable no teste, para deletar os dados em cada teste
        if (!preserveData)
            context.Database.EnsureDeleted();

        return context;
    }
}
