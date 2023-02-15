using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;

namespace Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection
    : ICollectionFixture<CategoryRepositoryTestFixture> { }

public class CategoryRepositoryTestFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var name = "";
        while (name.Length < 3)
            name = Faker.Commerce.Categories(1)[0];

        if (name.Length > 255)
            name = name[..255];

        return name;
    }
    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();

        if (description.Length > 10000)
            description = description[..10000];

        return description;
    }
    public bool GetRandomIsActive() => new Random().NextDouble() < 0.5;
   
    public Category GetExampleCategory() => new(
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        GetRandomIsActive()
    );

    public List<Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(1,length).Select(_ => GetExampleCategory()).ToList();
   

    public CodeflixCategoryDbContext CreateDbContext()
        => new (
            new DbContextOptionsBuilder<CodeflixCategoryDbContext>()
                .UseInMemoryDatabase("integration-tests-db")
                .Options
            );

}
