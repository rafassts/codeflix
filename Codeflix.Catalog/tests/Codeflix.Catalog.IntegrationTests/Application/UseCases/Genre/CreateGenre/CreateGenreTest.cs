using Codeflix.Catalog.Infra.Data.EF.Repositories;
using Codeflix.Catalog.Infra.Data.EF;
using AppUseCases = Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;

namespace Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(Create))]
    [Trait("Integration/Application", "CreateGenre Use Cases")]
    public async void Create()
    {
        var dbContext = _fixture.CreateDbContext();

        var useCase = new AppUseCases.CreateGenre(
            new GenreRepository(dbContext),
             new UnitOfWork(dbContext),
            new CategoryRepository(dbContext));

        var input = _fixture.GetExampleInput();

        GenreModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);

        //ver se salvou no banco
        var dbContext2 = _fixture.CreateDbContext(true);

        var genreFromDb = await dbContext2
            .Genres
            .FindAsync(output.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
        genreFromDb.CreatedAt.Should().Be(output.CreatedAt);

        

    }
}
