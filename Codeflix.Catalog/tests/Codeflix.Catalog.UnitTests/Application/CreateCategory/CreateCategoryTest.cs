using Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Exceptions;
using Codeflix.Catalog.UnitTests.Application.CreateCategory;
using Codeflix.Catalog.UnitTests.Domain.Entity.Category;
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

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateAggregate))]
    [Trait("Application", "Throw Error CreateCategory Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async void ThrowWhenCantInstantiateAggregate(CreateCategoryInput input, string exceptionMessage)
    {

        var useCase = new UseCases.CreateCategory(
            _fixture.GetUnitOfWorkMock().Object,
            _fixture.GetRepositoryMock().Object);

       Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }
    
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputList = new List<object[]>();

        //nome não pode ser null, não pode ser menor que 3 e maior que 255 caracteres
        //descrição não pode ser null e tamanho máximo 10k

        var invalidInputShortName = fixture.GetInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);

        invalidInputList.Add(new object[] 
        { 
            invalidInputShortName,
            "Name should have at least 3 characters"
        });

        var invalidInputLongName = fixture.GetInput();
        invalidInputLongName.Name = "";

        while (invalidInputLongName.Name.Length <= 250)
            invalidInputLongName.Name = $"{invalidInputLongName.Name} {fixture.Faker.Commerce.ProductDescription()}";

        invalidInputList.Add(new object[]
        {
            invalidInputLongName,
            "Name should have no more than 250 characters"
        });

        return invalidInputList;
    }
}
