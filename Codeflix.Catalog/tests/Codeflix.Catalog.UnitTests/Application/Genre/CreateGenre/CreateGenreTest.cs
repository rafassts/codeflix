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
        var repoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCase.CreateGenre(repoMock.Object, uowMock.Object);
        var input = _fixture.GetExampleInput();
        var dateTimeBefore = DateTime.Now;
        
        var output = await useCase.Handle(input, CancellationToken.None);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        repoMock.Verify(repo => repo.Insert(
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

}
