using Codeflix.Catalog.Application.Exceptions;
using Codeflix.Catalog.Application.Interfaces;
using Codeflix.Catalog.Application.UseCases.Genre.Common;
using Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using Codeflix.Catalog.Domain.Repository;

namespace Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
public class UpdateGenre : IUpdateGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGenre(
        IGenreRepository genreRepository,
        IUnitOfWork unitOfWork,
        ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(UpdateGenreInput input, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(input.Id, cancellationToken);

        genre.Update(input.Name);
        if(input.IsActive is not null && input.IsActive != genre.IsActive)
        {
            if((bool) input.IsActive)
                genre.Activate();
            else
                genre.Deactivate();
        }

        if(input.CategoriesIds is not null)
        {
            genre.RemoveAllCategories();
            if (input.CategoriesIds.Count > 0)
            {
                await ValidateCategoriesIds(input, cancellationToken);
                input.CategoriesIds!.ForEach(id => genre.AddCategory(id));
            }
        }

        await _genreRepository.Update(genre,cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(UpdateGenreInput input, CancellationToken cancellationToken)
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
