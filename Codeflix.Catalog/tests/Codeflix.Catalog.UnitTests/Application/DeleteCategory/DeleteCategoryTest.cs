namespace Codeflix.Catalog.UnitTests.Application.DeleteCategory;

using Codeflix.Catalog.Domain.Entity;
using Moq;
using UseCase = Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture=fixture;
    }

    [Fact(DisplayName = "")]
    [Trait("Application", "Delete Category - Use Cases")]
    public async void DeleteCategory()
    {
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();
        var validCategory = _fixture.GetValidCategory();
        var input = new DeleteCategoryInput(validCategory.Id);
        repo.Setup(x => x.Get(validCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(validCategory);
        var useCase = new DeleteCategory(repo, uow);

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
}
