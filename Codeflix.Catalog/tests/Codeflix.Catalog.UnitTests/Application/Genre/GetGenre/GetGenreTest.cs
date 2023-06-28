using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Moq;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.GetGenre;

namespace Codeflix.Catalog.UnitTests.Application.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    public readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture getGenreTestFixture) 
        => _fixture = getGenreTestFixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Application", "GetGenre Use Cases")]
    private async Task GetGenre()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var exampleGenre = _fixture.GetExampleGenre(categoriesIds: _fixture.GetRandomIdsList());

        genreRepoMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.GetGenre(genreRepoMock.Object);

        var input = new UseCase.GetGenreInput(exampleGenre.Id);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepoMock.Verify(repo => repo.Get(
          It.Is<Guid>(x => x == exampleGenre.Id),
          It.IsAny<CancellationToken>()),
          Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleGenre.Categories.Count);

        foreach (var item in output.Categories)
            output.Categories.Should().Contain(item);
        
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "GetGenre Use Cases")]
    private async Task ThrowWhenNotFound()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();

        var exampleId = Guid.NewGuid();

        genreRepoMock.Setup(x => x.Get(
             It.Is<Guid>(x => x == exampleId),
             It.IsAny<CancellationToken>()
             )).ThrowsAsync(new NotFoundException($"Genre '{exampleId} not found'"));

        var useCase = new UseCase.GetGenre(genreRepoMock.Object);

        var input = new UseCase.GetGenreInput(exampleId);

       var action = async () 
            => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId} not found'");

       genreRepoMock.Verify(repo => repo.Get(
          It.Is<Guid>(x => x == exampleId),
          It.IsAny<CancellationToken>()),
          Times.Once);

    }

}
