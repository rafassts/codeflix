using Codeflix.Catalog.IntegrationTests.Base;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
public class GenreUseCaseBaseFixture : BaseFixture
{
    //genre
    public string GetValidGenreName() => Faker.Commerce.Categories(1)[0];
    public bool GetRandomIsActive() => new Random().NextDouble() < 0.5;
    public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        var genre = new DomainEntity.Genre(GetValidGenreName(), isActive ?? GetRandomIsActive());
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<DomainEntity.Genre> GetExampleGenresList(int count = 10) =>
        Enumerable
            .Range(1, count)
            .Select(_ => GetExampleGenre()).ToList();

    //categories
    public string GetValidCategoryName()
    {
        var name = "";
        while (name.Length < 3)
            name = Faker.Commerce.Categories(1)[0];

        if (name.Length > 255)
            name = name[..255];

        return name;
    }
    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();

        if (description.Length > 10000)
            description = description[..10000];

        return description;
    }
    public DomainEntity.Category GetExampleCategory() => new(
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        GetRandomIsActive()
    );
    public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
}
