using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.UseCases.Category.Common;
using Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using UseCase = Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace Codeflix.Catalog.UnitTests.Application.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
        => _fixture = fixture;

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Application", "Update Category - Use cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetGategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void UpdateCategory(DomainEntity.Category exampleCategory, UpdateCategoryInput input)
    {
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();

        repo.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);

        var useCase = new UseCase.UpdateCategory(repo.Object, uow.Object);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

        repo.Verify(repo => repo.Get(
           exampleCategory.Id,
           It.IsAny<CancellationToken>()),
           Times.Once);

        repo.Verify(repo => repo.Update(
            exampleCategory,
            It.IsAny<CancellationToken>()),
            Times.Once);

        uow.Verify(uow => uow.Commit(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "Update Category - Use cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();
        var exampleInput = _fixture.GetValidInput();

        repo.Setup(x => x.Get(exampleInput.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category {exampleInput.Id} not found"));

        var useCase = new UseCase.UpdateCategory(repo.Object, uow.Object);

        var task = async ()
            => await useCase.Handle(exampleInput, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();
    }

    [Theory(DisplayName = nameof(UpdateCategoryNoActiveParameter))]
    [Trait("Application", "Update Category - Use cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetGategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void UpdateCategoryNoActiveParameter(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();

        repo.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);

        var useCase = new UseCase.UpdateCategory(repo.Object, uow.Object);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

        repo.Verify(repo => repo.Get(
           exampleCategory.Id,
           It.IsAny<CancellationToken>()),
           Times.Once);

        repo.Verify(repo => repo.Update(
            exampleCategory,
            It.IsAny<CancellationToken>()),
            Times.Once);

        uow.Verify(uow => uow.Commit(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyNameParameter))]
    [Trait("Application", "Update Category - Use cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetGategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async void UpdateCategoryOnlyNameParameter(DomainEntity.Category exampleCategory, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();

        repo.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);

        var useCase = new UseCase.UpdateCategory(repo.Object, uow.Object);

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

        repo.Verify(repo => repo.Get(
           exampleCategory.Id,
           It.IsAny<CancellationToken>()),
           Times.Once);

        repo.Verify(repo => repo.Update(
            exampleCategory,
            It.IsAny<CancellationToken>()),
            Times.Once);

        uow.Verify(uow => uow.Commit(
            It.IsAny<CancellationToken>()),
            Times.Once);
    }




    [Theory(DisplayName = nameof(ThrowWhenCannotUpdateCategory))]
    [Trait("Application", "Update Category - Use cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task ThrowWhenCannotUpdateCategory(UpdateCategoryInput input, string exceptionMessage)
    {
        var exampleCategory = _fixture.GetExampleCategory();
        input.Id = exampleCategory.Id;
        var repo = _fixture.GetRepositoryMock();
        var uow = _fixture.GetUnitOfWorkMock();
        repo.Setup(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(exampleCategory);
        var useCase = new UseCase.UpdateCategory(repo.Object, uow.Object);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>().WithMessage(exceptionMessage);

    }

}
