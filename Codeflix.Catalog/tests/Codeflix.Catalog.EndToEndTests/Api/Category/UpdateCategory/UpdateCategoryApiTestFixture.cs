using Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTestFixtureCollection
    : ICollectionFixture<UpdateCategoryApiTestFixture>
{ }

public class UpdateCategoryApiTestFixture
    : CategoryBaseFixture
{
    public UpdateCategoryInput GetExampleInput(Guid? id)
        => new(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomIsActive()
        );
}
