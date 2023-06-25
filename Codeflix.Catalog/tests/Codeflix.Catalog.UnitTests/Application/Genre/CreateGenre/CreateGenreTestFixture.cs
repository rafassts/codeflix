using Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Codeflix.Catalog.UnitTests.Application.Genre.Common;

namespace Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }
public class CreateGenreTestFixture : GenreUseCaseBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new(GetValidGenreName(), GetRandomIsActive());

    public CreateGenreInput GetExampleInput(string? name)
      => new(name!, GetRandomIsActive());

    public CreateGenreInput GetExampleInputWithCategories()
    {
        var categoriesCount = (new Random().Next(1,10));
        var categoriesIds = Enumerable.Range(1, categoriesCount)
            .Select(_ => Guid.NewGuid())
            .ToList();

        return new CreateGenreInput(GetValidGenreName(), GetRandomIsActive(), categoriesIds);
    }
}
