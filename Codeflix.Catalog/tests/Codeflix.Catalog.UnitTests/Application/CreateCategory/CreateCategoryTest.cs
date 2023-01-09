using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.UnitTests.Application.CreateCategory;
using FluentAssertions;
using Moq;
using UseCases = Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace Codeflix.Catalog.UnitTests.Application;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{

    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
    {
        _fixture=fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory Use Cases")]
    public async void CreateCategory()
    {

        var repoMock = _fixture.GetRepositoryMock();
        var uowMock  = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(uowMock.Object, repoMock.Object);

        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name) ;
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        repoMock.Verify(repo => repo.Insert(
            It.IsAny<Category>(), 
            It.IsAny<CancellationToken>()), 
            Times.Once);

        uowMock.Verify(uow => uow.Commit(
            It.IsAny<CancellationToken>()), 
            Times.Once);


    }
}
