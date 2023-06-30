using Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Codeflix.Catalog.UnitTests.Application.Genre.Common;

namespace Codeflix.Catalog.UnitTests.Application.Genre.ListGenre;

[CollectionDefinition(nameof(ListGenreTestFixture))]
public class ListGenreTestFixtureCollection : ICollectionFixture<ListGenreTestFixture> { }

public class ListGenreTestFixture : GenreUseCaseBaseFixture
{
    public ListGenresInput GetExampleInput()
    {
        var random = new Random();

        return new ListGenresInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc);
    }
}
