using Codeflix.Catalog.Infra.Data.EF.Repositories;
using Codeflix.Catalog.Infra.Data.EF;
using AppUseCases = Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Codeflix.Catalog.Application.Exceptions;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(Create))]
    [Trait("Integration/Application", "CreateGenre Use Cases")]
    public async void Create()
    {
        var dbContext = _fixture.CreateDbContext();

        var useCase = new AppUseCases.CreateGenre(
            new GenreRepository(dbContext),
             new UnitOfWork(dbContext),
            new CategoryRepository(dbContext));

        var input = _fixture.GetExampleInput();

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);

        //ver se salvou no banco
        var dbContext2 = _fixture.CreateDbContext(true);

        var genreFromDb = await dbContext2
            .Genres
            .FindAsync(output.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
        genreFromDb.CreatedAt.Should().Be(output.CreatedAt);

        

    }

    [Fact(DisplayName = nameof(CreateWithRelations))]
    [Trait("Integration/Application", "CreateGenre Use Cases")]
    public async void CreateWithRelations()
    {

        var actDbContext = _fixture.CreateDbContext();
        var useCase = new AppUseCases.CreateGenre(
            new GenreRepository(actDbContext),
             new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext));

        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
        var arrangeDbContext = _fixture.CreateDbContext(true);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();

        var input = _fixture.GetExampleInput();

        input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(input.CategoriesIds.Count);

        //somente guid de ids
        output.Categories
            .Select(category => category.Id)
            .ToList()
            .Should()
            .BeEquivalentTo(input.CategoriesIds);

        //ver se salvou no banco
        var assertDbContext = _fixture.CreateDbContext(true);

        var genreFromDb = await assertDbContext
            .Genres
            .FindAsync(output.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
        genreFromDb.CreatedAt.Should().Be(output.CreatedAt);

        var relationsFromDb = await assertDbContext
            .GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == output.Id).ToListAsync();

        relationsFromDb.Should().HaveCount(input.CategoriesIds.Count);

        relationsFromDb.Select(
            relation => relation.CategoryId)
            .ToList()
            .Should()
            .BeEquivalentTo(input.CategoriesIds);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryDoesntExist))]
    [Trait("Integration/Application", "CreateGenre Use Cases")]
    public async void ThrowWhenCategoryDoesntExist()
    {

        var actDbContext = _fixture.CreateDbContext();
        var useCase = new AppUseCases.CreateGenre(
            new GenreRepository(actDbContext),
             new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext));

        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(5);
        var arrangeDbContext = _fixture.CreateDbContext(true);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.SaveChangesAsync();

        var input = _fixture.GetExampleInput();

        input.CategoriesIds = exampleCategories.Select(category => category.Id).ToList();
        
        var randomGuid = Guid.NewGuid();
        input.CategoriesIds.Add(randomGuid);

        Func<Task<GenreModelOutput>> action = async() => await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: '{randomGuid}'");
    }
}
