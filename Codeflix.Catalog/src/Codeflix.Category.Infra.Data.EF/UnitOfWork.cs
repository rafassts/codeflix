using Codeflix.Catalog.Application.Interfaces;

namespace Codeflix.Catalog.Infra.Data.EF;
public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCategoryDbContext _context;

    public UnitOfWork(CodeflixCategoryDbContext context)
    {
        _context=context;
    }

    public Task Commit(CancellationToken cancellationToken)
        => _context.SaveChangesAsync(cancellationToken);

    public Task Rollback(CancellationToken cancellationToken)
      => Task.CompletedTask;
}
