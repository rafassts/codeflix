﻿using Codeflix.Catalog.Infra.Data.EF;
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

        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);
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
        var repo = new Repository.CategoryRepository(dbContext);

        var dbCategory = await repo.Get(exampleCategory.Id,CancellationToken.None); 

        dbCategory.Should().NotBeNull();
        dbCategory?.Id.Should().Be(exampleCategory.Id);
        dbCategory?.Name.Should().Be(exampleCategory.Name);
        dbCategory?.Description.Should().Be(exampleCategory.Description);
        dbCategory?.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory?.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }
}
