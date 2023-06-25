using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.UnitTests.Application.Genre.Common;
using Moq;

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

    public Mock<IGenreRepository> GetGenreRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
}
