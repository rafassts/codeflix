namespace Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory;

using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using Moq;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application", "Delete Category - Use Cases")]
    public async void DeleteCategory()
    {
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();
        var validCategory = _fixture.GetExampleCategory();
        var input = new DeleteCategoryInput(validCategory.Id);
        repo.Setup(x => x.Get(validCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(validCategory);
        var useCase = new DeleteCategory(repo.Object, uow.Object);

        await useCase.Handle(input, CancellationToken.None);

        repo.Verify(repo => repo.Get(
          validCategory.Id,
          It.IsAny<CancellationToken>()),
          Times.Once);

        repo.Verify(repo => repo.Delete(
           validCategory,
           It.IsAny<CancellationToken>()),
           Times.Once);

        uow.Verify(uow => uow.Commit(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(TrowWenCategoryNotFound))]
    [Trait("Application", "Delete Category - Use Cases")]
    public async void TrowWenCategoryNotFound()
    {
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();
        var id = Guid.NewGuid();
        var input = new DeleteCategoryInput(id);

        repo.Setup(x => x.Get(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category {id} not found"));

        var useCase = new DeleteCategory(repo.Object, uow.Object);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
    }
}
