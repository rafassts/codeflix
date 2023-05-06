using Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTestFixtureCollection : ICollectionFixture<CreateCategoryApiTestFixture> { }
public class CreateCategoryApiTestFixture : CategoryBaseFixture
{
    public CreateCategoryInput getExampleInput()
       => new(
           GetValidCategoryName(),
           GetValidCategoryDescription(),
           GetRandomIsActive()
       );
}
