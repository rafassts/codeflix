using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace Codeflix.Catalog.UnitTests.Application.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture> { }
public class CreateCategoryTestFixture : BaseFixture
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

    public bool GetRandomIsActive() => (new Random().NextDouble() < 0.5);

    public CreateCategoryInput GetInput() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomIsActive());

    public Mock<ICategoryRepository> GetRepositoryMock() => new();  
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

    
}
