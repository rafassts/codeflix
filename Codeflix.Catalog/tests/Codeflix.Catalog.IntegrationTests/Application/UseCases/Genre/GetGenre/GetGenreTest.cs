using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Infra.Data.EF.Models;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.GetGenre;


namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture) => _fixture=fixture;

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Application", "Get Genre - Use Cases")]
    public async Task Get()
    {
        var genresExampleList = _fixture.GetExampleGenresList();
        var expectedGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var repo = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(repo);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Application", "Get Genre - Use Cases")]
    public async Task GetThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleGenresList();
        var randomGuid = Guid.NewGuid();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var repo = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(repo);
        var input = new UseCase.GetGenreInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found");
    }

    [Fact(DisplayName = nameof(GetWithCategoryRelations))]
    [Trait("Integration/Application", "Get Genre - Use Cases")]
    public async Task GetWithCategoryRelations()
    {
        var genresExampleList = _fixture.GetExampleGenresList();
        var expectedGenre = genresExampleList[5];

        var categoriesExempleList = _fixture.GetExampleCategoriesList();
        categoriesExempleList.ForEach(cat => expectedGenre.AddCategory(cat.Id));

        var dbArrangeContext = _fixture.CreateDbContext();
        
        await dbArrangeContext.AddRangeAsync(categoriesExempleList);
        await dbArrangeContext.AddRangeAsync(genresExampleList);

        await dbArrangeContext
            .AddRangeAsync(expectedGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, expectedGenre.Id)));

        await dbArrangeContext.SaveChangesAsync();

        var repo = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(repo);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.Categories.Should().HaveCount(expectedGenre.Categories.Count);

        output.Categories
            .ToList()
            .ForEach(id => expectedGenre.Categories.Should().Contain(id));
    }
}
