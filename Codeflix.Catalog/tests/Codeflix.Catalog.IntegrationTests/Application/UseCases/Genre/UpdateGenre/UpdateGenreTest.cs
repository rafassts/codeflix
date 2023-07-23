using Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Models;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Application", "UpdateGenre Use Cases")]
    public async Task Update()
    {
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenreList[5];
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(exampleGenreList);
        await arrangeContext.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);

        var updateGenre = new UseCase.UpdateGenre(
            new GenreRepository(actContext),
            new UnitOfWork(actContext),
            new CategoryRepository(actContext));

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive);

        var output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool) input.IsActive!);

        var assertContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertContext.Genres.FindAsync(targetGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(UpdateWithCategoryRelations))]
    [Trait("Integration/Application", "UpdateGenre Use Cases")]
    public async Task UpdateWithCategoryRelations()
    {
        var arrangeContext = _fixture.CreateDbContext();
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        var exampleGenreList = _fixture.GetExampleGenresList(10);
        var targetGenre = exampleGenreList[5];
        var relatedCategories = exampleCategories.GetRange(0, 5);

        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));

        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();

        await arrangeContext.AddRangeAsync(exampleGenreList);
        await arrangeContext.AddRangeAsync(exampleCategories);
        await arrangeContext.AddRangeAsync(relatedCategories);
        await arrangeContext.SaveChangesAsync();

        var targetRelatedCategories = exampleCategories.GetRange(5, 3);

        var actContext = _fixture.CreateDbContext(true);

        var updateGenre = new UseCase.UpdateGenre(
            new GenreRepository(actContext),
            new UnitOfWork(actContext),
            new CategoryRepository(actContext));

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            targetRelatedCategories.Select(category => category.Id).ToList());

        var output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(targetRelatedCategories.Count);
        output.Categories.Select(relatedCategory => relatedCategory.Id)
            .ToList()
            .Should()
            .BeEquivalentTo(input.CategoriesIds);

        var assertContext = _fixture.CreateDbContext(true);
        var genreFromDb = await assertContext.Genres.FindAsync(targetGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);

        List<Guid> relatedCategoryIdsFromDb =
            await assertContext.GenresCategories
                .AsNoTracking()
                .Where(relation => relation.GenreId == input.Id)
                .Select(relation => relation.CategoryId)
                .ToListAsync();

        relatedCategoryIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);

    }   
}
