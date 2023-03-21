namespace Codeflix.Catalog.Application.Interfaces;
public interface IUnitOfWork
{
    public Task Commit(CancellationToken cancellationToken);
    //só fez para ter no patter, pois no ef não precisa
    public Task Rollback(CancellationToken cancellationToken);
}
