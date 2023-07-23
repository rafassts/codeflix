using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Models;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture) => _fixture=fixture;

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Application", "Delete Use Cases")]
    public async Task Delete()
    {
        var genreExampleList = _fixture.GetExampleGenresList();
        var targetGenre = genreExampleList[5];

        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(genreExampleList);
        await arrangeContext.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(new GenreRepository(actContext), new UnitOfWork(actContext));
        var input = new UseCase.DeleteGenreInput(targetGenre.Id);
        await useCase.Handle(input, CancellationToken.None);

        var assertContext = _fixture.CreateDbContext(true);

        var genreFromDb = await assertContext.Genres.FindAsync(targetGenre.Id);

        genreFromDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteThrowsWhenNotFound))]
    [Trait("Integration/Application", "Delete Use Cases")]
    public async Task DeleteThrowsWhenNotFound()
    {
        var genreExampleList = _fixture.GetExampleGenresList();

        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(genreExampleList);
        await arrangeContext.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(new GenreRepository(actContext), new UnitOfWork(actContext));

        var invalidId = Guid.NewGuid();

        var input = new UseCase.DeleteGenreInput(invalidId);
        var action = async () => await useCase.Handle(input, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>()
         .WithMessage($"Genre '{invalidId}' not found");

    }

    [Fact(DisplayName = nameof(DeleteWithRelations))]
    [Trait("Integration/Application", "Delete Use Cases")]
    public async Task DeleteWithRelations()
    {
        var genreExampleList = _fixture.GetExampleGenresList();
        var targetGenre = genreExampleList[5];
        var exampleCategories = _fixture.GetExampleCategoriesList();

        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(genreExampleList);
        await arrangeContext.AddRangeAsync(exampleCategories);
        
        await arrangeContext.AddRangeAsync(
            exampleCategories.Select(category => new GenresCategories(category.Id, targetGenre.Id)));

        await arrangeContext.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);
        var useCase = new UseCase.DeleteGenre(new GenreRepository(actContext), new UnitOfWork(actContext));
        var input = new UseCase.DeleteGenreInput(targetGenre.Id);
        await useCase.Handle(input, CancellationToken.None);

        var assertContext = _fixture.CreateDbContext(true);

        var genreFromDb = await assertContext.Genres.FindAsync(targetGenre.Id);

        var genresCategoriesFromDb = await assertContext
            .GenresCategories
            .AsNoTracking()
            .Where(relation => relation.GenreId == targetGenre.Id)
            .ToListAsync();

        genreFromDb.Should().BeNull();
        genresCategoriesFromDb.Should().HaveCount(0);
    }
}
