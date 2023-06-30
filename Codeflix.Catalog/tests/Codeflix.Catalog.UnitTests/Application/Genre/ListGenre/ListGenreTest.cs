using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Moq;
using DomainEntity = Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using Codeflix.Catalog.Application.UseCases.Genre.ListGenres;

namespace Codeflix.Catalog.UnitTests.Application.Genre.ListGenre;

[Collection(nameof(ListGenreTestFixture))]
public class ListGenreTest
{

    private readonly ListGenreTestFixture _fixture;

    public ListGenreTest(ListGenreTestFixture fixture) => _fixture=fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "GetGenre Use Cases")]
    private async Task List()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();

        var input = _fixture.GetExampleInput();

        var outputRepoSearch = new SearchOutput<DomainEntity.Genre>(
              currentPage: input.Page,
              perPage: input.PerPage,
              items: _fixture.GetExampleGenreList(),
              total: new Random().Next(50, 200));

        genreRepoMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepoSearch);

        var useCase = new ListGenres(genreRepoMock.Object);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(outputRepoSearch.CurrentPage);
        output.PerPage.Should().Be(outputRepoSearch.PerPage);
        output.Total.Should().Be(outputRepoSearch.Total);
        output.Items.Should().HaveCount(outputRepoSearch.Items.Count);

        output.Items.ToList().ForEach(outputItem =>
        {
            var repoGenre = outputRepoSearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);

            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repoGenre!.Name);
            outputItem.IsActive.Should().Be(repoGenre!.IsActive);
            outputItem.CreatedAt.Should().Be(repoGenre!.CreatedAt);
            outputItem.Categories.Should().HaveCount(repoGenre.Categories.Count);

            foreach (var item in repoGenre.Categories)
                outputItem.Categories.Should().Contain(item);   

        });

        genreRepoMock.Verify(x => x.Search(
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

    [Fact(DisplayName = nameof(ListEmpty))]
    [Trait("Application", "GetGenre Use Cases")]
    private async Task ListEmpty()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();

        var input = _fixture.GetExampleInput();

        var outputRepoSearch = new SearchOutput<DomainEntity.Genre>(
              currentPage: input.Page,
              perPage: input.PerPage,
              items: new List<DomainEntity.Genre>(),
              total: new Random().Next(50, 200));

        genreRepoMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepoSearch);

        var useCase = new ListGenres(genreRepoMock.Object);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(outputRepoSearch.CurrentPage);
        output.PerPage.Should().Be(outputRepoSearch.PerPage);
        output.Total.Should().Be(outputRepoSearch.Total);
        output.Items.Should().HaveCount(outputRepoSearch.Items.Count);

        genreRepoMock.Verify(x => x.Search(
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

    [Fact(DisplayName = nameof(ListUsingDefaultInputValues))]
    [Trait("Application", "GetGenre Use Cases")]
    private async Task ListUsingDefaultInputValues()
    {
        var genreRepoMock = _fixture.GetGenreRepositoryMock();

        var outputRepoSearch = new SearchOutput<DomainEntity.Genre>(
              currentPage: 1,
              perPage: 15,
              items: new List<DomainEntity.Genre>(),
              total: 0);

        genreRepoMock.Setup(x => x.Search(
            It.IsAny<SearchInput>(),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(outputRepoSearch);

        var useCase = new ListGenres(genreRepoMock.Object);

        ListGenresOutput output = await useCase.Handle(new ListGenresInput(), CancellationToken.None);

        genreRepoMock.Verify(x => x.Search(
             It.Is<SearchInput>(
                 searchInput =>
                 searchInput.Page == 1
                 && searchInput.PerPage == 15
                 && searchInput.Search == ""
                 && searchInput.OrderBy == ""
                 && searchInput.Order == SearchOrder.Asc
                 ),
             It.IsAny<CancellationToken>()
             ), Times.Once);

    }
}
