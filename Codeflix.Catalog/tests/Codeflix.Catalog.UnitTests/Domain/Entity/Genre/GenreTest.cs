using Codeflix.Catalog.Domain.Exceptions;
using Codeflix.Catalog.UnitTests.Domain.Entity.Category;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain","Genre - Aggregates")]
    public void Instantiate()
    {
        var name = _fixture.GetValidName();
        var dateTimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(name);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(name);
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt > dateTimeBefore).Should().BeTrue();
        (genre.CreatedAt <= dateTimeAfter).Should().BeTrue();

    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var name = _fixture.GetValidName();
        var dateTimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(name, isActive);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(name);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt > dateTimeBefore).Should().BeTrue();
        (genre.CreatedAt <= dateTimeAfter).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive);

        genre.Activate();
        genre.Should().NotBeNull();
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().NotBeSameDateAs(default);

    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var name = _fixture.GetValidName();
        var genre = new DomainEntity.Genre(name, isActive);

        genre.Deactivate();
        genre.Should().NotBeNull();
        genre.IsActive.Should().BeFalse();
        genre.CreatedAt.Should().NotBeSameDateAs(default);

    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Update()
    {

        var genre = _fixture.GetExampleGenre();
        var newName = _fixture.GetValidName();
        var oldIsActive = genre.IsActive;

        genre.Update(newName);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(newName);
        genre.IsActive.Should().Be(oldIsActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(InstantiateThhrowErrorWhenNameEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void InstantiateThhrowErrorWhenNameEmpty(string? name)
    {
        Action action = () => new DomainEntity.Genre(name!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty");

    }

    [Theory(DisplayName = nameof(UpdateThrowWhenNameIsEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateThrowWhenNameIsEmpty(string? name)
    {

        var genre = _fixture.GetExampleGenre();
     
        var action = () => genre.Update(name!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be null or empty");

    }

    [Fact(DisplayName = nameof(AddCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddCategories()
    {
        var genre = _fixture.GetExampleGenre();
        var categoryGuid = Guid.NewGuid();
        var categoryGuid2 = Guid.NewGuid();

        genre.AddCategory(categoryGuid);
        genre.AddCategory(categoryGuid2);

        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().Contain(categoryGuid);
        genre.Categories.Should().Contain(categoryGuid);
    }

    [Fact(DisplayName = nameof(RemoveCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveCategories()
    {
        var categoryGuid = Guid.NewGuid();

        var genre = _fixture.GetExampleGenre(
            categoriesIdsList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                categoryGuid,
                Guid.NewGuid()
            });

        genre.RemoveCategory(categoryGuid);

        genre.Categories.Should().HaveCount(3);
        genre.Categories.Should().NotContain(categoryGuid);

    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveAllCategories()
    {
        var categoryGuid = Guid.NewGuid();

        var genre = _fixture.GetExampleGenre(
            categoriesIdsList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                categoryGuid,
                Guid.NewGuid()
            });

        genre.RemoveAllCategories();

        genre.Categories.Should().HaveCount(0);

    }
}
