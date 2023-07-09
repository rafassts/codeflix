using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
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
   
    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateRemovingRelations()
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

        exampleGenre.RemoveAllCategories();

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

        genreCategoriesRelations.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateReplacingRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        var updateCategoriesListExample = _fixture.GetExampleCategoriesList(5);

        categoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));
        
        await dbContext.Categories.AddRangeAsync(categoriesListExample);

        await dbContext.Categories.AddRangeAsync(updateCategoriesListExample);

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

        exampleGenre.RemoveAllCategories();

        updateCategoriesListExample.ForEach(category => exampleGenre.AddCategory(category.Id));

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

        genreCategoriesRelations.Should().HaveCount(updateCategoriesListExample.Count);

        //se as categorias que deveriam ser inseridas foram de fato
        genreCategoriesRelations.ForEach(relation =>
        {
            var expectedCategory = updateCategoriesListExample.FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
        });

    }

    [Fact(DisplayName = nameof(SearchReturnsItemsAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsItemsAndTotal()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList();

        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache

        var actDbContext = _fixture.CreateDbContext(true);

        var genreRepository = new Repository.GenreRepository(actDbContext);

        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);

        searchResult.Items.Should().HaveCount(exampleGenresList.Count);

        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.CreatedAt.Should().Be(exampleGenre!.CreatedAt);
            item.IsActive.Should().Be(exampleGenre!.IsActive);
        }

    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList();

        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        var random = new Random();
       
        //percorre a lista de gêneros
        exampleGenresList.ForEach(genre =>
        {
            //cria uma lista de categorias para relacionar
            var categoriesListToRelate = _fixture.GetExampleCategoriesList(random.Next(0,4));

            if(categoriesListToRelate.Count > 0)
            {
                //adiciona as categorias em cada gênero
                categoriesListToRelate.ForEach(category => genre.AddCategory(category.Id));

                //salva as categorias no banco
                dbContext.Categories.AddRange(categoriesListToRelate);

                //cria o relacionamento do gênero com a lista de categorias geradas
                var relationsToAdd = categoriesListToRelate
                    .Select(category => new GenresCategories(category.Id, genre.Id))
                    .ToList();

                //salva o relacionamento no banco
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache

        var actDbContext = _fixture.CreateDbContext(true);

        var genreRepository = new Repository.GenreRepository(actDbContext);

        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);

        searchResult.Items.Should().HaveCount(exampleGenresList.Count);

        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.CreatedAt.Should().Be(exampleGenre!.CreatedAt);
            item.IsActive.Should().Be(exampleGenre!.IsActive);
            item.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);

        }

    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyPersistence))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsEmptyPersistence()
    {
        var actDbContext = _fixture.CreateDbContext();
        var genreRepository = new Repository.GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(0);
        searchResult.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
        int genresAmount,
        int page,
        int perPage,
        int expectedItemsAmount)
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(genresAmount);

        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        var random = new Random();

        //percorre a lista de gêneros
        exampleGenresList.ForEach(genre =>
        {
            //cria uma lista de categorias para relacionar
            var categoriesListToRelate = _fixture.GetExampleCategoriesList(random.Next(0, 4));

            if (categoriesListToRelate.Count > 0)
            {
                //adiciona as categorias em cada gênero
                categoriesListToRelate.ForEach(category => genre.AddCategory(category.Id));

                //salva as categorias no banco
                dbContext.Categories.AddRange(categoriesListToRelate);

                //cria o relacionamento do gênero com a lista de categorias geradas
                var relationsToAdd = categoriesListToRelate
                    .Select(category => new GenresCategories(category.Id, genre.Id))
                    .ToList();

                //salva o relacionamento no banco
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache

        var actDbContext = _fixture.CreateDbContext(true);

        var genreRepository = new Repository.GenreRepository(actDbContext);

        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);

        searchResult.Items.Should().HaveCount(expectedItemsAmount);

        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.CreatedAt.Should().Be(exampleGenre!.CreatedAt);
            item.IsActive.Should().Be(exampleGenre!.IsActive);
            item.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);

        }

    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedReturnedItemsAmount,
        int expectedTotalItemsAmount)
    {
        var dbContext = _fixture.CreateDbContext();

        var exampleGenresList = _fixture.GetExampleGenresListWithNames(new List<string>
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on real facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        });

        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        var random = new Random();

        //percorre a lista de gêneros
        exampleGenresList.ForEach(genre =>
        {
            //cria uma lista de categorias para relacionar
            var categoriesListToRelate = _fixture.GetExampleCategoriesList(random.Next(0, 4));

            if (categoriesListToRelate.Count > 0)
            {
                //adiciona as categorias em cada gênero
                categoriesListToRelate.ForEach(category => genre.AddCategory(category.Id));

                //salva as categorias no banco
                dbContext.Categories.AddRange(categoriesListToRelate);

                //cria o relacionamento do gênero com a lista de categorias geradas
                var relationsToAdd = categoriesListToRelate
                    .Select(category => new GenresCategories(category.Id, genre.Id))
                    .ToList();

                //salva o relacionamento no banco
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });

        await dbContext.SaveChangesAsync(CancellationToken.None);

        //criamos outra instância do context para garantir que não estava em cache

        var actDbContext = _fixture.CreateDbContext(true);

        var genreRepository = new Repository.GenreRepository(actDbContext);

        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(searchInput, CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(expectedTotalItemsAmount);
        searchResult.Items.Should().HaveCount(expectedReturnedItemsAmount);

        foreach (var item in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == item.Id);
            exampleGenre.Should().NotBeNull();
            item.Name.Should().Be(exampleGenre!.Name);
            item.CreatedAt.Should().Be(exampleGenre!.CreatedAt);
            item.IsActive.Should().Be(exampleGenre!.IsActive);
            item.Categories.Should().HaveCount(exampleGenre.Categories.Count);
            item.Categories.Should().BeEquivalentTo(exampleGenre.Categories);

        }

    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
       string orderBy,
       string order)
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var repo = new Repository.GenreRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var output = await repo.Search(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneGenresListOrdered(
            exampleGenresList, orderBy, searchOrder);

        //não vamos verificar paginação
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull().And.HaveCount(exampleGenresList.Count);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);

        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var expectedItem = expectedOrderedList[indice];
            var outputItem = output.Items[indice];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();

            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem!.Name);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
