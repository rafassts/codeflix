using Codeflix.Catalog.Api.ApiModels.Response;
using Codeflix.Catalog.Application.UseCases.Category.Common;
using Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Xunit.Abstractions;

namespace Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[Collection(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTest : IDisposable
{
    private readonly ListCategoriesApiTestFixture _fixture;
    private readonly ITestOutputHelper _output;

    public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture, ITestOutputHelper output)
    {
        _fixture=fixture;
        _output=output;
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotalDefault))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalDefault()
    {
        var defaultPerPage = 15;
        var defaultPage = 1;
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);

        await _fixture.Persistence.InsertList(exampleCategoriesList);
         
        var (response, output) = await _fixture
            .ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(defaultPerPage);
        output.Meta.CurrentPage.Should().Be(defaultPage);
        output.Meta.PerPage.Should().Be(defaultPerPage);

        foreach (var outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();

            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceIsEmpty))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceIsEmpty()
    {

        var (response, output) = await _fixture
            .ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {

        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var input = new ListCategoriesInput(page: 1, perPage: 5);

        var (response, output) = await _fixture
            .ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);

        foreach (var outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();

            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int quantityCategoriesToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var input = new ListCategoriesInput(page: page, perPage: perPage);

        var (response, output) = await _fixture
            .ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(expectedQuantityItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);

        foreach (var outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();

            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
         string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var categoryNamesList = new List<string>() {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        };

        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(categoryNamesList);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var input = new ListCategoriesInput(page: page, perPage: perPage,search: search);

        var (response, output) = await _fixture
            .ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(expectedQuantityTotalItems);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);

        foreach (var outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();

            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }

    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("", "asc")]
    public async Task ListOrdered(string orderBy, string order)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;

        var input = new ListCategoriesInput(page: 1, perPage: 20, sort: orderBy, dir: inputOrder);

        var (response, output) = await _fixture
            .ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
            exampleCategoriesList,
            input.Sort,
            input.Dir);

        //problema na ordenação, vamos ver como está vindo com o test helper - daria para ver no debug, mas facilita
        //criados no mesmo horário - banco em memória diferenciava pela última casa, mas no banco no container não
        var count = 0;
        var expectedArray = expectedOrderedList
            .Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}");

        count = 0;
        var outputArray = output.Data
           .Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}");

        _output.WriteLine("Expects...");
        _output.WriteLine(String.Join('\n',expectedArray));
        _output.WriteLine("Outputs...");
        _output.WriteLine(String.Join('\n', outputArray));


        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var outputItem = output.Data[indice];
            var exampleItem = expectedOrderedList[indice];
            outputItem.Should().NotBeNull();
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        }
    }


    [Theory(DisplayName = nameof(ListOrderedDates))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task ListOrderedDates(string orderBy, string order)
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;

        var input = new ListCategoriesInput(page: 1, perPage: 20, sort: orderBy, dir: inputOrder);

        var (response, output) = await _fixture
            .ApiClient.Get<TestApiResponseList<CategoryModelOutput>>($"/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output!.Meta!.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);

        //testa se está ordenado por data, pois pode ser igual até os segundos, então tem que ser maior ou igual 
        //para asc ou menor ou igual para desc
        DateTime? lastItemDate = null;
        foreach (var outputItem in output.Data!)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();

            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds().Should().Be(exampleItem.CreatedAt.TrimMillisseconds());

            if (lastItemDate != null)
            {
                if (order == "asc")
                    Assert.True(outputItem.CreatedAt >= lastItemDate);
                else
                    Assert.True(outputItem.CreatedAt <= lastItemDate);

            }

            lastItemDate = outputItem.CreatedAt;
        }
    }

    public void  Dispose()
      => _fixture.CleanPersistence();
}
