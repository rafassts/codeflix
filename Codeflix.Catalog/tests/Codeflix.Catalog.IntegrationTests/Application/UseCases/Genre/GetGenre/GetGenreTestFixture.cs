
using Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection : ICollectionFixture<GetGenreTestFixture> { }

public class GetGenreTestFixture : GenreUseCaseBaseFixture
{

}
