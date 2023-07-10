
using Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }
public class CreateGenreTestFixture : GenreUseCaseBaseFixture
{
    public CreateGenreInput GetExampleInput()
        => new CreateGenreInput(GetValidGenreName(), GetRandomIsActive());
}
