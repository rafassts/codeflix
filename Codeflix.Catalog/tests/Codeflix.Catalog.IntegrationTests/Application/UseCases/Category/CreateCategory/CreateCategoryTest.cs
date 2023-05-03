using Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Codeflix.Catalog.Domain.Exceptions;
using Codeflix.Catalog.Infra.Data.EF;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AppUseCases = Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
    {
        _fixture=fixture;
    }

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Integration/Application", "CreateCategory Use Cases")]
    public async void CreateCategory()
    {

        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);

        var useCase = new AppUseCases.CreateCategory(uow,repo);

        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        //lê do banco para verificar se deu certo, novo contexto para não usar o tracking  
        var dbContext2 = _fixture.CreateDbContext(true);
        var dbCategory = await dbContext2
            .Categories
            .FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);

    }

    [Fact(DisplayName = nameof(CreateCategoryOnlyWitName))]
    [Trait("Integration/Application", "CreateCategory Use Cases")]
    public async void CreateCategoryOnlyWitName()
    {

        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);

        var useCase = new AppUseCases.CreateCategory(uow, repo);

        var input = new CreateCategoryInput(_fixture.GetInput().Name);

        var output = await useCase.Handle(input, CancellationToken.None);

        //novo contexto para não usar o tracking  
        var dbContext2 = _fixture.CreateDbContext(true);
        var dbCategory = await dbContext2
            .Categories
            .FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be("");
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().Be(true);
        output.CreatedAt.Should().NotBeSameDateAs(default);

    }

    [Fact(DisplayName = nameof(CreateCategoryOnlyWitNameAndDescription))]
    [Trait("Integration/Application", "CreateCategory Use Cases")]
    public async void CreateCategoryOnlyWitNameAndDescription()
    {

        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);

        var useCase = new AppUseCases.CreateCategory(uow, repo);

        var exampleInput = _fixture.GetInput();
        var input = new CreateCategoryInput(exampleInput.Name, exampleInput.Description);

        var output = await useCase.Handle(input, CancellationToken.None);

        //novo contexto para não usar o tracking  
        var dbContext2 = _fixture.CreateDbContext(true);
        var dbCategory = await dbContext2
            .Categories
            .FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(true);
        output.CreatedAt.Should().NotBeSameDateAs(default);

    }

    [Theory(DisplayName = nameof(ThrowWenCantInstantiateCategory))]
    [Trait("Integration/Application", "Throw Error CreateCategory Use Cases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 4,
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async void ThrowWenCantInstantiateCategory(
        CreateCategoryInput input,
        string expectedMessage)
    {

        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);
        var uow = new UnitOfWork(dbContext);

        var useCase = new AppUseCases.CreateCategory(uow, repo);

        var exampleInput = _fixture.GetInput();

        var task = async() => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);

        //não está rodando banco em paralelo, então está ok fazer esse teste
        var dbCategoriesList = _fixture.CreateDbContext(true).Categories.AsNoTracking().ToList();

        dbCategoriesList.Should().HaveCount(0);

    }

}
