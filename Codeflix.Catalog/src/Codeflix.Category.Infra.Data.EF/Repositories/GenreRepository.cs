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

    public GenreRepository(CodeflixCatalogDbContext context) => _context=context;

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
            .Where(x => x.GenreId ==  id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);

        categoryIds.ForEach(genre!.AddCategory);

        return genre;
    }

    public async Task Insert(Genre genre, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(genre);

        if(genre.Categories.Count > 0)
        {
            var relations = genre.Categories.Select(catId => new GenresCategories(catId, genre.Id));
            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var genres = await _genres.ToListAsync();

        return new SearchOutput<Genre>(input.Page, input.PerPage, genres.Count, genres);
    }

    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        _genres.Update(genre);
        _genresCategories.RemoveRange(_genresCategories.Where(x => x.GenreId == genre.Id));
        
        if(genre.Categories.Count > 0)
        {
            var relations = genre
                .Categories
                .Select(categoryId => new GenresCategories(categoryId, genre.Id));

            await _genresCategories.AddRangeAsync(relations);
        }
    }

    private IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderProperty, SearchOrder order)
    {
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };

        return orderedQuery.ThenBy(x => x.CreatedAt);
    }
}
