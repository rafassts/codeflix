﻿using Codeflix.Catalog.Application.Exceptions;
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

    public Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Genre genre, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
