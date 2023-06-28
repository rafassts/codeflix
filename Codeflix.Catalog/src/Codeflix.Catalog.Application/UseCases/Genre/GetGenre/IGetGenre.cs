using Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
public interface IGetGenre : IRequestHandler<GetGenreInput, GenreModelOutput>
{

}
