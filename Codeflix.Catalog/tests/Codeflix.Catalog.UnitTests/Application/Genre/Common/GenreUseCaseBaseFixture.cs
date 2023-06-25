using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.UnitTests.Common;
using Moq;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCaseBaseFixture : BaseFixture
{
    public string GetValidGenreName() => Faker.Commerce.Categories(1)[0];

    public Mock<IGenreRepository> GetGenreRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();

    public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        var genre = new DomainEntity.Genre(GetValidGenreName(), isActive ?? GetRandomIsActive());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<Guid> GetRandomIdsList(int? count = null)
        => Enumerable
            .Range(1, count ?? (new Random()).Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();
    
}
