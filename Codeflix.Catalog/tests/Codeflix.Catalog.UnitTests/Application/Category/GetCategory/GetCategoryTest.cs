using FluentAssertions;
using Moq;
using UseCase = Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using Codeflix.Catalog.Application.Exceptions;

namespace Codeflix.Catalog.UnitTests.Application.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Application", "Get Category - Use Cases")]
    public async Task GetCategory()
    {
        var repo = _fixture.GetRepositoryMock();
        var exampleCategory = _fixture.GetExampleCategory();

        repo.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);

        var input = new UseCase.GetCategoryInput(exampleCategory.Id);
        var usecase = new UseCase.GetCategory(repo.Object);

        var output = await usecase.Handle(input, CancellationToken.None);

        repo.Verify(repo => repo.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()),
            Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesntExist))]
    [Trait("Application", "Get Category - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesntExist()
    {
        var repo = _fixture.GetRepositoryMock();
        var exampleGuid = Guid.NewGuid();

        repo.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category {exampleGuid} not found"));

        var input = new UseCase.GetCategoryInput(exampleGuid);
        var usecase = new UseCase.GetCategory(repo.Object);

        var task = async () => await usecase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repo.Verify(repo => repo.Get(
          It.IsAny<Guid>(),
          It.IsAny<CancellationToken>()),
          Times.Once);
    }
}