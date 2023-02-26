using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
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
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
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
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
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
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
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
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
       
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
        CodeflixCategoryDbContext dbContext = _fixture.CreateDbContext();
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

    
}