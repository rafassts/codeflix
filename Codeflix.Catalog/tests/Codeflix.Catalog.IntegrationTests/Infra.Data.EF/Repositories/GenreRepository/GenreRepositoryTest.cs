using FluentAssertions;
using Repository = Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Insert()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);

        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(dbContext);

        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        //feito isso por conta do tracking do ef core - cria outro contexto

        var assertsDbContext = _fixture.CreateDbContext(true);

        var dbGenre = await (assertsDbContext)
            .Categories
            .FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre?.Name.Should().Be(exampleGenre.Name);
        dbGenre?.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre?.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var genreCategoriesRelations = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
            .ToList();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelations.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();  
        });
    }
}
