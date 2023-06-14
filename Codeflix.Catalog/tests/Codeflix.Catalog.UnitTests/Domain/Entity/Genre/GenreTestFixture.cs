using Codeflix.Catalog.UnitTests.Common;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[CollectionDefinition(nameof(GenreTestFixture))]
public class GenreTestFixtureCollection : ICollectionFixture<GenreTestFixture> { }
public class GenreTestFixture : BaseFixture
{

    public string GetValidName() => Faker.Commerce.Categories(1)[0];
}
