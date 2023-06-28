using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Domain.Repository;

namespace Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
public class DeleteGenre : IDeleteGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteGenreInput input, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(input.Id, cancellationToken);

        await _genreRepository.Delete(genre,cancellationToken);

        await _unitOfWork.Commit(cancellationToken);
    }
}
