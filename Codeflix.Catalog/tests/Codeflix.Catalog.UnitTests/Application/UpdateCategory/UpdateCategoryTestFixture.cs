using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.UnitTests.Application.CreateCategory;
using Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }
public class UpdateCategoryTestFixture : BaseFixture
{
    public Mock<ICategoryRepository> GetRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

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
    public Category GetExampleCategory() => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomIsActive());

    public UpdateCategoryInput GetValidInput(Guid? id = null)
        => new (
              id ?? Guid.NewGuid(),
              GetValidCategoryName(),
              GetValidCategoryDescription(),
              GetRandomIsActive());


    public UpdateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetValidInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;
    }

    public UpdateCategoryInput GetInvalidInputTooLongName()
    {
        var invalidInputLongName = GetValidInput();
        invalidInputLongName.Name = "";

        while (invalidInputLongName.Name.Length <= 250)
            invalidInputLongName.Name = $"{invalidInputLongName.Name} {Faker.Commerce.ProductDescription()}";

        return invalidInputLongName;
    }

    public UpdateCategoryInput GetInvalidInputDescriptionTooLong()
    {
        var invalidInputDescriptionTooLong = GetValidInput();

        while (invalidInputDescriptionTooLong.Description.Length <= 10000)
            invalidInputDescriptionTooLong.Description =
                $"{invalidInputDescriptionTooLong.Description} {Faker.Commerce.ProductDescription()}";

        return invalidInputDescriptionTooLong;
    }
}
