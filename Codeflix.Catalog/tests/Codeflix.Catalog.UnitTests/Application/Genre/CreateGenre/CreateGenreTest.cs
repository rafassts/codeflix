using FluentAssertions;
using Moq;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture) => _fixture=fixture;

    [Fact(DisplayName = nameof(Create))]
    [Trait("Application", "CreateGenre Use Cases")]
    public async Task Create()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
       
        var useCase = new UseCase.CreateGenre(
            genreRepoMock.Object,
            uowMock.Object, 
            categoryRepoMock.Object);

        var input = _fixture.GetExampleInput();
        var dateTimeBefore = DateTime.Now;
        
        var output = await useCase.Handle(input, CancellationToken.None);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        genreRepoMock.Verify(repo => repo.Insert(
           It.IsAny<DomainEntity.Genre>(),
           It.IsAny<CancellationToken>()),
           Times.Once);

        uowMock.Verify(uow => uow.Commit(
          It.IsAny<CancellationToken>()),
          Times.Once);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.Categories.Should().HaveCount(0);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        (output.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (output.CreatedAt <= dateTimeAfter).Should().BeTrue();

    }

    [Fact(DisplayName = nameof(CreateWithRelatedCategories))]
    [Trait("Application", "CreateGenre Use Cases")]
    public async Task CreateWithRelatedCategories()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
       
        var useCase = new UseCase.CreateGenre(
            genreRepoMock.Object,
            uowMock.Object, 
            categoryRepoMock.Object);

        var input = _fixture.GetExampleInputWithCategories();

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepoMock.Verify(repo => repo.Insert(
           It.IsAny<DomainEntity.Genre>(),
           It.IsAny<CancellationToken>()),
           Times.Once);

        uowMock.Verify(uow => uow.Commit(
          It.IsAny<CancellationToken>()),
          Times.Once);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);

        input.CategoriesIds?.ForEach(id => output.Categories.Should().Contain(id));

        output.CreatedAt.Should().NotBeSameDateAs(default);

    }

    [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
    [Trait("Application", "CreateGenre Use Cases")]
    public async Task CreateThrowWhenRelatedCategoryNotFound()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();
        var categoryRepoMock = _fixture.GetCategoryRepositoryMock();
        var input = _fixture.GetExampleInputWithCategories();
        var exampeGuid = input.CategoriesIds![^1]; //pega o último item

        categoryRepoMock.Setup(
            x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            (IReadOnlyList<Guid>) input.CategoriesIds.FindAll(x => x != exampeGuid)
          ); //retorna todos menos o último

        var uowMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateGenre(
            genreRepoMock.Object,
            uowMock.Object,
            categoryRepoMock.Object);

        var action = async() =>
            await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage($"Related category id (or ids) not found: '{exampeGuid}'");

        categoryRepoMock.Verify(x =>
           x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

    }

}
