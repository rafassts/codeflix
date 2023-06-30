using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
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
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var repo = new Repository.CategoryRepository(dbContext);
       
        await repo.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        //feito isso por conta do tracking do ef core
        var dbCategory = await (_fixture.CreateDbContext(true))
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
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        //feito isso por conta do tracking do ef core
        var repo = new Repository.CategoryRepository(_fixture.CreateDbContext(true));

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
        var dbContext = _fixture.CreateDbContext();
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
        var dbContext = _fixture.CreateDbContext();
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

        //feito isso por conta do tracking do ef core
        var dbCategory = await (_fixture.CreateDbContext(true))
            .Categories
            .FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory?.Id.Should().Be(exampleCategory.Id);
        dbCategory?.Name.Should().Be(exampleCategory.Name);
        dbCategory?.Description.Should().Be(exampleCategory.Description);
        dbCategory?.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory?.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Delete()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);

        await repo.Delete(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        //feito isso por conta do tracking do ef core
        var dbCategory = await (_fixture.CreateDbContext(true))
            .Categories
            .FindAsync(exampleCategory.Id);

        //retorna null porque estou procurando direto e não pelo repo (throw)
        dbCategory.Should().BeNull();
        
    }

    [Fact(DisplayName = nameof(SearcReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearcReturnsListAndTotal()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await repo.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull().And.HaveCount(exampleCategoriesList.Count);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);

        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category => category.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    
    }

    [Fact(DisplayName = nameof(SearcReturnsEmptyWhenNoneIsFound))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearcReturnsEmptyWhenNoneIsFound()
    {
        var dbContext = _fixture.CreateDbContext();
       
        var repo = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await repo.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull().And.HaveCount(0);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);

    }

    [Theory(DisplayName = nameof(SearcReturnsPaginatedListAndTotal))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData(10,1,5,5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearcReturnsPaginatedListAndTotal(
        int categoriesAmount, 
        int page, 
        int perPage,
        int expectedItemsAmount)
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(categoriesAmount);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var output = await repo.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull().And.HaveCount(expectedItemsAmount);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(categoriesAmount);

        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category => category.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }

    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
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
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(new List<string>
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

        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var output = await repo.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull().And.HaveCount(expectedReturnedItemsAmount);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItemsAmount);

        foreach (Category outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(category => category.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
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
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);

        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repo = new Repository.CategoryRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy , searchOrder);

        var output = await repo.Search(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
            exampleCategoriesList, orderBy, searchOrder);

        //não vamos verificar paginação
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull().And.HaveCount(exampleCategoriesList.Count);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);

        for(int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var expectedItem = expectedOrderedList[indice];
            var outputItem = output.Items[indice];

            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();

            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Name.Should().Be(expectedItem!.Name);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }

    }
}