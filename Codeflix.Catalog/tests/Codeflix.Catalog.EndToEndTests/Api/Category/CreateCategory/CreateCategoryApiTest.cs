using Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using System.Net;

namespace Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("EndToEnd/API", "Category/Create - Endpoints")]
    public async Task CreateCategory()
    {
        //var input = _fixture.getExampleInput();

        //var (response, output) = await _fixture.
        //    ApiClient.Post<ApiResponse<CategoryModelOutput>>("/categories",input);

        //response.Should().NotBeNull();
        //response!.StatusCode.Should().Be(HttpStatusCode.Created);
        
        //output.Should().NotBeNull();
        //output!.Data.Should().NotBeNull();
        //output.Data.Name.Should().Be(input.Name);
        //output.Data.Description.Should().Be(input.Description);
        //output.Data.IsActive.Should().Be(input.IsActive);
        //output.Data.Id.Should().NotBeEmpty();
        //output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        //var dbCategory = await _fixture.Persistence.GetById(output.Data.Id);

        //dbCategory.Should().NotBeNull();
        //dbCategory!.Name.Should().Be(input.Name);
        //dbCategory.Description.Should().Be(input.Description);
        //dbCategory.IsActive.Should().Be(input.IsActive);
        //dbCategory.Id.Should().NotBeEmpty();
        //dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }
}
