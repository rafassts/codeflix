using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Codeflix.Catalog.Infra.Data.EF.Repositories;
public class GenreRepository : IGenreRepository
{
    private readonly CodeflixCatalogDbContext _context;

    public GenreRepository(CodeflixCatalogDbContext context) => _context = context;

    private DbSet<Genre> _genres => _context.Set<Genre>();

    private DbSet<GenresCategories> _genresCategories => _context.Set<GenresCategories>();

    public Task Delete(Genre genre, CancellationToken cancellationToken)
    {
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));

        _genres.Remove(genre);

        return Task.CompletedTask;
    }

    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genres
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found");

        var categoryIds = await _genresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);

        categoryIds.ForEach(genre!.AddCategory);

        return genre;
    }

    public async Task Insert(Genre genre, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(genre);

        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories.Select(catId => new GenresCategories(catId, genre.Id));
            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        _genres.Update(genre);
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));

        if (genre.Categories.Count > 0)
        {
            var relations = genre
                .Categories
                .Select(categoryId => new GenresCategories(categoryId, genre.Id));

            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;

        var genres = await _genres
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync();

        var total = await _genres.CountAsync();

        var genresIds = genres.Select(x => x.Id).ToList();

        //todos os relacionamentos da lista de gêneros
        var relations = await _genresCategories
            .Where(relation => genresIds.Contains(relation.GenreId))
            .ToListAsync();

        /*
         * agrupa os generos iguais 
         * G1 C1 
         * G1 C2 
         * G2 C1 
         * G2 C2  
         * fica: 
         * KEY G1 [C1,C2]
         * KEY G2 [C1,C2]
         */
        var relationsByGenreIdGroup = relations.GroupBy(x => x.GenreId).ToList();

        //percorre o agrupamento
        relationsByGenreIdGroup.ForEach(relationGroup =>
        {
            //encontra cada gênero da lista (a chave do agrupamento é o genreId)
            var genre = genres.Find(genre => genre.Id == relationGroup.Key);

            if (genre is null)
                return;

            //adiciona no gênero encontrato as categorias que estão no relacionamento dele
            relationGroup.ToList().ForEach(relation => genre.AddCategory(relation.CategoryId));
        });

        return new SearchOutput<Genre>(input.Page, input.PerPage, total, genres);
    }

}
