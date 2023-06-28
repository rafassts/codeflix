using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
using Codeflix.Catalog.Domain.Entity;
using Codeflix.Catalog.Domain.Repository;
using MediatR;
using System.Threading;
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

    public async Task<GenreModelOutput> Handle(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var genre = new DomainEntity.Genre(input.Name, input.IsActive);

        if ((input.CategoriesIds?.Count ?? 0) > 0)
        {
            await ValidateCategoriesIds(input, cancellationToken);
            input.CategoriesIds?.ForEach(genre.AddCategory); //mesmo que (id => genre.AddCategory(id))
        }
        
        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(CreateGenreInput input, CancellationToken cancellationToken)
    {
        var idsInPersistence =
        await _categoryRepository.GetIdsListByIds(input.CategoriesIds!, cancellationToken);

        if (idsInPersistence.Count < input.CategoriesIds!.Count)
        {
            var notFoundIds = input.CategoriesIds.FindAll(x => !idsInPersistence.Contains(x));
            var notFoundIdsString = String.Join(", ", notFoundIds);
            throw new RelatedAggregateException($"Related category id (or ids) not found: '{notFoundIdsString}'");
        }
    }
}
