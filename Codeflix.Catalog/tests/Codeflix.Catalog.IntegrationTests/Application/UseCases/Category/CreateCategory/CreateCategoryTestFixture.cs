
using Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection 
    : ICollectionFixture<CreateCategoryTestFixture> { }

public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
{
    public CreateCategoryInput GetInput()
    {
        var category = GetExampleCategory();
        return new CreateCategoryInput(category.Name, category.Description, category.IsActive);
    }

    public CreateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name[..2];
        return invalidInputShortName;
    }

    public CreateCategoryInput GetInvalidInputTooLongName()
    {
        var invalidInputLongName = GetInput();
        invalidInputLongName.Name = "";

        while (invalidInputLongName.Name.Length <= 250)
            invalidInputLongName.Name = $"{invalidInputLongName.Name} {Faker.Commerce.ProductDescription()}";

        return invalidInputLongName;
    }

    public CreateCategoryInput GetInvalidInputDescriptionNull()
    {
        var invalidInputDescriptionNull = GetInput();
        invalidInputDescriptionNull.Description = null;
        return invalidInputDescriptionNull;
    }

    public CreateCategoryInput GetInvalidInputDescriptionTooLong()
    {
        var invalidInputDescriptionTooLong = GetInput();

        while (invalidInputDescriptionTooLong.Description.Length <= 10000)
            invalidInputDescriptionTooLong.Description =
                $"{invalidInputDescriptionTooLong.Description} {Faker.Commerce.ProductDescription()}";

        return invalidInputDescriptionTooLong;
    }
}
