using Codeflix.Catalog.Domain.Entity;
using Moq;

namespace Codeflix.Catalog.UnitTests.Application.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture)
    {
        _fixture=fixture;
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("Application","ListCategories Use Cases")]
    public async Task List()
    {
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var repo = _fixture.GetRepositoryMock();
        var input = new ListCategoriesInput(
            page: 2, 
            perPage: 15,
            search: "search-example", 
            sort: "name",
            direction: SearchOrder.Asc);

        repo.Setup(x => x.Search(
            It.IsAny<SearchInput>(
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search = input.Search
                && searchInput.Order = input.Sort
                && searchInput.Order == input.Direction
                ),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(new SearchOutput<Category>(
                currentPage: input.Page,
                perPage: input.PerPage,
                Items: (IReadOnlyList<Category>)categoriesExampleList,
                Total: 70
                ));

        var useCase = new ListCategories(repo.Object);

        var output = await useCase.Handle(input,CancellationToken.None);

        output.Should().NotBeNull();
        

    }

}
