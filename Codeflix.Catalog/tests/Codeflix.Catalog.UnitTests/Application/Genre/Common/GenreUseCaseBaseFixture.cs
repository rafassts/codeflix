using Codeflix.Catalog.UnitTests.Common;

namespace Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCaseBaseFixture : BaseFixture
{
    public string GetValidGenreName() => Faker.Commerce.Categories(1)[0];
}
