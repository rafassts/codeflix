using Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
public class UpdateGenreInput : IRequest<GenreModelOutput>
{
    public UpdateGenreInput(Guid id, string name, bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        Name = name;
        IsActive = isActive;
        Id=id;
        CategoriesIds = categoriesIds;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool? IsActive { get; set; }
    public List<Guid>? CategoriesIds { get; set; }
}
