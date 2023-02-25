using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Repository = Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTest
{
    private readonly CategoryRepositoryTestFixture _fixture;

    public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture)
        => _fixture = fixture;
    

    [Fact(DisplayName =nameof(Insert))]
    [Trait("Integration/Infra.Data","CategoryRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var repo = new Repository.CategoryRepository(dbContext);
       
        await repo.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        //feito isso por conta do tracking do ef core, se procurarmos no mesmo contexto, não precisaria
        //usar o savechanges e nem mesmo o update para o find retornar a categoria atualizada
        //poderíamos usar o asnotracking no mesmo contexto também
        var dbCategory =
            await (_fixture.CreateDbContext())
            .Categories
            .FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory?.Name.Should().Be(exampleCategory.Name);
        dbCategory?.Description.Should().Be(exampleCategory.Description);   
        dbCategory?.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory?.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);

        var dbCategory = await repo.Get(exampleCategory.Id,CancellationToken.None); 

        dbCategory.Should().NotBeNull();
        dbCategory?.Id.Should().Be(exampleCategory.Id);
        dbCategory?.Name.Should().Be(exampleCategory.Name);
        dbCategory?.Description.Should().Be(exampleCategory.Description);
        dbCategory?.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory?.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowIfNotFound))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetThrowIfNotFound()
    {
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);

        var task = async() => await repo.Get(exampleId, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{exampleId}' not found");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var newCategoryValues = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);


        exampleCategory.Update(newCategoryValues.Name, newCategoryValues.Description);
        await repo.Update(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        //feito isso por conta do tracking do ef core, se procurarmos no mesmo contexto, não precisaria
        //usar o savechanges e nem mesmo o update para o find retornar a categoria atualizada
        //poderíamos usar o asnotracking no mesmo contexto também
        var dbCategory =
            await (_fixture.CreateDbContext())
            .Categories
            .FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory?.Id.Should().Be(exampleCategory.Id);
        dbCategory?.Name.Should().Be(exampleCategory.Name);
        dbCategory?.Description.Should().Be(exampleCategory.Description);
        dbCategory?.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory?.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

}
