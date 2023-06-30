using Codeflix.Catalog.Application.Common;
using Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenresInput : 
    PaginatedListInput,
    IRequest<ListGenresOutput>
{
    public ListGenresInput(
        int page = 1, 
        int perPage = 15, 
        string search = "", 
        string sort = "",
        SearchOrder dir = SearchOrder.Asc) : base(page, perPage, search, sort, dir)
    {
    }

}
