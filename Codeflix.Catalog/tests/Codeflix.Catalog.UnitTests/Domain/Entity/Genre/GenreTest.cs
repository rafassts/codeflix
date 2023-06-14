using FluentAssertions;
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
        var name = _fixture.GetValidName();
        var genre = new DomainEntity.Genre(name, isActive);

        genre.Activate();
        genre.Should().NotBeNull();
        genre.Name.Should().Be(name);
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
        genre.Name.Should().Be(name);
        genre.IsActive.Should().BeFalse();
        genre.CreatedAt.Should().NotBeSameDateAs(default);

    }
}
