using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Repository;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace Codeflix.Catalog.Infra.Data.EF.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly CodeflixCategoryDbContext _context;
    private DbSet<Category> _categories 
        => _context.Set<Category>();

    public CategoryRepository(CodeflixCategoryDbContext context)
        => _context=context;  

    public Task Delete(Category aggregate, CancellationToken _)
    {
        //o ef não tem delete async
        return Task.FromResult(_categories.Remove(aggregate));
    }

    public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(category, $"Category '{id}' not found");
        return category!;
    }
        
    public async Task Insert(Category aggregate, CancellationToken cancellationToken)
        => await _categories.AddAsync(aggregate, cancellationToken);
    
    public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        
        var total = await _categories.CountAsync();
        
        var items = await _categories
            .AsNoTracking()
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync();

        return new SearchOutput<Category>(input.Page, input.PerPage, total, items);
    }

    public Task Update(Category aggregate, CancellationToken _)
    {
        //o ef não tem update async
        return Task.FromResult(_categories.Update(aggregate));
    }
}
