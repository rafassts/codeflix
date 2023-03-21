using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using UseCase = Codeflix.Catalog.Application.UseCases.Category.GetCategory;
namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
    {
        _fixture=fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Integration/Application", "Get Category - Use Cases")]
    public async Task GetCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();

        dbContext.Categories.Add(exampleCategory);
        dbContext.SaveChanges();

        var repo = new CategoryRepository(dbContext);

        var input = new UseCase.GetCategoryInput(exampleCategory.Id);
        var usecase = new UseCase.GetCategory(repo);

        var output = await usecase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesntExist))]
    [Trait("Integration/Application", "Get Category - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesntExist()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        //insere só por graça mesmo
        dbContext.Categories.Add(exampleCategory);
        dbContext.SaveChanges();

        var repo = new CategoryRepository(dbContext);

        //para não encontrar
        var input = new UseCase.GetCategoryInput(Guid.NewGuid());
        var usecase = new UseCase.GetCategory(repo);

        var task = async () => await usecase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found");

    }
}
