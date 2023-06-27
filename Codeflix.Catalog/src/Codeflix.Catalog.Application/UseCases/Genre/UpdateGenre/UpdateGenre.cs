﻿using Codeflix.Catalog.Application.Exceptions;
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

    public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _genreRepository.Get(request.Id, cancellationToken);

        genre.Update(request.Name);
        if(request.IsActive is not null && request.IsActive != genre.IsActive)
        {
            if((bool) request.IsActive)
                genre.Activate();
            else
                genre.Deactivate();
        }

        if((request.CategoriesIds?.Count ?? 0) > 0)
        {
           await ValidateCategoriesIds(request, cancellationToken);
            genre.RemoveAllCategories();
            request.CategoriesIds!.ForEach(id => genre.AddCategory(id));
        }

        await _genreRepository.Update(genre,cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }

    private async Task ValidateCategoriesIds(UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var idsInPersistence =
        await _categoryRepository.GetIdsListByIds(request.CategoriesIds!, cancellationToken);

        if (idsInPersistence.Count < request.CategoriesIds!.Count)
        {
            var notFoundIds = request.CategoriesIds.FindAll(x => !idsInPersistence.Contains(x));
            var notFoundIdsString = String.Join(", ", notFoundIds);
            throw new RelatedAggregateException($"Related category id (or ids) not found: '{notFoundIdsString}'");
        }
    }
}
