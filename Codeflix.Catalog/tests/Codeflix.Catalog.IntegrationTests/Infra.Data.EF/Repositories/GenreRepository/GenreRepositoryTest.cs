using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
            .Genres
            .FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre?.Name.Should().Be(exampleGenre.Name);
        dbGenre?.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre?.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var genreCategoriesRelations = await assertsDbContext
            .GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);
        

        //se as categorias que deveriam ser inseridas foram de fato
        genreCategoriesRelations.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();  
        });
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Get()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);

        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);

        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        
        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var genreFromRepo = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);

        genreFromRepo.Should().NotBeNull();
        genreFromRepo!.Name.Should().Be(exampleGenre.Name);
        genreFromRepo.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromRepo.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        genreFromRepo.Categories.Should().NotBeNull();
        genreFromRepo.Categories.Should().HaveCount(categoriesListExample.Count);

        //se as categorias que deveriam ser inseridas foram de fato
        foreach (var categoryId in genreFromRepo.Categories)
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == categoryId);
            expectedCategory.Should().NotBeNull();
        }
    }

    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetThrowWhenNotFound()
    {
        var exampleNotFoundGuid = Guid.NewGuid();
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);

        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);

        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache
        var genreRepository = new Repository.GenreRepository(_fixture.CreateDbContext(true));

        var action = async () => await genreRepository.Get(exampleNotFoundGuid, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleNotFoundGuid}' not found");
    
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Delete()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);

        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);

        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache
        var repoDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(repoDbContext);

        await genreRepository.Delete(exampleGenre, CancellationToken.None);

        //não dá o save changes dentro do repo porque vai ser tarefa do uow na aplicação
        await repoDbContext.SaveChangesAsync();

        var assertDbContext = _fixture.CreateDbContext(true);

        var dbGenre = await assertDbContext
            .Genres
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleGenre.Id);

        dbGenre.Should().BeNull();

        var categoriesIdsList = await assertDbContext
            .GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == exampleGenre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();

        categoriesIdsList.Should().HaveCount(0);
        
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Update()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);

        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);

        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache

        var actDbContext = _fixture.CreateDbContext(true);

        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();


        await genreRepository.Update(exampleGenre, CancellationToken.None);

        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);

        var genreFromDb = await (assertsDbContext)
            .Genres
            .FindAsync(exampleGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(exampleGenre.Name);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var genreCategoriesRelations = await assertsDbContext
            .GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);

        //se as categorias que deveriam ser inseridas foram de fato
        genreCategoriesRelations.ForEach(relation =>
        {
            var expectedCategory = categoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
        });
    }
}
