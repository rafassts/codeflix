using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
public interface IListGenres : IRequestHandler<ListGenresInput, ListGenresOutput>
{
}
