using Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using UseCase = Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.UnitTests.Application.Category.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCategories Use Cases")]
    public async Task List()
    {
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var repo = _fixture.GetRepositoryMock();

        var input = _fixture.GetExampleInput();

        var outputRepoSearch = new SearchOutput<DomainEntity.Category>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: (IReadOnlyList<DomainEntity.Category>)categoriesExampleList,
                total: new Random().Next(50, 200));

        repo.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepoSearch);

        var useCase = new UseCase.ListCategories(repo.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepoSearch.CurrentPage);
        output.PerPage.Should().Be(outputRepoSearch.PerPage);
        output.Total.Should().Be(outputRepoSearch.Total);
        output.Items.Should().HaveCount(outputRepoSearch.Items.Count);

        output.Items.ToList().ForEach(outputItem =>
        {
            var repoCategory = outputRepoSearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);

            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repoCategory!.Name);
            outputItem.Description.Should().Be(repoCategory!.Description);
            outputItem.IsActive.Should().Be(repoCategory!.IsActive);
            outputItem.CreatedAt.Should().Be(repoCategory!.CreatedAt);
        });

        repo.Verify(x => x.Search(
           It.Is<SearchInput>(
               searchInput =>
               searchInput.Page == input.Page
               && searchInput.PerPage == input.PerPage
               && searchInput.Search == input.Search
               && searchInput.OrderBy == input.Sort
               && searchInput.Order == input.Dir
               ),
           It.IsAny<CancellationToken>()
           ), Times.Once);
    }

    [Theory(DisplayName = nameof(ListWithoutAllParameters))]
    [Trait("Application", "ListCategories Use Cases")]
    [MemberData(
        nameof(ListCategoriesTestDataGenerator.GetInputsWithoutAllParameters),
        parameters: 12,
        MemberType = typeof(ListCategoriesTestDataGenerator))]
    public async Task ListWithoutAllParameters(ListCategoriesInput input)
    {
        var categoriesExampleList = _fixture.GetExampleCategoriesList();
        var repo = _fixture.GetRepositoryMock();

        var outputRepoSearch = new SearchOutput<DomainEntity.Category>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: (IReadOnlyList<DomainEntity.Category>)categoriesExampleList,
                total: new Random().Next(50, 200));

        repo.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepoSearch);

        var useCase = new UseCase.ListCategories(repo.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepoSearch.CurrentPage);
        output.PerPage.Should().Be(outputRepoSearch.PerPage);
        output.Total.Should().Be(outputRepoSearch.Total);
        output.Items.Should().HaveCount(outputRepoSearch.Items.Count);

        output.Items.ToList().ForEach(outputItem =>
        {
            var repoCategory = outputRepoSearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);

            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repoCategory!.Name);
            outputItem.Description.Should().Be(repoCategory!.Description);
            outputItem.IsActive.Should().Be(repoCategory!.IsActive);
            outputItem.CreatedAt.Should().Be(repoCategory!.CreatedAt);
        });

        repo.Verify(x => x.Search(
           It.Is<SearchInput>(
               searchInput =>
               searchInput.Page == input.Page
               && searchInput.PerPage == input.PerPage
               && searchInput.Search == input.Search
               && searchInput.OrderBy == input.Sort
               && searchInput.Order == input.Dir
               ),
           It.IsAny<CancellationToken>()
           ), Times.Once);
    }

    [Fact(DisplayName = nameof(EmptyListWhenDoesntFindItems))]
    [Trait("Application", "ListCategories Use Cases")]
    public async Task EmptyListWhenDoesntFindItems()
    {

        var repo = _fixture.GetRepositoryMock();

        var input = _fixture.GetExampleInput();

        var outputRepoSearch = new SearchOutput<DomainEntity.Category>(
                currentPage: input.Page,
                perPage: input.PerPage,
                items: new List<DomainEntity.Category>().AsReadOnly(),
                total: 0);

        repo.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput =>
                searchInput.Page == input.Page
                && searchInput.PerPage == input.PerPage
                && searchInput.Search == input.Search
                && searchInput.OrderBy == input.Sort
                && searchInput.Order == input.Dir
                ),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepoSearch);

        var useCase = new UseCase.ListCategories(repo.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepoSearch.CurrentPage);
        output.PerPage.Should().Be(outputRepoSearch.PerPage);
        output.Total.Should().Be(outputRepoSearch.Total);
        output.Items.Should().HaveCount(0);

        repo.Verify(x => x.Search(
           It.Is<SearchInput>(
               searchInput =>
               searchInput.Page == input.Page
               && searchInput.PerPage == input.PerPage
               && searchInput.Search == input.Search
               && searchInput.OrderBy == input.Sort
               && searchInput.Order == input.Dir
               ),
           It.IsAny<CancellationToken>()
           ), Times.Once);
    }
}
