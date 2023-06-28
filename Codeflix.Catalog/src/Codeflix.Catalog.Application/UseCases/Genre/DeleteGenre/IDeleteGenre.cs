using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
public interface IDeleteGenre : IRequestHandler<DeleteGenreInput>
{
}
