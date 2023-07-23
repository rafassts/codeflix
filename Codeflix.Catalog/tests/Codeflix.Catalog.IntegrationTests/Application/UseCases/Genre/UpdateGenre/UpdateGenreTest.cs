using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
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
        await arrangeContext.AddRangeAsync(relations);
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

    [Fact(DisplayName = nameof(UpdateTrowsWhenCategoryDoesntExist))]
    [Trait("Integration/Application", "UpdateGenre Use Cases")]
    public async Task UpdateTrowsWhenCategoryDoesntExist()
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
        await arrangeContext.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var targetRelatedCategories = exampleCategories.GetRange(5, 3);

        var invalidCategoryId = Guid.NewGuid();
        
        var categoriesToRelate = targetRelatedCategories.Select(category => category.Id).ToList();
        categoriesToRelate.Add(invalidCategoryId);  

        var actContext = _fixture.CreateDbContext(true);

        var updateGenre = new UseCase.UpdateGenre(
            new GenreRepository(actContext),
            new UnitOfWork(actContext),
            new CategoryRepository(actContext));

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            categoriesToRelate);

        Func<Task<GenreModelOutput>> action = 
            async () => await updateGenre.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: '{invalidCategoryId}'");

    }

    [Fact(DisplayName = nameof(UpdateThrowsWhenNotFound))]
    [Trait("Integration/Application", "UpdateGenre Use Cases")]
    public async Task UpdateThrowsWhenNotFound()
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
        await arrangeContext.AddRangeAsync(relations);
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

    [Fact(DisplayName = nameof(UpdateWithoutNewCategoryRelations))]
    [Trait("Integration/Application", "UpdateGenre Use Cases")]
    public async Task UpdateWithoutNewCategoryRelations()
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
        await arrangeContext.AddRangeAsync(relations);
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
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(relatedCategories.Count);
        output.Categories.Select(relatedCategory => relatedCategory.Id)
            .ToList()
            .Should()
            .BeEquivalentTo(relations.Select(x => x.CategoryId));

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

        relatedCategoryIdsFromDb.Should().BeEquivalentTo(relations.Select(x => x.CategoryId));

    }

    [Fact(DisplayName = nameof(UpdateWithEmptyCategoryList))]
    [Trait("Integration/Application", "UpdateGenre Use Cases")]
    public async Task UpdateWithEmptyCategoryList()
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
        await arrangeContext.AddRangeAsync(relations);
        await arrangeContext.SaveChangesAsync();

        var actContext = _fixture.CreateDbContext(true);

        var updateGenre = new UseCase.UpdateGenre(
            new GenreRepository(actContext),
            new UnitOfWork(actContext),
            new CategoryRepository(actContext));

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            new List<Guid>());

        var output = await updateGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(0);
        
        output.Categories.Select(relatedCategory => relatedCategory.Id)
            .ToList()
            .Should()
            .BeEquivalentTo(new List<Guid>());

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

        relatedCategoryIdsFromDb.Should().BeEquivalentTo(new List<Guid>());

    }
}
