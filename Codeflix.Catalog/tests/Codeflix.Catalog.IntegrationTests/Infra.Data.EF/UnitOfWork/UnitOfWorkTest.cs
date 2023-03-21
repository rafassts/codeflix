namespace Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UnifOfWorkInfra = Catalog.Infra.Data.EF;

[Collection(nameof(UnifOfWorkTestFixture))]
public class UnitOfWorkTest
{
    private readonly UnifOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnifOfWorkTestFixture fixture)
    {
        _fixture=fixture;
    }

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoriesList);

        var uow = new UnifOfWorkInfra.UnitOfWork(dbContext);

        await uow.Commit(CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var savedCategories = assertDbContext
            .Categories
            .AsNoTracking()
            .ToList();

        savedCategories.Should().HaveCount(exampleCategoriesList.Count);
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();

        var uow = new UnifOfWorkInfra.UnitOfWork(dbContext);

        var task = async () => await uow.Rollback(CancellationToken.None);

        //o ef já implementa, mas se um dia usar outro, está no teste
        await task.Should().NotThrowAsync();
    }

}
