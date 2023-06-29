using Codeflix.Catalog.Application.UseCases.Genre.Common;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Moq;
using UseCase = Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

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

        var useCase = new UseCase.ListGenre(genreRepoMock.Object);

        ListGenresOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
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
            outputItem.Categories.Should().HaveCount(repoGenre.Categories.List);

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
}
