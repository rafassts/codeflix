using Codeflix.Catalog.Application.UseCases.Genre.Common;
using Moq;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using Codeflix.Catalog.Application.Exceptions;

namespace Codeflix.Catalog.UnitTests.Application.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Application", "DeleteGenre Use Cases")]
    private async Task Delete()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        
        genreRepoMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.DeleteGenre(
            genreRepoMock.Object,
            uowMock.Object);

        var input = new UseCase.DeleteGenreInput(exampleGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        genreRepoMock.Verify(repo => repo.Get(
          It.Is<Guid>(x => x == exampleGenre.Id),
          It.IsAny<CancellationToken>()),
          Times.Once);

        genreRepoMock.Verify(repo => repo.Delete(
          It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
          It.IsAny<CancellationToken>()),
          Times.Once);

        uowMock.Verify(uow => uow.Commit(
          It.IsAny<CancellationToken>()),
          Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "DeleteGenre Use Cases")]
    private async Task ThrowWhenNotFound()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();

        var exampleId = Guid.NewGuid();

        genreRepoMock.Setup(x => x.Get(
             It.Is<Guid>(x => x == exampleId),
             It.IsAny<CancellationToken>()
             )).ThrowsAsync(new NotFoundException($"Genre '{exampleId} not found'"));

        var useCase = new UseCase.DeleteGenre(genreRepoMock.Object, uowMock.Object);

        var input = new UseCase.DeleteGenreInput(exampleId);

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
