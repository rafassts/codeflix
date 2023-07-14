
using Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection : ICollectionFixture<UpdateGenreTestFixture> { }
public class UpdateGenreTestFixture : GenreUseCaseBaseFixture
{
}
