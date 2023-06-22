using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
using Codeflix.Catalog.Domain.Repository;
using DomainEntity = Codeflix.Catalog.Domain.Entity;

namespace Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenre : ICreateGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGenre(
        IGenreRepository genreRepository,
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(CreateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = new DomainEntity.Genre(request.Name, request.IsActive);

        if (request.CategoriesIds is not null)
            request.CategoriesIds.ForEach(genre.AddCategory); //mesmo que id => genre.AddCategory(id)

        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }
}
