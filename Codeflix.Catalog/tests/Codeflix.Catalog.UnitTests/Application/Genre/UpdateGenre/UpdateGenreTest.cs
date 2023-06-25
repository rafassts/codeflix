using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
using Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

namespace Codeflix.Catalog.UnitTests.Application.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture) => _fixture=fixture;

    [Fact(DisplayName = nameof(Update))]
    [Trait("Application", "CreateGenre Use Cases")]
    private async Task Update()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepoMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            genreRepoMock.Object,
            uowMock.Object,
            _fixture.GetCategoryRepositoryMock().Object);

        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample,newIsActive);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepoMock.Verify(repo => repo.Update(
          It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
          It.IsAny<CancellationToken>()),
          Times.Once);

        uowMock.Verify(uow => uow.Commit(
          It.IsAny<CancellationToken>()),
          Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "CreateGenre Use Cases")]
    private async Task ThrowWhenNotFound()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();
        genreRepoMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException($"Genre '{exampleId}' not found"));

        var useCase = new UseCase.UpdateGenre(
            genreRepoMock.Object,
            _fixture.GetUnitOfWorkMock().Object,
            _fixture.GetCategoryRepositoryMock().Object);

        var input = new UseCase.UpdateGenreInput(exampleId, _fixture.GetValidGenreName(), _fixture.GetRandomIsActive());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found");
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "CreateGenre Use Cases")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    private async Task ThrowWhenNameIsInvalid(string? name)
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var exampleGenre = _fixture.GetExampleGenre();

        genreRepoMock.Setup(x => x.Get(
              It.Is<Guid>(x => x == exampleGenre.Id),
              It.IsAny<CancellationToken>()
              )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            genreRepoMock.Object,
            _fixture.GetUnitOfWorkMock().Object,
            _fixture.GetCategoryRepositoryMock().Object);

        var input = new UseCase.UpdateGenreInput(
            exampleGenre.Id,
            name!, 
            _fixture.GetRandomIsActive());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("Name should not be null or empty");

    }

    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "CreateGenre Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    private async Task UpdateGenreOnlyName(bool isActive)
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(isActive);
        var newNameExample = _fixture.GetValidGenreName();
       
        genreRepoMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleGenre);

        var useCase = new UseCase.UpdateGenre(
            genreRepoMock.Object,
            uowMock.Object,
            _fixture.GetCategoryRepositoryMock().Object);

        var input = new UseCase.UpdateGenreInput(exampleGenre.Id, newNameExample);

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        genreRepoMock.Verify(repo => repo.Update(
          It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
          It.IsAny<CancellationToken>()),
          Times.Once);

        uowMock.Verify(uow => uow.Commit(
          It.IsAny<CancellationToken>()),
          Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(isActive);
        output.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);

    }
}
