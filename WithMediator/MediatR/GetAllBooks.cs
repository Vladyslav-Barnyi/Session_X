using MediatR;
using WithMediator.Contracts.Reposnes;
using WithMediator.Extensions;
using WithMediator.Repositories.Interfaces;

namespace WithMediator.MediatR;

public record GetAllBooksQuery : IRequest<List<GetBookResponse>>;

public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, List<GetBookResponse>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    public async Task<List<GetBookResponse>> Handle(GetAllBooksQuery request, CancellationToken ct)
    {
        var books = await _bookRepository.GetAll(ct);
        var response = books.Select(b=>b.ToGetBookResponse()).ToList();

        return response;
    }
}