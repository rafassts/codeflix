using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.IntegrationTests.Base;
using DomainEntity = Codeflix.Catalog.Domain.Entity;


namespace Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[CollectionDefinition(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTestFixtureCollection : ICollectionFixture<GenreRepositoryTestFixture> { }

public class GenreRepositoryTestFixture : BaseFixture
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

    public List<Genre> GetExampleGenresListWithNames(List<string> names)
       => names.Select(name =>
       {
           var genre = GetExampleGenre();
           genre.Update(name);
           return genre;
       }).ToList();

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
    public Category GetExampleCategory() => new(
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        GetRandomIsActive()
    );
    public List<Category> GetExampleCategoriesList(int length = 10)
        => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();
}
