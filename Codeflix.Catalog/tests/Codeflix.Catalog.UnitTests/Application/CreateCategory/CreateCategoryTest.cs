using Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using Moq;
using UseCases = Codeflix.Catalog.Application.UseCases.CreateCategory;

namespace Codeflix.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTest
{
    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        var input = new CreateCategoryInput("Name", "Description", true);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        (output.Id != null && output.Id != default(Guid)).Should().BeTrue();
        output.Name.Should().Be("Name");
        output.Description.Should().Be("Description");
        output.IsActive.Should().Be(true);
        (output.CreatedAt != null && output.CreatedAt != default(DateTime)).Should.BeTrue();
        repositoryMock.Verify(repo => repo.Create(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);

    }
}
